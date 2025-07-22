import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs'; // For managing subscriptions
import { share, takeUntil } from 'rxjs/operators'; // For managing subscriptions
import { UserMetrics } from '../../models/progress.model';
import { SharedMaterialModule } from '../../core/shared.module';


@Component({
  selector: 'app-progress-tracking',
  templateUrl: './progress.component.html',
  styleUrls: ['./progress.component.scss'],
  // If this is a standalone component, you'd add:
  standalone: true,
  imports: [SharedMaterialModule], // Import your shared material module if needed
  // imports: [SharedMaterialModule, CommonModule, ...] // Adjust imports based on your SharedMaterialModule and if it's standalone
})
export class ProgressTrackingComponent implements OnInit, OnDestroy {
  title = 'Progress Tracking';
  weightTrendData: number[] = []; // Placeholder for chart data
  userMetrics: UserMetrics;

  // Placeholder for simulating data fetching
  isLoading = true;
  private destroy$ = new Subject<void>(); // Used to unsubscribe from observables

  constructor(private router: Router) {
    // Initialize with some mock data for demonstration
    this.userMetrics = {
      weight: [180, 179, 178, 177, 176, 175.5, 175, 174.5, 174, 173.5, 173, 172.8, 172.5, 172, 171.8, 171.5, 171.2, 171, 170.8, 170.5, 170.2, 170, 169.8, 169.5, 169.2, 169, 168.8, 168.5, 168.2, 168], // Last 30 days dummy data
      waistSize: { value: 80, unit: 'cm', average: 79.5 },
      bodyFat: { value: 18, unit: '%', average: 18.2 },
    };
  }

  ngOnInit(): void {
    // Simulate fetching data
    this.fetchProgressData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  fetchProgressData(): void {
    this.isLoading = true;
    // In a real application, you would call a service here to get data
    // Example: this.progressService.getMetrics().pipe(takeUntil(this.destroy$)).subscribe(data => { ... });

    setTimeout(() => { // Simulate API call delay
      // After fetching, update properties
      // this.userMetrics = fetchedData.userMetrics;
      // this.weightTrendData = fetchedData.weightData;
      this.isLoading = false;
    }, 1500);
  }

  // --- Event Handlers ---

  onBackToDashboard(): void {
    this.router.navigate(['/dashboard']); // Adjust route as necessary
  }

  onLogNewMetrics(): void {
    console.log('Log New Weight/Metrics clicked');
    // Navigate to a form for logging new data
    this.router.navigate(['/log-metrics']); // Adjust route as necessary
  }

  onViewDetailedHistory(): void {
    console.log('View Detailed History clicked');
    // Navigate to a detailed history page
    this.router.navigate(['/history']); // Adjust route as necessary
  }
}