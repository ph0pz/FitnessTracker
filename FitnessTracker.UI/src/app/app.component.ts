// src/app/app.component.ts (formerly app.ts - UPDATED)
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common'; // Important: Add CommonModule for basic directives if needed in HTML
import { AuthService } from './core/auth/auth.service'; // Import AuthService
import { Observable } from 'rxjs';
import { Layout } from './layout/layout/layout';
// import { LayoutComponent } from './layout/layout/layout.component'; 

@Component({
  selector: 'app-root',
  standalone: true, // <--- Add this line to make it a standalone component
  imports: [
    RouterOutlet,
    CommonModule, // <--- Add this for *ngIf, *ngFor etc.
    Layout // <--- Import your LayoutComponent here
  ],
  templateUrl: './app.component.html', // <--- Update filename
  styleUrl: './app.component.scss' // <--- Update filename
})
export class AppComponent {
  protected title = 'fitness-tracker-ui';
  isAuthenticated$: Observable<boolean>; // <--- Add this property

  constructor(private authService: AuthService) { // <--- Inject AuthService
    this.isAuthenticated$ = this.authService.isAuthenticated$;
  }
}