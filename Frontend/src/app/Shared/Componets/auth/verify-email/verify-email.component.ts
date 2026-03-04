import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../Core/Services/auth.service';

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
    private authService: AuthService
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
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 3000);
        }
      },
      error: (error) => {
        this.isVerifying = false;
        this.isSuccess = false;
        this.message = error.error?.message || 'Verification failed. Please try again.';
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
