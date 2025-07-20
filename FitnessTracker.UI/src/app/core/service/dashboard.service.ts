// src/app/core/services/dashboard.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardSummary } from '../../models/dashboard-summary.model';
import { environment } from '../../../environments/environment.dev';


@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = environment.apiUrl + '/Dashboard'; // Assuming base API URL in environment

  constructor(private http: HttpClient) { }

  getDailySummary(date: string): Observable<DashboardSummary> {
    let params = new HttpParams().set('date', date); // date should be in YYYY-MM-DD format
    return this.http.get<DashboardSummary>(`${this.apiUrl}/summary`, { params });
  }

  // You might add other methods here later for historical data if needed for the chart
}