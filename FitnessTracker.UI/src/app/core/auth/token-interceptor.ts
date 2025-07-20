// src/app/core/auth/token.interceptor.ts (UPDATED)
// Remove: import { Injectable } from '@angular/core';
import {
  // Remove: HttpRequest,
  // Remove: HttpHandler,
  // Remove: HttpEvent,
  // Remove: HttpInterceptor
  HttpInterceptorFn // <--- Import HttpInterceptorFn
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { inject } from '@angular/core'; // <--- Import 'inject'

// Convert to a function-based interceptor for standalone components
export const tokenInterceptor: HttpInterceptorFn = (req, next) => { // <--- Use HttpInterceptorFn and define as a const
  const authService = inject(AuthService); // <--- Use inject() to get the service

  const token = authService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};