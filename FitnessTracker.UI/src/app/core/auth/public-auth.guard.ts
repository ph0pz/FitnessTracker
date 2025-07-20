// src/app/core/auth/public-auth.guard.ts
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { map } from 'rxjs/operators';

export const publicAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isAuthenticated$.pipe(
    map(isAuthenticated => {
      if (isAuthenticated) {
        // If authenticated, redirect to dashboard
        return router.createUrlTree(['/dashboard']);
      } else {
        // If not authenticated, allow access to the public route
        return true;
      }
    })
  );
};