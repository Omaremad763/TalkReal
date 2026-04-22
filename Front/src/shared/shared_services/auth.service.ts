import { HttpClient } from '@angular/common/http';
import { inject, Injectable, NgZone, signal } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable, tap } from 'rxjs';
import { environment } from '../../environment';
import { ApiResponse } from '../../shared/shared_models/api-response.model';
import * as AuthDtos from '../shared_models/Auth-models';
export interface UserState {
  userId: string;
  userEmail: string;
  userName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Auth`;
  private route = inject(Router);
  private zone = inject(NgZone);

  currentUser = signal<UserState | null>(null);

  constructor() {
    this.initializeAuthState();
  }

  private initializeAuthState() {
    const token = this.getToken();
    const storedUser = localStorage.getItem('user_data');
    if (token && storedUser) {
      this.currentUser.set(JSON.parse(storedUser));
    }
  }
  login(data: AuthDtos.LoginDto): Observable<string> {
    return this.http.post<ApiResponse<string>>(`${this.baseUrl}/Login`, data).pipe(
      tap((response) => {
        if (response.success && response.data) {
          this.saveToken(response.data);
          this.extractAndSaveClaims(response.data);
        }
      }),
      map((res) => res.data),
    );
  }

  register(data: AuthDtos.RegistrationDto): Observable<ApiResponse<any>> {
    return this.http
      .post<ApiResponse<any>>(`${this.baseUrl}/Register`, data)
      .pipe(map((res) => res.data));
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  public extractAndSaveClaims(token: string): void {
    if (!token || !token.includes('.')) {
      console.error('Invalid token format');
      this.logout();
      return;
    }
    try {
      const payloadBase64 = token.split('.')[1];
      const payloadJson = window.atob(payloadBase64);
      const payload = JSON.parse(decodeURIComponent(escape(payloadJson)));
      const idClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
      const emailClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
      const userNameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

      const userState: UserState = {
        userId: payload[idClaim],
        userEmail: payload[emailClaim],
        userName: payload[userNameClaim],
      };
      this.currentUser.set(userState);
      localStorage.setItem('user_data', JSON.stringify(userState));
    } catch (error) {
      console.error('Error decoding token:', error);
      this.logout();
    }
  }
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user_data');
    this.currentUser.set(null);
    this.zone.run(() => this.route.navigate(['/login']));
  }
  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
