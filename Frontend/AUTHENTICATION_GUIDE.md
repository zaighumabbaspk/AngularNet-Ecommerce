# Frontend Authentication Implementation Guide

## ✅ What I've Created For You:

### 1. **Models** (`Frontend/src/app/Core/Models/auth.model.ts`)
- LoginRequest, SignupRequest interfaces
- LoginResponse, User, AuthState interfaces
- Complete TypeScript types for authentication

### 2. **Auth Service** (`Frontend/src/app/Core/Services/auth.service.ts`)
- Complete authentication service with:
  - Login, Signup, Logout methods
  - Token management (JWT)
  - Refresh token functionality
  - Role-based access (isAdmin, hasRole)
  - User state management with RxJS
  - LocalStorage integration

### 3. **Auth Guard** (`Frontend/src/app/Core/Guards/auth.guard.ts`)
- `authGuard` - Protects routes requiring authentication
- `adminGuard` - Protects routes requiring admin role
- Automatic redirect to login with return URL

### 4. **HTTP Interceptor** (`Frontend/src/app/Core/Interceptors/auth.interceptor.ts`)
- Automatically adds JWT token to all HTTP requests
- Handles 401 errors and refreshes token automatically
- Retries failed requests after token refresh

### 5. **Login Component** (Complete)
- `Frontend/src/app/Shared/Componets/auth/login/login.component.ts`
- `Frontend/src/app/Shared/Componets/auth/login/login.component.html`
- `Frontend/src/app/Shared/Componets/auth/login/login.component.css`
- Beautiful UI with form validation
- Password visibility toggle
- Loading states and error handling

---

## 📝 What You Need To Create:

### 1. **Signup Component**

Create these files:
- `Frontend/src/app/Shared/Componets/auth/signup/signup.component.ts`
- `Frontend/src/app/Shared/Componets/auth/signup/signup.component.html`
- `Frontend/src/app/Shared/Componets/auth/signup/signup.component.css`

**signup.component.ts:**
```typescript
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../Core/Services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent {
  signupForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.signupForm = this.fb.group({
      fullname: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  togglePasswordVisibility(field: 'password' | 'confirmPassword'): void {
    if (field === 'password') {
      this.showPassword = !this.showPassword;
    } else {
      this.showConfirmPassword = !this.showConfirmPassword;
    }
  }

  onSubmit(): void {
    if (this.signupForm.invalid) {
      this.signupForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.signup(this.signupForm.value).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = 'Account created successfully! Redirecting to login...';
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else {
          this.errorMessage = response.message || 'Signup failed';
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'An error occurred during signup';
        this.isLoading = false;
      }
    });
  }

  get fullname() { return this.signupForm.get('fullname'); }
  get email() { return this.signupForm.get('email'); }
  get password() { return this.signupForm.get('password'); }
  get confirmPassword() { return this.signupForm.get('confirmPassword'); }
}
```

**signup.component.html:** (Similar to login, add fullname and confirmPassword fields)

**signup.component.css:** (Copy from login.component.css)

---

### 2. **Update app.config.ts**

Add HTTP interceptor and providers:

```typescript
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { authInterceptor } from './Core/Interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor])
    )
  ]
};
```

---

### 3. **Update app.routes.ts**

Add authentication routes and guards:

```typescript
import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './Core/Guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./Shared/Componets/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./Shared/Componets/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'signup',
    loadComponent: () => import('./Shared/Componets/auth/signup/signup.component').then(m => m.SignupComponent)
  },
  {
    path: 'products',
    loadComponent: () => import('./Shared/Componets/product/product.component').then(m => m.ProductComponent)
  },
  {
    path: 'product-detail/:id',
    loadComponent: () => import('./Shared/Componets/product-detail/product-detail.component').then(m => m.ProductDetailComponent)
  },
  // Protected routes (require authentication)
  {
    path: 'cart',
    canActivate: [authGuard],
    loadComponent: () => import('./Shared/Componets/cart/cart.component').then(m => m.CartComponent)
  },
  // Admin only routes
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./Shared/Componets/admin/admin.component').then(m => m.AdminComponent)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
```

---

### 4. **Install jwt-decode Package**

Run this command:
```bash
npm install jwt-decode
```

---

### 5. **Update Header Component**

Add login/logout buttons and user info:

```typescript
// In header.component.ts
import { AuthService } from '../../../Core/Services/auth.service';

export class HeaderComponent {
  constructor(public authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}
```

```html
<!-- In header.component.html -->
<div class="auth-section" *ngIf="(authService.authState$ | async) as authState">
  <ng-container *ngIf="authState.isAuthenticated; else notLoggedIn">
    <span class="user-name">{{ authState.user?.fullName }}</span>
    <button class="btn-logout" (click)="logout()">
      <i class="fa-solid fa-right-from-bracket"></i>
      Logout
    </button>
  </ng-container>
  
  <ng-template #notLoggedIn>
    <a routerLink="/login" class="btn-login">Login</a>
    <a routerLink="/signup" class="btn-signup">Sign Up</a>
  </ng-template>
</div>
```

---

### 6. **Update Product Component (Admin Features)**

Show admin buttons only for admin users:

```html
<!-- In product.component.html -->
<button 
  *ngIf="authService.isAdmin()" 
  class="add-btn" 
  (click)="openAddForm()">
  <i class="fa-solid fa-plus"></i> Add New Product
</button>

<div class="admin-buttons" *ngIf="authService.isAdmin()">
  <button class="admin-actions" (click)="openEditForm(p)">
    <i class="fa-regular fa-pen-to-square"></i>
  </button>
  <button class="admin-actions" (click)="deleteProduct(p.id)">
    <i class="fa-regular fa-trash-can"></i>
  </button>
</div>
```

```typescript
// In product.component.ts
constructor(
  private productService: ProductService,
  private categoryService: CategoryService,
  private fb: FormBuilder,
  public authService: AuthService  // Add this
) { }
```

---

## 🔧 Backend Changes (If Needed):

Your backend authentication is already working! But if you need to add role-based endpoints:

### Add [Authorize] Attributes:

```csharp
// In ProductController.cs
[HttpPost("add")]
[Authorize(Roles = "Admin")]  // Only admin can add products
public async Task<IActionResult> Add(CreateProduct product)
{
    // ...
}

[HttpPut("update")]
[Authorize(Roles = "Admin")]  // Only admin can update
public async Task<IActionResult> Update(UpdateProduct product)
{
    // ...
}

[HttpDelete("delete/{id}")]
[Authorize(Roles = "Admin")]  // Only admin can delete
public async Task<IActionResult> Delete(Guid id)
{
    // ...
}

[HttpGet("all")]
[AllowAnonymous]  // Everyone can view products
public async Task<IActionResult> GetAll()
{
    // ...
}
```

---

## 🚀 Testing Your Authentication:

1. **Create an account** at `/signup`
2. **Login** at `/login`
3. **Check token** in browser DevTools → Application → LocalStorage
4. **Test protected routes** - try accessing cart without login
5. **Test admin features** - first user is admin, others are users
6. **Test token refresh** - wait for token to expire and see auto-refresh

---

## 📱 Features Included:

✅ JWT Token Authentication
✅ Refresh Token Support
✅ Role-Based Access Control (Admin/User)
✅ Protected Routes with Guards
✅ Automatic Token Injection (Interceptor)
✅ Auto Token Refresh on 401
✅ Beautiful Login/Signup UI
✅ Form Validation
✅ Error Handling
✅ Loading States
✅ Password Visibility Toggle
✅ LocalStorage Persistence
✅ Responsive Design

---

## 🎯 Next Steps:

1. Create the Signup component (copy login and modify)
2. Update app.config.ts with interceptor
3. Update app.routes.ts with guards
4. Install jwt-decode package
5. Update header component with auth buttons
6. Update product component to show admin features
7. Test everything!

Let me know when you're ready to implement any of these steps and I can help!
