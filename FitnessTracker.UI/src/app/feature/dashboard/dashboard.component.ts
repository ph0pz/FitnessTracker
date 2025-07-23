import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { format } from 'date-fns';

// Chart Imports
import { BaseChartDirective } from 'ng2-charts';
import { Chart, CategoryScale, LinearScale, LineController, PointElement, LineElement, Legend, Tooltip } from 'chart.js';
import { ChartData, ChartOptions, ChartType } from 'chart.js'; // Import ChartType

// Services and Models
import { DashboardService } from '../../core/service/dashboard.service';
import { DashboardSummary, MacroGoal, MacroGoalInput } from '../../models/dashboard.model';
import { SharedMaterialModule } from '../../core/shared.module';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { EditMacrosDialogComponent } from './macro-dialog/macro-dialog.component'; // Ensure path is correct

// Register Chart.js components ONCE (if not already done globally or in a shared module)
Chart.register(
  CategoryScale,
  LinearScale,
  LineController,
  PointElement,
  LineElement,
  Legend,
  Tooltip
);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    SharedMaterialModule,
    BaseChartDirective, // Ensure this is imported for Chart.js canvases
    ReactiveFormsModule // Required for FormControl
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  // DashboardSummary for the *currently selected day* (from the list)
  dashboardSummaryForSelectedDay: DashboardSummary | null = null;
  
  // Stores the full list of 30-day summaries from the API
  dashboardSummariesHistory: DashboardSummary[] = [];

  // Date picker control to select which day's summary is displayed
  selectedDateControl = new FormControl(new Date()); 
  
  isLoading: boolean = false;
  errorMessage: string | null = null;

  // --- Chart.js Properties for Daily Macro Consumption Trend (NEW) ---
  public macroConsumptionTrendChartData: ChartData<'line'> = { datasets: [], labels: [] };
  public macroConsumptionTrendChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: true, position: 'top' },
      tooltip: {
        mode: 'index',
        intersect: false,
        callbacks: {
          label: function(context) {
            let label = context.dataset.label || '';
            if (label) {
              label += ': ';
            }
            if (context.parsed.y !== null) {
              label += context.parsed.y;
              // Add unit based on dataset's yAxisID
              if (context.dataset.yAxisID === 'y-calories') {
                label += ' kcal';
              } else { // Protein, Carbs, Fat will use the 'y-grams' axis
                label += ' g';
              }
            }
            return label;
          }
        }
      }
    },
    scales: {
      x: {
        title: { display: true, text: 'Date' }
      },
      'y-grams': { // For Protein, Carbs, Fat
        type: 'linear',
        position: 'left',
        title: { display: true, text: 'Grams (g)' },
        grid: {
          drawOnChartArea: true,
        },
        beginAtZero: true
      },
      'y-calories': { // For Calories
        type: 'linear',
        position: 'right',
        title: { display: true, text: 'Calories (kcal)' },
        grid: {
          drawOnChartArea: false,
        },
        beginAtZero: true
      }
    }
  };
  public lineChartType: 'line' = 'line'; // Chart type for macro consumption trend

  // --- Pie Chart Properties (for the selected day's summary) ---
  public pieChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: function (context) {
            const label = context.label || '';
            if (context.parsed !== null) {
              return `${label}: ${context.parsed} ${context.dataset.label}`;
            }
            return label;
          }
        }
      }
    }
  };
  public pieChartType: 'doughnut' = 'doughnut';
  public caloriesPieData: ChartData<'doughnut'> = {
    labels: ['Consumed', 'Remaining'],
    datasets: [{
      data: [],
      backgroundColor: ['#4CAF50', '#FFCDD2'],
      hoverBackgroundColor: ['#66BB6A', '#EF9A9A'],
      borderWidth: 0,
      label: 'kcal'
    }]
  };
  public proteinPieData: ChartData<'doughnut'> = {
    labels: ['Consumed', 'Remaining'],
    datasets: [{
      data: [],
      backgroundColor: ['#2196F3', '#BBDEFB'],
      hoverBackgroundColor: ['#42A5F5', '#90CAF9'],
      borderWidth: 0,
      label: 'g'
    }]
  };
  public carbsPieData: ChartData<'doughnut'> = {
    labels: ['Consumed', 'Remaining'],
    datasets: [{
      data: [],
      backgroundColor: ['#FFC107', '#FFECB3'],
      hoverBackgroundColor: ['#FFD54F', '#FFE082'],
      borderWidth: 0,
      label: 'g'
    }]
  };
  public fatPieData: ChartData<'doughnut'> = {
    labels: ['Consumed', 'Remaining'],
    datasets: [{
      data: [],
      backgroundColor: ['#9C27B0', '#E1BEE7'],
      hoverBackgroundColor: ['#AB47BC', '#CE93D8'],
      borderWidth: 0,
      label: 'g'
    }]
  };

  constructor(
    private dashboardService: DashboardService,
    private router: Router,
    private dialog: MatDialog,
  ) { }

  ngOnInit(): void {
    // Initial fetch of the 30-day dashboard summaries
    // We pass today's date as the reference point for the backend
    this.fetchDashboardSummariesHistory(this.selectedDateControl.value);

    // Subscribe to date changes for the daily summary display
    // This will pick a specific day from the already fetched 30-day history
    this.selectedDateControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(date => {
        if (date) {
          this.updateDashboardForSelectedDate(date);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // NEW: Fetch the 30-day history of dashboard summaries
  fetchDashboardSummariesHistory(endDate: Date | null): void {
    if (!endDate) {
      this.errorMessage = "Please select a valid end date for the history.";
      return;
    }
    this.isLoading = true;
    this.errorMessage = null;

    const formattedEndDate = format(endDate, 'yyyy-MM-dd');

    this.dashboardService.getDailySummary(formattedEndDate).pipe(takeUntil(this.destroy$)).subscribe({
      next: (data: DashboardSummary[]) => {
        this.dashboardSummariesHistory = data;
        this.isLoading = false;

        // Select the latest day's summary for initial display
        const latestSummary = data.find(s => {
          const sDate = new Date(s.todayDate);
          return sDate.toDateString() === endDate.toDateString();
        });
        
        // If today's summary is not found in the list, try the very last one
        this.dashboardSummaryForSelectedDay = latestSummary || data[data.length - 1] || null;

        // Update pie charts for the selected day
        if (this.dashboardSummaryForSelectedDay) {
          this.updateAllPieCharts(this.dashboardSummaryForSelectedDay);
        } else {
          // Clear pie charts if no data for selected day
          this.clearAllPieCharts();
        }

        // Update the macro consumption trend chart
        this.updateMacroConsumptionTrendChart(data);
      },
      error: (err: any) => {
        console.error('Error fetching dashboard summaries history:', err);
        this.errorMessage = 'Failed to load dashboard data history. Please try again.';
        this.dashboardSummariesHistory = [];
        this.dashboardSummaryForSelectedDay = null;
        this.isLoading = false;
        this.clearAllPieCharts();
        this.clearMacroConsumptionTrendChart();
      }
    });
  }

  // NEW: Update the specific daily summary display based on selected date
  updateDashboardForSelectedDate(selectedDate: Date): void {
    const foundSummary = this.dashboardSummariesHistory.find(s => {
      const sDate = new Date(s.todayDate);
      // Compare dates without time to match backend's 'date' handling
      return sDate.toDateString() === selectedDate.toDateString();
    });

    this.dashboardSummaryForSelectedDay = foundSummary || null;

    if (this.dashboardSummaryForSelectedDay) {
      this.updateAllPieCharts(this.dashboardSummaryForSelectedDay);
    } else {
      this.clearAllPieCharts();
    }
  }

  // NEW: Update the Macro Consumption Trend Line Chart
  updateMacroConsumptionTrendChart(data: DashboardSummary[]): void {
    // Sort data by date to ensure correct chart order
    const sortedData = [...data].sort((a, b) => new Date(a.todayDate).getTime() - new Date(b.todayDate).getTime());

    const labels = sortedData.map(s => format(new Date(s.todayDate), 'MMM dd'));
    const calories = sortedData.map(s => s.caloriesConsumed);
    const protein = sortedData.map(s => s.proteinConsumed);
    const carbs = sortedData.map(s => s.carbsConsumed);
    const fat = sortedData.map(s => s.fatConsumed);

    this.macroConsumptionTrendChartData = {
      labels: labels,
      datasets: [
        {
          data: calories,
          label: 'Calories',
          borderColor: '#FF0000', // Red
          backgroundColor: 'rgba(255, 0, 0, 0.2)',
          pointBackgroundColor: '#FF0000',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: '#FF0000',
          fill: false,
          tension: 0.3,
          yAxisID: 'y-calories'
        },
        {
          data: protein,
          label: 'Protein',
          borderColor: '#3f51b5', // Blue
          backgroundColor: 'rgba(63, 81, 181, 0.2)',
          pointBackgroundColor: '#3f51b5',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: '#3f51b5',
          fill: false,
          tension: 0.3,
          yAxisID: 'y-grams'
        },
        {
          data: carbs,
          label: 'Carbs',
          borderColor: '#ff9800', // Orange
          backgroundColor: 'rgba(255, 152, 0, 0.2)',
          pointBackgroundColor: '#ff9800',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: '#ff9800',
          fill: false,
          tension: 0.3,
          yAxisID: 'y-grams'
        },
        {
          data: fat,
          label: 'Fat',
          borderColor: '#4caf50', // Green
          backgroundColor: 'rgba(76, 175, 80, 0.2)',
          pointBackgroundColor: '#4caf50',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: '#4caf50',
          fill: false,
          tension: 0.3,
          yAxisID: 'y-grams'
        }
      ]
    };
  }

  // Utility to clear pie chart data if no summary is available for a selected day
  private clearAllPieCharts(): void {
    this.caloriesPieData.datasets[0].data = [];
    this.proteinPieData.datasets[0].data = [];
    this.carbsPieData.datasets[0].data = [];
    this.fatPieData.datasets[0].data = [];
    // Ensure charts refresh by creating new object references
    this.caloriesPieData.datasets = [...this.caloriesPieData.datasets];
    this.proteinPieData.datasets = [...this.proteinPieData.datasets];
    this.carbsPieData.datasets = [...this.carbsPieData.datasets];
    this.fatPieData.datasets = [...this.fatPieData.datasets];
  }

  // Utility to clear macro consumption trend chart
  private clearMacroConsumptionTrendChart(): void {
    this.macroConsumptionTrendChartData = { datasets: [], labels: [] };
  }


  updateAllPieCharts(data: DashboardSummary): void {
    this.caloriesPieData.datasets[0].data = [
      data.caloriesConsumed,
      Math.max(data.dailyCalorieGoal - data.caloriesConsumed, 0)
    ];
    this.proteinPieData.datasets[0].data = [
      data.proteinConsumed,
      Math.max(data.dailyProteinGoal - data.proteinConsumed, 0)
    ];
    this.carbsPieData.datasets[0].data = [
      data.carbsConsumed,
      Math.max(data.dailyCarbGoal - data.carbsConsumed, 0)
    ];
    this.fatPieData.datasets[0].data = [
      data.fatConsumed,
      Math.max(data.dailyFatGoal - data.fatConsumed, 0)
    ];

    // Trigger chart update
    this.caloriesPieData.datasets = [...this.caloriesPieData.datasets];
    this.proteinPieData.datasets = [...this.proteinPieData.datasets];
    this.carbsPieData.datasets = [...this.carbsPieData.datasets];
    this.fatPieData.datasets = [...this.fatPieData.datasets];
  }


  addMeal(): void {
    this.router.navigate(['/meals']);
  }
  logWeight(): void {
    this.router.navigate(['/progress']);
  }

  editMacros(): void {
    // Pass the goal from the selected day's summary to the dialog if available
    let currentGoalForDialog: MacroGoalInput | undefined;
    if (this.dashboardSummaryForSelectedDay) {
      currentGoalForDialog = {
        // Use 'Maintenance' as a default if goalType isn't explicitly returned from backend in DashboardSummary
        goalType: 'Custom',
        calories: this.dashboardSummaryForSelectedDay.dailyCalorieGoal,
        protein: this.dashboardSummaryForSelectedDay.dailyProteinGoal,
        carbs: this.dashboardSummaryForSelectedDay.dailyCarbGoal,
        fat: this.dashboardSummaryForSelectedDay.dailyFatGoal,
      };
    }

    const dialogRef = this.dialog.open(EditMacrosDialogComponent, {
      width: '400px',
      data: {
        currentGoal: currentGoalForDialog
      }
    });

    dialogRef.afterClosed().pipe(takeUntil(this.destroy$)).subscribe((result: MacroGoalInput | undefined) => {
      if (result) {
        console.log('Dialog result (Macro Goal):', result);
        this.dashboardService.setMacroGoal(result).pipe(takeUntil(this.destroy$)).subscribe({
          next: (response) => {
            console.log('Macro Goal set successfully:', response);
            // After setting a new goal, re-fetch the history to update everything
            this.fetchDashboardSummariesHistory(this.selectedDateControl.value);
          },
          error: (err) => {
            console.error('Error setting macro goal:', err);
          }
        });
      } else {
        console.log('Macro Goal dialog closed without saving.');
      }
    });
  }
}