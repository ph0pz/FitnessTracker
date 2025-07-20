import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { SidebarComponent } from '../sidebar.component/sidebar.component';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../header.component/header.component';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
CommonModule,
    RouterOutlet,
    // Material Modules
    MatSidenavModule, // <--- Add MatSidenavModule
    MatButtonModule,  // Add MatButtonModule if not already here
    MatIconModule,    // Add MatIconModule if not already here
    // Child Components
    HeaderComponent,
    SidebarComponent,
   
  ],
  templateUrl: './layout.html',
  styleUrl: './layout.scss'
})
export class Layout implements AfterViewInit {
  // Get a reference to the MatSidenav component in the template
  @ViewChild('sidenav') sidenav!: MatSidenav; // <--- NEW

  constructor() { }

  ngAfterViewInit(): void {
    // You can perform actions here after the view is initialized,
    // e.g., for responsive logic if not fully handled by Material directly.
  }

  // Method to be called by the header to toggle the sidebar
  toggleSidenav(): void {
    this.sidenav.toggle();
  }
}
