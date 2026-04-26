import { HttpClient } from '@angular/common/http';
import { inject, Injectable, NgZone, signal } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable, tap } from 'rxjs';
import Swal from 'sweetalert2';
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
  private router = inject(Router);
  private zone = inject(NgZone);

  currentUser = signal<UserState | null>(null);

  private logoutTimer: any;
  private refreshTimer: any;
  private isRefreshing = false;

  constructor() {
    this.initializeAuthState();
  }

  // ---------------- INIT ----------------
  private initializeAuthState() {
    const token = this.getToken();
    const storedUser = localStorage.getItem('user_data');

    if (token && storedUser) {
      this.currentUser.set(JSON.parse(storedUser));

      this.startTokenMonitor(token);
    }
  }

  // ---------------- LOGIN ----------------
  login(data: AuthDtos.LoginDto): Observable<AuthDtos.TokenDto> {
    return this.http
      .post<ApiResponse<AuthDtos.TokenDto>>(`${this.baseUrl}/Login`, data, {
        withCredentials: true,
      })
      .pipe(
        tap((response) => {
          if (response.success && response.data) {
            this.saveToken(response.data.accessToken);
            this.extractAndSaveClaims(response.data.accessToken);
            this.startTokenMonitor(response.data.accessToken);
          }
        }),
        map((res) => res.data),
      );
  }

  // ---------------- REGISTER ----------------
  register(data: AuthDtos.RegistrationDto): Observable<ApiResponse<any>> {
    return this.http
      .post<ApiResponse<any>>(`${this.baseUrl}/Register`, data)
      .pipe(map((res) => res.data));
  }

  // ---------------- TOKEN ----------------
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  // ---------------- LOGOUT ----------------
  logout(): void {
    if (this.logoutTimer) clearTimeout(this.logoutTimer);
    if (this.refreshTimer) clearTimeout(this.refreshTimer);

    localStorage.removeItem('token');
    localStorage.removeItem('user_data');

    this.currentUser.set(null);

    this.router.navigate(['/login'], { replaceUrl: true });
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  // ---------------- REFRESH API ----------------
  refreshToken() {
    return this.http
      .post<{ accessToken: string }>(`${this.baseUrl}/refresh-token`, {}, { withCredentials: true })
      .pipe(
        tap((response) => {
          this.saveToken(response.accessToken);
          this.startTokenMonitor(response.accessToken);
        }),
      );
  }

  // ---------------- PROACTIVE TOKEN MONITOR ----------------
  public startTokenMonitor(token: string) {
    const payload = JSON.parse(atob(token.split('.')[1]));

    const exp = payload.exp * 1000;
    const now = Date.now();

    const timeLeft = exp - now;

    // refresh قبل الانتهاء بـ 60 ثانية
    const refreshTime = timeLeft - 60_000;

    if (this.refreshTimer) clearTimeout(this.refreshTimer);
    if (this.logoutTimer) clearTimeout(this.logoutTimer);

    if (refreshTime > 0) {
      this.refreshTimer = setTimeout(() => {
        this.silentRefresh();
      }, refreshTime);
    } else {
      this.expireSession();
    }
  }

  // ---------------- SILENT REFRESH ----------------
  private silentRefresh() {
    if (this.isRefreshing) return;

    this.isRefreshing = true;

    this.refreshToken().subscribe({
      next: (res: any) => {
        this.isRefreshing = false;

        const newToken = res?.data ?? res?.accessToken;

        this.saveToken(newToken);
        this.startTokenMonitor(newToken);
      },

      error: () => {
        this.isRefreshing = false;
        this.expireSession();
      },
    });
  }

  // ---------------- CLAIMS ----------------
  public extractAndSaveClaims(token: string): void {
    const parts = token.split('.');

    if (parts.length !== 3) return;

    const payload = JSON.parse(atob(parts[1]));

    const userState: UserState = {
      userId:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
        payload.sub ||
        '',

      userEmail:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ||
        payload.email ||
        '',

      userName:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
        payload.unique_name ||
        payload.name ||
        '',
    };

    this.currentUser.set(userState);
    localStorage.setItem('user_data', JSON.stringify(userState));
  }

  // ---------------- EXPIRE ----------------
  public expireSession(): void {
    Swal.fire({
      title: 'Session Expired',
      text: 'Your security token has expired. Please login again.',
      icon: 'warning',
      confirmButtonText: 'Login Now',
      confirmButtonColor: '#3085d6',
      allowOutsideClick: false,
      allowEscapeKey: false,
    }).then(() => {
      this.logout();
    });
  }
}
