import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../../Core/Services/cart.service';
import { CartBase } from '../cart/cart-base';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent extends CartBase {
  constructor(cartService: CartService) {
    super(cartService);
  }

  // Override the default checkout behavior for the cart page.
  override checkout(): void {
    // TODO: Implement checkout flow
    alert('Checkout functionality coming soon!');
  }
}
