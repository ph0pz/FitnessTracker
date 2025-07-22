// src/app/core/auth/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { LoginRequest, RegisterRequest, TokenResponse, ChangePasswordRequest } from '../../models/auth.models'; 
import { environment } from '../../../environments/environment.dev';

@Injectable({
  providedIn: 'root' 
})
export class AuthService {
  private apiUrl = environment.apiUrl + '/Auth'; 
  private tokenKey = 'jwt_token';
  private userIdKey = 'user_id';

  private _isAuthenticated = new BehaviorSubject<boolean>(this.hasToken());
  isAuthenticated$ = this._isAuthenticated.asObservable();

  constructor(private http: HttpClient) { }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  login(request: LoginRequest): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        localStorage.setItem(this.tokenKey, response.token);
        localStorage.setItem(this.userIdKey, response.userId.toString());
        this._isAuthenticated.next(true);
      })
    );
  }

  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/change-password`, request);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userIdKey);
    this._isAuthenticated.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getUserId(): number | null {
    const userId = localStorage.getItem(this.userIdKey);
    return userId ? parseInt(userId, 10) : null;
  }
}