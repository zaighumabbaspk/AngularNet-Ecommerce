import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environment/environment';
import { LoginRequest, SignupRequest, LoginResponse, User, AuthState } from '../Models/auth.model';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiBaseUrl}/Authentication`;
  
  private authState = new BehaviorSubject<AuthState>({
    isAuthenticated: false,
    user: null,
    token: null,
    refreshToken: null
  });

  public authState$ = this.authState.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadAuthState();
  }

  
  private loadAuthState(): void {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');

    if (token && this.isTokenValid(token)) {
      const user = this.getUserFromToken(token);
      this.authState.next({
        isAuthenticated: true,
        user,
        token,
        refreshToken
      });
    } else {
      this.clearAuthState();
    }
  }

  // Signup
  signup(request: SignupRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, request);
  }

  // Login
  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        if (response.success && response.token) {
          this.setAuthState(response.token, response.refreshToken || '');
        }
      })
    );
  }

  // Logout
  logout(): void {
    this.clearAuthState();
    this.router.navigate(['/login']);
  }

  // Refresh Token
  refreshToken(): Observable<LoginResponse> {
    const refreshToken = this.authState.value.refreshToken;
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    return this.http.get<LoginResponse>(`${this.apiUrl}/refreshToken/${refreshToken}`).pipe(
      tap(response => {
        if (response.success && response.token) {
          this.setAuthState(response.token, response.refreshToken || '');
        }
      })
    );
  }

  // Set auth state
  private setAuthState(token: string, refreshToken: string): void {
    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);

    const user = this.getUserFromToken(token);
    
    // Add small delay to ensure token is available in interceptor
    setTimeout(() => {
      this.authState.next({
        isAuthenticated: true,
        user,
        token,
        refreshToken
      });
    }, 100);

    // Transfer guest cart to authenticated user
    this.transferGuestCart();
  }

  // Transfer guest cart (will be called by CartService)
  private transferGuestCart(): void {
    // Import CartService dynamically to avoid circular dependency
    import('./cart.service').then(({ CartService }) => {
      // Get CartService instance from injector
      // This is a workaround for circular dependency
      setTimeout(() => {
        const cartService = (window as any).cartServiceInstance;
        if (cartService && cartService.transferGuestCartToUser) {
          cartService.transferGuestCartToUser();
        }
      }, 100);
    });
  }

  // Clear auth state
  private clearAuthState(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    
    this.authState.next({
      isAuthenticated: false,
      user: null,
      token: null,
      refreshToken: null
    });
  }

  // Get user from JWT token
  private getUserFromToken(token: string): User | null {
    try {
      const decoded: any = jwtDecode(token);
      return {
        userId: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || '',
        email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '',
        fullName: decoded['FullName'] || '',
        role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'User'
      };
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  // Check if token is valid
  private isTokenValid(token: string): boolean {
    try {
      const decoded: any = jwtDecode(token);
      const expirationDate = decoded.exp * 1000;
      return Date.now() < expirationDate;
    } catch {
      return false;
    }
  }

  // Get current token
  getToken(): string | null {
    return this.authState.value.token;
  }

  // Check if user is authenticated
  isAuthenticated(): boolean {
    return this.authState.value.isAuthenticated;
  }

  // Get current user
  getCurrentUser(): User | null {
    return this.authState.value.user;
  }

  // Check if user has specific role
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  // Check if user is admin
  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  // Forgot Password
  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, { email });
  }

  // Reset Password
  resetPassword(email: string, token: string, newPassword: string, confirmPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, {
      email,
      token,
      newPassword,
      confirmPassword
    });
  }

  // Verify Email
  verifyEmail(email: string, token: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-email?email=${encodeURIComponent(email)}&token=${encodeURIComponent(token)}`, {});
  }

  // Resend Verification Email
  resendVerificationEmail(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/resend-verification`, { email });
  }
}
