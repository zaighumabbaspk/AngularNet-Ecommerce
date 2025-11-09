import { Component, AfterViewInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Shared/Componets/header/header.component';
import { FooterComponent } from './Shared/Componets/footer/footer.component';
import feather from 'feather-icons';
import { HeroComponent } from './Shared/Componets/hero/hero.component';
import { ProductComponent } from './Shared/Componets/product/product.component';
import { TestimonialsComponent } from './Shared/Componets/testimonials/testimonials.component';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeroComponent, ProductComponent, TestimonialsComponent, HeaderComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'e-commerce-angular';


}
