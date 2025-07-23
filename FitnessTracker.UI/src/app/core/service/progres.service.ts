// src/app/core/services/dashboard.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardSummary } from '../../models/dashboard.model';
import { environment } from '../../../environments/environment.dev';
import { WeightLog } from '../../models/progress.model';


@Injectable({
  providedIn: 'root'
})
export class ProgressService {
  private apiUrl = environment.apiUrl + '/progress'; // Assuming base API URL in environment

  constructor(private http: HttpClient) { }

  getWeightHistory(): Observable<WeightLog[]> {

    return this.http.get<WeightLog[]>(`${this.apiUrl}/weight-history`);
  }

    logWeight(meal: WeightLog): Observable<WeightLog> {
      return this.http.post<WeightLog>(`${this.apiUrl}/weight`, meal);
    }
}