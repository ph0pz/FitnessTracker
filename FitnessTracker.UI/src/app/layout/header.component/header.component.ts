import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service'; // Adjust path based on your project structure

// Angular Material Imports
import { MatToolbarModule } from '@angular/material/toolbar'; // For <mat-toolbar>
import { MatButtonModule } from '@angular/material/button';   // For <button mat-button>
import { MatIconModule } from '@angular/material/icon';     // For <mat-icon>

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatToolbarModule, // <--- Add this
    MatButtonModule,  // <--- Add this
    MatIconModule     // <--- Add this
  ],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit { // Add OnInit if you use it

  isAuthenticated$: Observable<boolean>;

  // Output event to notify parent (LayoutComponent) to toggle the sidenav
  @Output() sidenavToggle = new EventEmitter<void>(); // <--- NEW

  constructor(private authService: AuthService, private router: Router) {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
  }

  ngOnInit(): void {
    // Initialization logic if any
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  // Method to emit the toggle event
  onToggleSidenav(): void { // <--- NEW
    this.sidenavToggle.emit();
  }
}