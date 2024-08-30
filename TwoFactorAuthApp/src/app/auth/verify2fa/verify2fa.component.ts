import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
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
  username: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder
  ) {
    this.verifyForm = this.fb.group({
      token: [
        '',
        [Validators.required, Validators.minLength(6), Validators.maxLength(6)],
      ]
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.username = params['username'];
      console.log('Retrieved username:', this.username); // Debugging line
    });
  }

  onVerify() {
    if (this.verifyForm.valid && this.username) {
      const code = this.verifyForm.get('token')?.value;

      this.authService.verify2FA(code, this.username).subscribe({
        next: (response) => {
          if (response.token) {
            localStorage.setItem('authToken', response.token); // Store the token
            this.router.navigate(['home']); // Navigate to the home page
          } else {
            console.error('Verification failed: ', response.message);
          }
        },
        error: (err) => {
          console.error('Error verifying 2FA: ', err);
        },
        complete: () => {
          console.log('2FA verification request completed');
        }
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
