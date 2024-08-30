import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { LoginData } from '../../shared/models/login-data';
import { CommonModule } from '@angular/common';
import { state } from '@angular/animations';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      Username: ['', Validators.required],
      Password: ['', Validators.required],
      rememberMe: [false], // Optional: default to false
    });
  }

  onSignIn() {
    if (this.loginForm.valid) {
      const user: LoginData = this.loginForm.value;
  
      this.authService.login(user).subscribe({
        next: (res) => {
          if (res.data.is2FAEnabled) {
            console.log('Navigating to verify-2fa');
            this.router.navigate(['verify-2fa'], { queryParams: { username: user.Username } }).then(success => {
              console.log('Navigation to verify-2fa successful:', success, user.Username);
            }).catch(error => {
              console.error('Navigation error to verify-2fa:', error);
            });
          } else {
            localStorage.setItem('authToken', res.data.Token);
            console.log('Token saved, navigating to home');
            this.router.navigate(['home']).then(success => {
              console.log('Navigation to home successful:', success);
            }).catch(error => {
              console.error('Navigation error to home:', error);
            });
          }
        },
        error: (error) => {
          console.error('Login failed:', error);
        }
      });
    }
  }
  
}
