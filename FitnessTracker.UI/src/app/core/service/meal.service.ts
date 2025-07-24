// src/app/core/services/dashboard.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Meal, SavedItem } from '../../models/meal.model';
import { environment } from '../../../environments/environment.dev';

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getSavedMeals(): Observable<SavedItem[]> {
    return this.http.get<SavedItem[]>(`${this.baseUrl}/Meals/saved`);
  }

  deleteSavedMeal(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Meals/saved/${id}`);
  }

  getMealsByDate(date: string): Observable<Meal[]> {
    return this.http.get<Meal[]>(`${this.baseUrl}/Meals/${date}`);
  }

  addMeal(meal: Meal): Observable<Meal> {
    return this.http.post<Meal>(`${this.baseUrl}/Meals`, meal);
  }

  deleteMeal(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Meals/${id}`);
  }

  addSavedMeal(item: SavedItem): Observable<SavedItem> {
    return this.http.post<SavedItem>(`${this.baseUrl}/Meals/saved`, item);
  }
    fetchMacroSuggestion(item: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/gptMacro/analyze-macros`, item);
  }
}