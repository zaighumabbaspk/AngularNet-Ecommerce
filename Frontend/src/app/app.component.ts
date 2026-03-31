import { Component, AfterViewInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Shared/Componets/header/header.component';
import { FooterComponent } from './Shared/Componets/footer/footer.component';
import { CartDrawerComponent } from './Shared/Componets/cart-drawer/cart-drawer.component';
import { HeroComponent } from './Shared/Componets/hero/hero.component';
import { ProductComponent } from './Shared/Componets/product/product.component';
import { TestimonialsComponent } from './Shared/Componets/testimonials/testimonials.component';
import { environment } from './environment/environment';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeroComponent, ProductComponent, TestimonialsComponent, HeaderComponent, FooterComponent, CartDrawerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements AfterViewInit {
  title = 'e-commerce-angular';

  ngAfterViewInit(): void {
    // App initialized
    console.log('✅ App initialized successfully');
  }
}
