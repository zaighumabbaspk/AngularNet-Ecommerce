import { Routes } from '@angular/router';
import { ProductComponent } from './Shared/Componets/product/product.component';
import { ProductDetailComponent } from './Shared/Componets/product-detail/product-detail.component';
import { HomeComponent } from './Shared/Componets/home/home.component';
import { LoginComponent } from './Shared/Componets/auth/login/login.component';
import { SignupComponent } from './Shared/Componets/auth/signup/signup.component';
import { ResetPasswordComponent } from './Shared/Componets/auth/reset-password/reset-password.component';
import { ForgotPasswordComponent } from './Shared/Componets/auth/forgot-password/forgot-password.component';
import { VerifyEmailComponent } from './Shared/Componets/auth/verify-email/verify-email.component';
import { CartComponent } from './Shared/Componets/cart/cart.component';
import { authGuard } from './Core/Guards/auth.guard';

export const routes: Routes = [
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
    path: 'verify-email',
    component: VerifyEmailComponent
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
    path: 'cart',
    component: CartComponent,
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
