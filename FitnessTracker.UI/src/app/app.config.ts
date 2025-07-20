// src/app/app.config.ts (UPDATED)
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
// CORRECTED IMPORT PATH for TokenInterceptor:
import { tokenInterceptor } from './core/auth/token-interceptor';



export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        tokenInterceptor // <--- Use the function name, not the class name here
      ])
    ),

  ]
};