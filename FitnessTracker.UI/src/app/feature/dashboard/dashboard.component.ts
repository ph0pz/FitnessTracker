// src/app/features/dashboard/dashboard.component.ts (UPDATED)
import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms'; // <--- For date picker

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker'; // <--- Datepicker
import { MatNativeDateModule } from '@angular/material/core';     // <--- Datepicker
import { format } from 'date-fns'; // <--- NEW: For date formatting

// Chart Imports
import { BaseChartDirective } from 'ng2-charts'; // <--- Charting
import { ChartConfiguration, ChartData, ChartType } from 'chart.js'; // <--- Charting
import { DashboardService } from '../../core/service/dashboard.service';
import { DashboardSummary } from '../../models/dashboard-summary.model';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'; // <-- Add this import
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule, // For FormControl
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule, 
    MatProgressSpinnerModule,
    BaseChartDirective // For charts
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  dashboardSummary: DashboardSummary | null = null;
  selectedDateControl = new FormControl(new Date()); // Initialize with today's date
  isLoading: boolean = false;
  errorMessage: string | null = null;

  // Chart Configuration for Weight Progress
  public lineChartData: ChartData<'line'> = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Weight (kg)',
        borderColor: 'rgba(77,189,116,1)', // Greenish color
        backgroundColor: 'rgba(77,189,116,0.2)',
        pointBackgroundColor: 'rgba(77,189,116,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(77,189,116,0.8)',
        fill: 'origin',
      }
    ]
  };

  public lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false, // Allows height to be set by CSS
    elements: {
      line: {
        tension: 0.3 // Smooth curves
      }
    },
    scales: {
      x: {
        type: 'category', // Explicitly set x-axis type
        title: {
          display: true,
          text: 'Date'
        }
      },
      y: {
        beginAtZero: false,
        title: {
          display: true,
          text: 'Weight (kg)'
        },
        // Optional: Ensure ticks are integers if weights are whole numbers
        // ticks: {
        //   precision: 0
        // }
      }
    },
    plugins: {
      legend: {
        display: true,
        position: 'top',
      },
      tooltip: {
        mode: 'index',
        intersect: false,
      }
    }
  };

  public lineChartType: ChartType = 'line';

  constructor(private dashboardService: DashboardService) { }

  ngOnInit(): void {
    // Fetch data for the initial selected date (today)
    this.fetchDashboardSummary(this.selectedDateControl.value);

    // Subscribe to date changes from the date picker
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

    // Format date to YYYY-MM-DD for the API call
    const formattedDate = format(date, 'yyyy-MM-dd'); // Using date-fns for formatting

    this.dashboardService.getDailySummary(formattedDate).subscribe({
      next: (data: DashboardSummary) => {
        this.dashboardSummary = data;
        this.isLoading = false;
        this.updateChartData(data.currentWeight); // Update chart with current weight
      },
      error: (err: any) => {
        console.error('Error fetching dashboard summary:', err);
        this.errorMessage = 'Failed to load dashboard data. Please try again.';
        this.dashboardSummary = null; // Clear previous data
        this.isLoading = false;
      }
    });
  }

  // Simulate updating chart data. In a real app, you'd fetch historical data.
  updateChartData(currentWeight: number): void {
    // For demonstration, we'll just add the current weight to a dummy history.
    // In a real application, you would fetch actual historical weight data
    // from your API and populate this chart.

    // Dummy historical data (replace with actual API data)
    const dummyDates = ['Jul 15', 'Jul 16', 'Jul 17', 'Jul 18', 'Jul 19', 'Jul 20']; // Sample dates
    const dummyWeights = [79.2, 79.0, 78.5, 78.9, 78.7, 78.8]; // Sample weights

    // Add current weight to dummy data if not already there for today
    const todayFormatted = format(this.selectedDateControl.value!, 'MMM dd');
    if (!dummyDates.includes(todayFormatted)) {
      dummyDates.push(todayFormatted);
      dummyWeights.push(currentWeight);
    } else {
      // Update if date already exists
      const index = dummyDates.indexOf(todayFormatted);
      dummyWeights[index] = currentWeight;
    }

    // Ensure dummy data is sorted by date if necessary
    // (for this simple demo, we assume chronological order)

    this.lineChartData.labels = dummyDates;
    this.lineChartData.datasets[0].data = dummyWeights;

    // Trigger chart update (optional, ng2-charts usually auto-updates)
    // this.chart?.update(); // If using @ViewChild for chart
  }

  // Placeholder methods for action buttons
  addMeal(): void {
    console.log('Add Meal button clicked!');
    // Implement navigation or dialog for adding a meal
  }

  addExercise(): void {
    console.log('Add Exercise button clicked!');
    // Implement navigation or dialog for adding an exercise
  }

  logWeight(): void {
    console.log('Log Weight button clicked!');
    // Implement navigation or dialog for logging weight
  }
}