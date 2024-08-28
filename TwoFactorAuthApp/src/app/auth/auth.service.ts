import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterData } from '../shared/models/register-data';
import { ResponseData } from '../shared/models/response-data';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7193/api/auth/'

  constructor(private http:HttpClient) { }

  register(data: RegisterData): Observable<ResponseData> {
    return this.http.post<ResponseData>(this.baseUrl + 'register', data);
  }
}
