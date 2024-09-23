import { Component, inject, OnInit } from '@angular/core';
import { HomeComponent } from "../home.component";
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Verify2faComponent } from "../../auth/verify2fa/verify2fa.component";
import { AuthService } from '../../auth/auth.service';
import { ChangePasswordData } from '../../shared/models/change-password-data';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [HomeComponent, RouterModule, ReactiveFormsModule, Verify2faComponent,CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  changePasswordForm: FormGroup;
  twoFactorAuthForm: FormGroup;
  is2FAEnabled: boolean = true;
  showVerifyComponent: boolean = false;
  showEnableModal: boolean = false;
  newPasswordVisible: boolean = false;
  repeatPasswordVisible: boolean = false;


  constructor(private fb: FormBuilder,
    private authService: AuthService
  ) { 
    this.changePasswordForm = this.fb.group({
      CurrentPassword : [''],
      NewPassword: [''],
      RepeatNewPassword: ['']
    });

    this.twoFactorAuthForm = this.fb.group({
      enable2FA: [this.is2FAEnabled]
    });

  }


  ngOnInit(): void {
    this.get2FAStatus();
  }

  onChangePassword() {
    console.log('onChangePassword called');
    if (this.changePasswordForm.valid) {
        const data: ChangePasswordData = this.changePasswordForm.value;
        this.authService.changePassword(data).subscribe({
            next: (res) => {
                console.log('Password changed successfully', res);
                // Maybe show a success message to the user here
            },
            error: (error) => {
                console.error('Error changing password', error);
                if (error.status === 401) {
                    // Handle unauthorized (maybe redirect to login)
                    console.log('Unauthorized - Please log in again.');
                } else if (error.status === 403) {
                    // Handle forbidden (access denied)
                    console.log('Access Denied');
                } else {
                    // Handle other errors
                    console.log('An error occurred', error.message);
                }
            }
        });
    }
}


  get2FAStatus() {
    // Fetch the 2FA status from the backend
    // this.authService.get2FAStatus().subscribe((status: boolean) => {
    //   this.is2FAEnabled = status;
    //   this.twoFactorAuthForm.get('enable2FA')?.setValue(true);
    // });
    this.twoFactorAuthForm.get('enable2FA')?.setValue(true);

  }

  onCheckboxChange(event: any) {
    if (!this.is2FAEnabled && event.target.checked) {
      // Show modal to enable 2FA if it's currently disabled
      this.showEnableModal = true;
    }
  }

  onToggle2FA() {
    if (!this.is2FAEnabled) {
      // If enabling 2FA, show the verification component
      this.showVerifyComponent = true;
      this.showEnableModal = false;
    } else {
      // Disable 2FA
      this.disable2FA();
    }
  }

  disable2FA() {
    // Logic to disable 2FA via backend call
    // this.authService.disable2FA().subscribe(() => {
    //   this.is2FAEnabled = false;
    //   this.showVerifyComponent = false;
    // });
  }

  onVerify() {
    // Logic for verifying the 2FA code
    // this.authService.verify2FA(this.twoFactorAuthForm.value.token,username).subscribe(() => {
    //   this.is2FAEnabled = true;
    //   this.showVerifyComponent = false;
    // });
  }

  closeModal() {
    this.showEnableModal = false;
  }
}
