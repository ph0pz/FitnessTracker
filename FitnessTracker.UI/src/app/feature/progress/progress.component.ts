import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs'; // For managing subscriptions
import { share, takeUntil } from 'rxjs/operators'; // For managing subscriptions
import { UserMetrics, WeightLog } from '../../models/progress.model';
import { SharedMaterialModule } from '../../core/shared.module';
import { MatDialog } from '@angular/material/dialog';
import { CategoryScale, Chart, ChartData, ChartOptions, ChartType, Legend, LinearScale, LineController, LineElement, PointElement, Tooltip } from 'chart.js';
import { LogMetricsDialogComponent, LogMetricsDialogData } from './log-metric/log-metric.component';
import { ProgressService } from '../../core/service/progres.service';
import { BaseChartDirective } from 'ng2-charts';

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
  selector: 'app-progress-tracking',
  templateUrl: './progress.component.html',
  styleUrls: ['./progress.component.scss'],
  // If this is a standalone component, you'd add:
  standalone: true,
  imports: [SharedMaterialModule , BaseChartDirective], // Import your shared material module if needed
  // imports: [SharedMaterialModule, CommonModule, ...] // Adjust imports based on your SharedMaterialModule and if it's standalone
})
export class ProgressTrackingComponent implements OnInit, OnDestroy {

  title = 'Progress Tracking';
  isLoading = true;
  private destroy$ = new Subject<void>();

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
  public lineChartType: ChartType = 'line'; // FIX: Changed type to ChartType

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
        // Sort data by logDate to ensure correct chart display
        const sortedData = data.sort((a, b) => new Date(a.logDate).getTime() - new Date(b.logDate).getTime());

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
        this.combinedLineChartData = { datasets: [], labels: [] };
      }
    });
  }

  onBackToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  onLogNewMetrics(): void {
    const dialogRef = this.dialog.open(LogMetricsDialogComponent, {
      width: '400px',
      data: {}
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
    const payload = {
      logDate: data.logDate.toISOString(),
      weight: data.weight,
      waistSizeCm: data.waistSizeCm,
      bodyFatPercentage: data.bodyFatPercentage,
      notes: data.notes
    };

    this.progressService.logWeight(payload).pipe(takeUntil(this.destroy$)).subscribe({
      next: (response) => {
        console.log('Metrics logged successfully:', response);
        this.fetchProgressData(); // Re-fetch to update the combined chart
      },
      error: (error) => {
        console.error('Error logging metrics:', error);
      }
    });
  }


}