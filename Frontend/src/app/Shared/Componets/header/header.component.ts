import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../../../Core/Services/auth.service';
import { CartService } from '../../../Core/Services/cart.service';
import { WishlistService } from '../../../Core/Services/wishlist.service';
import { CartDrawerService } from '../../../Core/Services/cart-drawer.service';
import { SearchAutocompleteComponent } from '../search-autocomplete/search-autocomplete.component';
import { ToastrService } from 'ngx-toastr';

declare var feather: any;

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, SearchAutocompleteComponent],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  searchOpen = false;
  userMenuOpen = false;
  showHero = true;
  isDarkBackground = false;
  wishlist$!: Observable<any>;

  constructor(
    public authService: AuthService,
    public cartService: CartService,
    public wishlistService: WishlistService,
    private cartDrawerService: CartDrawerService,
    private toastr: ToastrService,
    private router: Router
  ) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showHero = event.urlAfterRedirects === '/' || event.urlAfterRedirects === '';
        this.updateNavbarStyle();
      }
    });
  }

  ngOnInit(): void {
    this.wishlist$ = this.wishlistService.getWishlistWithUpdates();
  }



  toggleUserMenu(): void {
    this.userMenuOpen = !this.userMenuOpen;
  }

  closeUserMenu(): void {
    this.userMenuOpen = false;
  }

  @HostListener('window:scroll', ['$event'])
  onScroll(): void {
    this.updateNavbarStyle();
  }

  updateNavbarStyle(): void {
    if (this.showHero) {
      const scrollPosition = window.scrollY;
      this.isDarkBackground = scrollPosition > 100; 
    } else {
      this.isDarkBackground = true;
    }
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

  openCartDrawer(): void {
    if (!this.authService.isAuthenticated()) {
      this.toastr.warning('Please login to view your cart', 'Login Required', {
        timeOut: 3000,
        progressBar: true
      });
      return;
    }
    this.cartDrawerService.openDrawer();
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

