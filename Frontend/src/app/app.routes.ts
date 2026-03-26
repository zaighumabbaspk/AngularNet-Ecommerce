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
import { StripeCheckoutSimpleComponent } from './Shared/Componets/stripe-checkout/stripe-checkout-simple.component';
import { CheckoutSuccessComponent } from './Shared/Componets/checkout-success/checkout-success.component';
import { CheckoutCancelComponent } from './Shared/Componets/checkout-cancel/checkout-cancel.component';
import { OrdersComponent } from './Shared/Componets/orders/orders.component';
import { OrderDetailsComponent } from './Shared/Componets/order-details/order-details.component';
import { AboutUsComponent } from './Shared/Componets/about-us/about-us.component';
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
    path: 'checkout',
    component: StripeCheckoutSimpleComponent,
    canActivate: [authGuard]
  },
  {
    path: 'checkout/success',
    component: CheckoutSuccessComponent,
    canActivate: [authGuard]
  },
  {
    path: 'checkout/cancel',
    component: CheckoutCancelComponent,
    canActivate: [authGuard]
  },
  {
    path: 'orders',
    component: OrdersComponent,
    canActivate: [authGuard]
  },
  {
    path: 'orders/:id',
    component: OrderDetailsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'about',
    component: AboutUsComponent
  },
  {
    path: '**',
    redirectTo: ''
  }
];
