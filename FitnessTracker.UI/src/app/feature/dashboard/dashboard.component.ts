// src/app/features/dashboard/dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // Needed for *ngIf, *ngFor etc.
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

// Optional: If you want to use Angular Material components directly in the dashboard,
// import them here. For a simple dashboard, you might not need many.
// Example:
// import { MatCardModule } from '@angular/material/card';
// import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dashboard',
  standalone: true, // <--- This marks it as a standalone component
  imports: [
    CommonModule,
    // Add any Angular Material modules you'll use directly in this component's template
    MatCardModule,
    MatButtonModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  // You can add properties here to display dynamic data, e.g.:
  welcomeMessage: string = 'Welcome to your Fitness Tracker Dashboard!';
  upcomingWorkouts: string[] = ['Leg Day (Mon)', 'Upper Body (Wed)', 'Cardio (Fri)'];
  progressSummary: string = 'Keep up the great work!';

  constructor() { }

  ngOnInit(): void {
    // This method is called once when the component is initialized.
    // You might fetch data from your API here, e.g., this.dashboardService.getDashboardData().subscribe(...)
    console.log('DashboardComponent initialized.');
  }

  // Example method if you have interactive elements
  viewWorkoutDetails(workout: string): void {
    alert(`Viewing details for: ${workout}`);
  }
}