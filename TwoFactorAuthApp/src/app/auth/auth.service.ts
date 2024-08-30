import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterData } from '../shared/models/register-data';
import { ResponseData } from '../shared/models/response-data';
import { LoginData } from '../shared/models/login-data';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = 'https://localhost:7124/api';

  constructor(private http: HttpClient) {}

  register(data: RegisterData): Observable<ResponseData> {
    return this.http.post<ResponseData>(`${this.baseUrl}/Auth/register`, data);
  }

  login(data: LoginData): Observable<ResponseData> {
    return this.http.post<ResponseData>(`${this.baseUrl}/Auth/login`, data);
  }

  verify2FA(code: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/_2FA/verify-2fa`, { Code: code });
  }  

  isAuthenticated(): boolean {
    const token = localStorage.getItem('authToken');
    console.log(token)
    if (!token) {
      return false;
    }

    return !this.isTokenExpired(token);
  }

  logout(): void {
    localStorage.removeItem('authToken');
  }

  private isTokenExpired(token: string): boolean {
    const payload = this.decodeToken(token);
    if (!payload || !payload.exp) {
      return true;
    }

    const expiryTimeInMilliseconds = payload.exp * 1000;
    return Date.now() > expiryTimeInMilliseconds;
  }

  private decodeToken(token: string): any {
    try {
      const payloadBase64 = token.split('.')[1];
      return JSON.parse(atob(payloadBase64));
    } catch (e) {
      return null; 
    }
  }

  private getToken(): string | null {
    return localStorage.getItem('authToken');
  }
  
  private saveToken(token: string): void {
    localStorage.setItem('authToken', token);
  }

  private getAuthHeaders(): { headers: HttpHeaders } {
    const token = this.getToken();
    return {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${token}`,
      }),
    };
  }
}
