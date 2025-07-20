import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';

// Angular Material Imports
import { MatListModule } from '@angular/material/list'; // For <mat-list> and <mat-list-item>
import { MatIconModule } from '@angular/material/icon'; // For <mat-icon>

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    MatListModule, // <--- Add this
    MatIconModule  // <--- Add this
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent { }