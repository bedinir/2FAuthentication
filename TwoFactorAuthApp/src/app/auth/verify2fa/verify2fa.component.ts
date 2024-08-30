import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router, RouterModule } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-verify2fa',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, CommonModule],
  templateUrl: './verify2fa.component.html',
  styleUrl: './verify2fa.component.css',
})
export class Verify2faComponent {
  verifyForm: FormGroup;

  constructor(
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.verifyForm = this.fb.group({
      token: [
        '',
        [Validators.required, Validators.minLength(6), Validators.maxLength(6)],
      ],
    });
  }

  onVerify() {
    if (this.verifyForm.valid) {
      const token = this.verifyForm.get('token')?.value;

      this.authService.verify2FA(token).subscribe({
        next: (response) => {
          if (response.success) {
            localStorage.setItem('authToken', response.token); // Store the token
            this.router.navigate(['']); // Navigate to the home page
          } else {
            console.error('Verification failed: ', response.message);
          }
        },
        error: (err) => {
          console.error('Error verifying 2FA: ', err);
        },
        complete: () => {
          console.log('2FA verification request completed');
        },
      });
    }
  }

  resendCode() {
    // this.authService.resend2FACode().subscribe(response => {
    //   console.log('Code resent', response);
    // }, error => {
    //   console.error('Error resending code', error);
    // });
  }
}
