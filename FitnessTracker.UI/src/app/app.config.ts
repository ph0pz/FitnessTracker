// src/app/app.config.ts (UPDATED)
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
// CORRECTED IMPORT PATH for TokenInterceptor:
import { tokenInterceptor } from './core/auth/token-interceptor';
import { provideNativeDateAdapter } from '@angular/material/core';
import { Chart, CategoryScale, LinearScale, LineController, PointElement, LineElement, Legend, Tooltip, PieController, ArcElement } from 'chart.js';


Chart.register(
  CategoryScale,    // For X-axis with string labels (e.g., dates)
  LinearScale,      // For Y-axis with numerical values (e.g., weight)
  LineController,   // For line charts
  PointElement,     // For data points on the line
  LineElement,      // For the line segments
  PieController,    // For pie/doughnut charts
  ArcElement,       // For the arcs in pie/doughnut charts
  Legend,           // For chart legend (if used)
  Tooltip    
);
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        tokenInterceptor // <--- Use the function name, not the class name here
      ])
    ),
    provideNativeDateAdapter()

  ],

};