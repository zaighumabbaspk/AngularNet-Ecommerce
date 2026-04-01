import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeroComponent } from '../hero/hero.component';
import { TestimonialsComponent } from '../testimonials/testimonials.component';
import { ProductComponent } from '../product/product.component';
import { RecentlyViewedComponent } from '../recently-viewed/recently-viewed.component';
import { AuthService } from '../../../Core/Services/auth.service';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, HeroComponent, ProductComponent, TestimonialsComponent, RecentlyViewedComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  constructor(public authService: AuthService) {}
}
