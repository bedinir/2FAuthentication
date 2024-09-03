import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { RegisterData } from '../shared/models/register-data';
import { ResponseData } from '../shared/models/response-data';
import { LoginData } from '../shared/models/login-data';
import { jwtDecode } from 'jwt-decode';
import { isPlatformBrowser } from '@angular/common';
import { ChangePasswordData } from '../shared/models/change-password-data';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = 'https://localhost:7124/api';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  register(data: RegisterData): Observable<ResponseData> {
    return this.http.post<ResponseData>(`${this.baseUrl}/Auth/register`, data);
  }

  login(data: LoginData): Observable<ResponseData> {
    return this.http.post<ResponseData>(`${this.baseUrl}/Auth/login`, data);
  }

  verify2FA(code: string, username: string): Observable<any> {
    const body = { code, username }; // Create the body object
    return this.http.post(`${this.baseUrl}/_2FA/verify-2fa`, body);
  }

  isAuthenticated(): boolean {
    if (!isPlatformBrowser(this.platformId)) {
      // If not running in the browser, return false or handle accordingly
      return false;
    }
    const token = localStorage.getItem('authToken');
    if (!token) {
      return false;
    }

    return !this.isTokenExpired(token);
  }

  logout(): void {
    localStorage.removeItem('authToken');
  }

  changePassword(data: ChangePasswordData) {
    const token = localStorage.getItem('authToken') || '';
    let headersToSend = new HttpHeaders();
    headersToSend = headersToSend
      // .set('Bearer', token)
      .set('Accept', 'application/json');
    return this.http.post(`${this.baseUrl}/Auth/change-password`, data, {
      headers: headersToSend,
    });
  }

  private isTokenExpired(token: string): boolean {
    if (!token) return true;
    const decoded = jwtDecode(token);
    const isTokenExpired = Date.now() >= decoded['exp']! * 1000;
    if (isTokenExpired) this.logout();
    return isTokenExpired;
  }

  getToken() {
    return localStorage.getItem('authToken');
  }

  private saveToken(token: string): void {
    localStorage.setItem('authToken', token);
  }

  getAuthHeaders() {
    const token = localStorage.getItem('authToken'); // or wherever you store your token
    console.log(token);

    if (!token) {
      console.error('Token not found, redirecting to login.');
      // Redirect to login page or handle unauthorized access
    }

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return { headers: headers };
  }
}
