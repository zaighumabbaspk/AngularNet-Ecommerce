import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../Core/Services/auth.service';
import { CustomNotificationService } from '../../../../Core/Services/custom-notification.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.css']
})
export class VerifyEmailComponent implements OnInit {
  isVerifying = true;
  isSuccess = false;
  message = '';
  email = '';
  token = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private notification: CustomNotificationService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
      this.token = params['token'] || '';

      if (this.email && this.token) {
        this.verifyEmail();
      } else {
        this.isVerifying = false;
        this.message = 'Invalid verification link.';
      }
    });
  }

  verifyEmail(): void {
    this.authService.verifyEmail(this.email, this.token).subscribe({
      next: (response) => {
        this.isVerifying = false;
        this.isSuccess = response.success;
        this.message = response.message;
        
        if (response.success) {
          this.notification.authSuccess(
            'Email verified successfully! Redirecting to login...',
            'Verification Complete'
          );
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 3000);
        } else {
          this.notification.authError(response.message || 'Verification failed');
        }
      },
      error: (error) => {
        this.isVerifying = false;
        this.isSuccess = false;
        const errorMsg = error.error?.message || 'Verification failed. Please try again.';
        this.message = errorMsg;
        this.notification.authError(errorMsg, 'Verification Failed');
      }
    });
  }

  resendVerification(): void {
    this.isVerifying = true;
    this.authService.resendVerificationEmail(this.email).subscribe({
      next: (response) => {
        this.isVerifying = false;
        this.message = response.message;
      },
      error: (error) => {
        this.isVerifying = false;
        this.message = error.error?.message || 'Failed to resend email.';
      }
    });
  }
}
