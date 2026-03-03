import { Routes } from '@angular/router';
import { HeroComponent } from './Shared/Componets/hero/hero.component';
import { ProductComponent } from './Shared/Componets/product/product.component';
import { ProductDetailComponent } from './Shared/Componets/product-detail/product-detail.component';
import { TestimonialsComponent } from './Shared/Componets/testimonials/testimonials.component';
import { HomeComponent } from './Shared/Componets/home/home.component';
import { LoginComponent } from './Shared/Componets/auth/login/login.component';
import { SignupComponent } from './Shared/Componets/auth/signup/signup.component';
import { authGuard, adminGuard } from './Core/Guards/auth.guard';
import { ResetPasswordComponent } from './Shared/Componets/auth/reset-password/reset-password.component';
import { ForgotPasswordComponent } from './Shared/Componets/auth/forgot-password/forgot-password.component';

export const routes: Routes = [
  // Public routes (no authentication required)
  {
    path: '',
    component: HomeComponent, 
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'signup',
    component: SignupComponent
  },
  {
    path: 'forgot-password',
    component: ForgotPasswordComponent
  },
  {
    path: 'reset-password',
    component: ResetPasswordComponent
  },
  {
    path: 'products',
    component: ProductComponent
  },
  {
    path: 'product-detail/:id',
    component: ProductDetailComponent
  },
  
  {
    path: '**',
    redirectTo: ''
  }
];
