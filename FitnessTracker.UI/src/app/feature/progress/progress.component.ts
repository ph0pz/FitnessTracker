import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs'; // For managing subscriptions
import { share, takeUntil } from 'rxjs/operators'; // For managing subscriptions
import { UserMetrics, WeightLog } from '../../models/progress.model'; // Assuming WeightLog is defined here
import { SharedMaterialModule } from '../../core/shared.module';
import { MatDialog } from '@angular/material/dialog';
import { CategoryScale, Chart, ChartData, ChartOptions, ChartType, Legend, LinearScale, LineController, LineElement, PointElement, Tooltip } from 'chart.js';
import { LogMetricsDialogComponent, LogMetricsDialogData } from './log-metric/log-metric.component'; // Ensure correct path
import { ProgressService } from '../../core/service/progres.service'; // Ensure correct path
import { BaseChartDirective } from 'ng2-charts';



@Component({
  selector: 'app-progress-tracking',
  templateUrl: './progress.component.html',
  styleUrls: ['./progress.component.scss'],
  standalone: true,
  imports: [SharedMaterialModule, BaseChartDirective],
})
export class ProgressTrackingComponent implements OnInit, OnDestroy {

  title = 'Progress Tracking';
  isLoading = true;
  private destroy$ = new Subject<void>();

  // NEW: Property to hold today's (latest) metrics
  latestMetrics: WeightLog | null = null; 

  // --- Combined Chart.js properties ---
  public combinedLineChartData: ChartData<ChartType> = { datasets: [], labels: [] };

  public combinedLineChartOptions: ChartOptions<ChartType> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: true, position: 'top' },
      tooltip: {
        mode: 'index',
        intersect: false, // Show all data points for a given date
        callbacks: {
          label: function(context) {
            let label = context.dataset.label || '';
            if (label) {
              label += ': ';
            }
            if (context.parsed.y !== null) {
              label += context.parsed.y;
              // Add unit based on dataset's yAxisID
              if (context.dataset.yAxisID === 'y-weight') {
                label += ' kg';
              } else if (context.dataset.yAxisID === 'y-waist') {
                label += ' cm';
              } else if (context.dataset.yAxisID === 'y-bodyfat') {
                label += ' %';
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
      'y-weight': { // Unique ID for weight axis
        type: 'linear',
        position: 'left',
        title: { display: true, text: 'Weight (kg)' },
        grid: {
          drawOnChartArea: true, // Only draw grid lines for this axis
        },
        beginAtZero: false,
      },
      'y-waist': { // Unique ID for waist size axis
        type: 'linear',
        position: 'right', // Place on the right side
        title: { display: true, text: 'Waist Size (cm)' },
        grid: {
          drawOnChartArea: false, // Do not draw grid lines for this axis to avoid clutter
        },
        beginAtZero: false,
      },
      'y-bodyfat': { // Unique ID for body fat axis
        type: 'linear',
        position: 'right', // Also on the right, but Chart.js will stack them
        title: { display: true, text: 'Body Fat (%)' },
        grid: {
          drawOnChartArea: false, // Do not draw grid lines for this axis
        },
        beginAtZero: true, // Body fat can realistically be closer to 0 for very lean individuals
      }
    }
  };
  public lineChartType: ChartType = 'line';

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private progressService: ProgressService
  ) { }

  ngOnInit(): void {
    this.fetchProgressData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  fetchProgressData(): void {
    this.isLoading = true;
    this.progressService.getWeightHistory().pipe(takeUntil(this.destroy$)).subscribe({
      next: (data: WeightLog[]) => {
        // Sort data by logDate to ensure correct chart display and to easily find the latest
        const sortedData = data.sort((a, b) => new Date(a.logDate).getTime() - new Date(b.logDate).getTime());

        // NEW: Get the latest log entry
        this.latestMetrics = sortedData.length > 0 ? sortedData[sortedData.length - 1] : null;

        const labels = sortedData.map(log => new Date(log.logDate).toLocaleDateString());
        const weights = sortedData.map(log => log.weight);
        const waistSizes = sortedData.map(log => log.waistSizeCm);
        const bodyFatPercentages = sortedData.map(log => log.bodyFatPercentage);

        this.combinedLineChartData = {
          labels: labels,
          datasets: [
            {
              data: weights,
              label: 'Weight',
              borderColor: '#3f51b5',
              backgroundColor: 'rgba(63, 81, 181, 0.2)',
              pointBackgroundColor: '#3f51b5',
              pointBorderColor: '#fff',
              pointHoverBackgroundColor: '#fff',
              pointHoverBorderColor: '#3f51b5',
              fill: 'origin',
              tension: 0.3,
              yAxisID: 'y-weight'
            },
            {
              data: waistSizes,
              label: 'Waist Size',
              borderColor: '#ff9800',
              backgroundColor: 'rgba(255, 152, 0, 0.2)',
              pointBackgroundColor: '#ff9800',
              pointBorderColor: '#fff',
              pointHoverBackgroundColor: '#fff',
              pointHoverBorderColor: '#ff9800',
              fill: 'origin',
              tension: 0.3,
              yAxisID: 'y-waist'
            },
            {
              data: bodyFatPercentages,
              label: 'Body Fat %',
              borderColor: '#4caf50',
              backgroundColor: 'rgba(76, 175, 80, 0.2)',
              pointBackgroundColor: '#4caf50',
              pointBorderColor: '#fff',
              pointHoverBackgroundColor: '#fff',
              pointHoverBorderColor: '#4caf50',
              fill: 'origin',
              tension: 0.3,
              yAxisID: 'y-bodyfat'
            }
          ]
        };

        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Error fetching progress data:', err);
        this.isLoading = false;
        this.latestMetrics = null; // Clear latest metrics on error
        this.combinedLineChartData = { datasets: [], labels: [] };
      }
    });
  }

  onBackToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  onLogNewMetrics(): void {
    // Pass existing values to the dialog if you want to pre-fill
    // Example: pass this.latestMetrics to the dialog data
    const dialogData: LogMetricsDialogData = {
      logDate: new Date(), // Default to today
      weight: this.latestMetrics?.weight || null,
      waistSizeCm: this.latestMetrics?.waistSizeCm || null,
      bodyFatPercentage: this.latestMetrics?.bodyFatPercentage || null,
      notes: ''
    };

    const dialogRef = this.dialog.open(LogMetricsDialogComponent, {
      width: '400px',
      data: dialogData // Pass initial data
    });

    dialogRef.afterClosed().subscribe((result: LogMetricsDialogData | undefined) => {
      if (result) {
        console.log('Dialog result:', result);
        this.logMetricsToApi(result);
      } else {
        console.log('Dialog closed without saving.');
      }
    });
  }

  logMetricsToApi(data: LogMetricsDialogData): void {
   const payload: any = { // Use 'any' or a new type that reflects optional properties
    logDate: data.logDate.toISOString(),
    notes: data.notes // notes can be an empty string if not provided
  };

    if (data.weight !== null) {
      payload.weight = data.weight;
    }
    if (data.waistSizeCm !== null) {
      payload.waistSizeCm = data.waistSizeCm;
    }
    if (data.bodyFatPercentage !== null) {
      payload.bodyFatPercentage = data.bodyFatPercentage;
    }

    this.progressService.logWeight(payload).pipe(takeUntil(this.destroy$)).subscribe({
      next: (response) => {
        console.log('Metrics logged successfully:', response);
        this.fetchProgressData(); // Re-fetch to update the combined chart and latest metrics
      },
      error: (error) => {
        console.error('Error logging metrics:', error);
      }
    });
  }
}