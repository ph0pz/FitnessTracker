// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth-guard';
import { publicAuthGuard } from './core/auth/public-auth.guard';
import { LoginComponent } from './feature/auth/login/login.component/login.component';
import { RegisterComponent } from './feature/auth/register/register.component/register.component';
import { Layout } from './layout/layout/layout';
import { DashboardComponent } from './feature/dashboard/dashboard.component';
import { MealComponent } from './feature/meal/meal.component';
import { ProgressTrackingComponent } from './feature/progress/progress.component';


export const routes: Routes = [
  // Redirection for root path based on authentication status
  {
    path: '',
    canActivate: [publicAuthGuard],
    children: [] // This route is just for guard redirection, no component directly rendered here
  },

  // Public authentication routes (login/register)
  { path: 'auth/login', component: LoginComponent, canActivate: [publicAuthGuard] },
  { path: 'auth/register', component: RegisterComponent, canActivate: [publicAuthGuard] },
  { path: 'auth', redirectTo: 'auth/login', pathMatch: 'full' }, // Redirect /auth to /auth/login

  // --- PROTECTED ROUTES WITH LAYOUT ---
  // This is the ONLY place where LayoutComponent should be directly used as a 'component'
  {
    path: '', // This means protected routes will be like /dashboard, /profile, etc.
    component: DashboardComponent, // <--- LayoutComponent is rendered ONCE for this parent route
    canActivate: [authGuard], // Protects all children of this route
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./feature/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
    
    ]
  },
    {
    path: '', // This means protected routes will be like /dashboard, /profile, etc.
    component: MealComponent, // <--- LayoutComponent is rendered ONCE for this parent route
    canActivate: [authGuard], // Protects all children of this route
    children: [
      {
        path: 'meals',
        loadComponent: () => import('./feature/meal/meal.component').then(m => m.MealComponent)
      },
    
    ]
  },
   {
    path: '', // This means protected routes will be like /dashboard, /profile, etc.
    component: ProgressTrackingComponent, // <--- LayoutComponent is rendered ONCE for this parent route
    canActivate: [authGuard], // Protects all children of this route
    children: [
      {
        path: 'progress',
        loadComponent: () => import('./feature/progress/progress.component').then(m => m.ProgressTrackingComponent)
      },
    
    ]
  },



  // Wildcard route for any unhandled paths (redirects based on auth status)
  {
    path: '**',
    canActivate: [publicAuthGuard],
    children: [] // This route is just for guard redirection
  }
];