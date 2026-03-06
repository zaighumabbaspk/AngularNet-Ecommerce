import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { AuthService } from '../../../Core/Services/auth.service';
import { CartService } from '../../../Core/Services/cart.service';
import { ToastrService } from 'ngx-toastr';

declare var feather: any;

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  searchOpen = false;
  showHero = true;

  constructor(
    public authService: AuthService,
    public cartService: CartService,
    private toastr: ToastrService,
    private router: Router
  ) {
    // hide hero section on any route except home
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showHero = event.urlAfterRedirects === '/' || event.urlAfterRedirects === '';
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.toastr.info('You have been logged out successfully', 'Goodbye!', {
      timeOut: 3000,
      progressBar: true
    });
  }

  scrollToProducts(): void {
    const productSection = document.getElementById('products-section');
    if (productSection) {
      productSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  goToCart(): void {
    if (!this.authService.isAuthenticated()) {
      this.toastr.warning('Please login to view your cart', 'Login Required', {
        timeOut: 3000,
        progressBar: true
      });
      return;
    }
    this.router.navigate(['/cart']);
  }
}
