import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { LoginData } from '../../shared/models/login-data';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterModule,ReactiveFormsModule,CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ){
    this.loginForm = this.fb.group({
      Username: ['', Validators.required],
      Password: ['', Validators.required],
      rememberMe: [false] // Optional: default to false
    });
  }

  onSignIn() {
    console.log(this.loginForm)
    if (this.loginForm.valid) {
      const user: LoginData = this.loginForm.value;
      this.authService.login(user).subscribe({
        next: (res) => {
          console.log(res);
          if (res.data.is2FAEnabled) {
            this.router.navigate(['/verify-2fa'], { state: { Username: user.Username } });
          } else {
            localStorage.setItem('authToken', res.data.Token);
          }
        },
        error: (error) => {
          console.log('Login failed', error);
          // Optionally, show a user-friendly error message
        },
        complete: () => {
          // Navigate to the home page after login completes
          this.router.navigate(['home']);
        }
      });
    }
  }

}
