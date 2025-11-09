import { Routes } from '@angular/router';
import { HeroComponent } from './Shared/Componets/hero/hero.component';
import { ProductComponent } from './Shared/Componets/product/product.component';
import { ProductDetailComponent } from './Shared/Componets/product-detail/product-detail.component';
import { TestimonialsComponent } from './Shared/Componets/testimonials/testimonials.component';


export const routes: Routes = [
  {
    path: '',
    component: HeroComponent, 
    pathMatch: 'full'
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
