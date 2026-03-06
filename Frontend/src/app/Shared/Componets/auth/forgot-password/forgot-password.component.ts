import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../Core/Services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  forgotPasswordForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotPasswordForm.invalid) {
      this.forgotPasswordForm.markAllAsTouched();
      this.toastr.warning('Please enter a valid email address', 'Validation Error', {
        timeOut: 3000,
        progressBar: true
      });
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const email = this.forgotPasswordForm.value.email;

    this.authService.forgotPassword(email).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.successMessage = response.message || 'Password reset link has been sent to your email.';
          this.toastr.success(
            'Password reset link sent! Check your email inbox.',
            'Email Sent',
            {
              timeOut: 4000,
              progressBar: true,
              progressAnimation: 'increasing'
            }
          );
          this.forgotPasswordForm.reset();
        } else {
          this.errorMessage = response.message || 'Failed to send reset link.';
          this.toastr.error(this.errorMessage, 'Error', {
            timeOut: 4000,
            progressBar: true
          });
        }
      },
      error: (error) => {
        this.isLoading = false;
        const errorMsg = error.error?.message || 'An error occurred. Please try again.';
        this.errorMessage = errorMsg;
        this.toastr.error(errorMsg, 'Failed', {
          timeOut: 4000,
          progressBar: true
        });
      }
    });
  }

  get email() {
    return this.forgotPasswordForm.get('email');
  }
}
