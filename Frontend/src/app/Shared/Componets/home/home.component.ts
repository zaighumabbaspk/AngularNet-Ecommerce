import { Component } from '@angular/core';
import { HeroComponent } from '../hero/hero.component';
import { TestimonialsComponent } from '../testimonials/testimonials.component';
import { ProductComponent } from '../product/product.component';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HeroComponent , ProductComponent, TestimonialsComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
