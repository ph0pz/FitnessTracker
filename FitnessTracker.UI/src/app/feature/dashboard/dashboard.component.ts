// src/app/features/dashboard/dashboard.component.ts (UPDATED)
import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { format } from 'date-fns';

// Chart Imports
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData, ChartOptions, ChartType } from 'chart.js';
import { DashboardService } from '../../core/service/dashboard.service';
import { DashboardSummary } from '../../models/dashboard-summary.model';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SharedMaterialModule } from '../../core/shared.module';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    SharedMaterialModule,
    BaseChartDirective // Ensure this is imported for Chart.js canvases
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  dashboardSummary: DashboardSummary | null = null;
  selectedDateControl = new FormControl(new Date());
  isLoading: boolean = false;
  errorMessage: string | null = null;

  // --- Chart.js Properties ---
  // Pie Chart Options
  public pieChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: function(context) {
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

  // FIX: Initialize datasets with an empty dataset object
  public caloriesPieData: ChartData<'doughnut'> = {
    labels: ['Consumed', 'Remaining'], // Initialize labels too
    datasets: [{
      data: [], // Initialize with an empty data array
      backgroundColor: ['#4CAF50', '#FFCDD2'], // Example colors
      hoverBackgroundColor: ['#66BB6A', '#EF9A9A'],
      borderWidth: 0,
      label: 'kcal' // Add a label for tooltips
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

  // Line Chart Options and Data
  public lineChartData: ChartData<'line'> = {
    labels: [], // Initialize with empty labels
    datasets: [
      {
        data: [], // Initialize with empty data
        label: 'Weight (kg)',
        borderColor: '#3f51b5',
        backgroundColor: 'rgba(63, 81, 181, 0.2)',
        pointBackgroundColor: '#3f51b5',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: '#3f51b5',
        fill: 'origin',
        tension: 0.3
      }
    ]
  };
  public lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: true, position: 'top' },
      title: { display: false }
    },
    scales: {
      x: {
        title: {
          display: true,
          text: 'Date'
        }
      },
      y: {
        title: {
          display: true,
          text: 'Weight (kg)'
        },
        beginAtZero: false
      }
    }
  };
  public lineChartType: 'line' = 'line';


  constructor(private dashboardService: DashboardService, private router: Router) { }

  ngOnInit(): void {
    // Initial fetch of dashboard summary for the selected date
    this.fetchDashboardSummary(this.selectedDateControl.value);

    // Subscribe to date changes to refetch data
    this.selectedDateControl.valueChanges.subscribe(date => {
      if (date) {
        this.fetchDashboardSummary(date);
      }
    });
  }

  fetchDashboardSummary(date: Date | null): void {
    if (!date) {
      this.errorMessage = "Please select a valid date.";
      return;
    }
    this.isLoading = true;
    this.errorMessage = null;

    const formattedDate = format(date, 'yyyy-MM-dd');

    this.dashboardService.getDailySummary(formattedDate).subscribe({
      next: (data: DashboardSummary) => {
        this.dashboardSummary = data;
        this.isLoading = false;
        this.updateAllPieCharts(data);
        // You would likely also call a method here to update your line chart data
        // For example: this.updateLineChartData(data.historicalWeightData);
      },
      error: (err: any) => {
        console.error('Error fetching dashboard summary:', err);
        this.errorMessage = 'Failed to load dashboard data. Please try again.';
        this.dashboardSummary = null;
        this.isLoading = false;
      }
    });
  }

  updateAllPieCharts(data: DashboardSummary): void {
    // Ensure the datasets array and the first dataset object exist before trying to set data
    // This is handled by the improved initialization above.
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

    // Important: Tell ng2-charts to update the charts
    // If you're using ViewChild directives for each chart, you can call their .update() method.
    // However, if the data reference itself changes (which it doesn't here, only array contents),
    // ng2-charts might not detect it automatically.
    // If charts don't update, consider making a copy of the datasets or calling ViewChild.update()
    // For example:
    // if (this.chartDirective) { // assuming @ViewChild('caloriesChart') chartDirective!: BaseChartDirective;
    //   this.chartDirective.update();
    // }
    // A simpler way (often works for ng2-charts) is to reassign the entire data object
    // this.caloriesPieData = { ...this.caloriesPieData };
    // Or just reassign the datasets array
    this.caloriesPieData.datasets = [...this.caloriesPieData.datasets];
    this.proteinPieData.datasets = [...this.proteinPieData.datasets];
    this.carbsPieData.datasets = [...this.carbsPieData.datasets];
    this.fatPieData.datasets = [...this.fatPieData.datasets];
  }


  addMeal(): void {
    this.router.navigate(['/meals']);
  }

}