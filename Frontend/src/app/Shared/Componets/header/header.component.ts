import { Component, HostListener, OnInit, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AuthService } from '../../../Core/Services/auth.service';
import { CartService } from '../../../Core/Services/cart.service';
import { WishlistService } from '../../../Core/Services/wishlist.service';
import { CartDrawerService } from '../../../Core/Services/cart-drawer.service';
import { SearchAutocompleteComponent } from '../search-autocomplete/search-autocomplete.component';
import { CustomNotificationService } from '../../../Core/Services/custom-notification.service';

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
    private notification: CustomNotificationService,
    private router: Router,
    private elementRef: ElementRef
  ) {
    // hide hero section on any route except home
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showHero = event.urlAfterRedirects === '/' || event.urlAfterRedirects === '';
        this.updateNavbarStyle();
      }
    });
  }

  ngOnInit(): void {
    // Only load wishlist when user is authenticated
    this.authService.authState$.subscribe(authState => {
      if (authState.isAuthenticated) {
        this.wishlist$ = this.wishlistService.getWishlistWithUpdates();
      } else {
        // Return empty observable for unauthenticated users
        this.wishlist$ = of({ success: true, data: { items: [] } });
      }
    });
  }



  toggleUserMenu(): void {
    this.userMenuOpen = !this.userMenuOpen;
  }

  closeUserMenu(): void {
    this.userMenuOpen = false;
  }

  closeMobileMenu(): void {
    // Close Bootstrap collapse on mobile
    const navbarCollapse = document.getElementById('navbarMenu');
    if (navbarCollapse && navbarCollapse.classList.contains('show')) {
      const bsCollapse = (window as any).bootstrap?.Collapse?.getInstance(navbarCollapse);
      if (bsCollapse) {
        bsCollapse.hide();
      }
    }
  }

  @HostListener('window:scroll', ['$event'])
  onScroll(): void {
    this.updateNavbarStyle();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;
    const userMenuWrapper = this.elementRef.nativeElement.querySelector('.user-menu-wrapper');
    const mobileUserMenu = this.elementRef.nativeElement.querySelector('.mobile-user-menu');
    
    // Close user menu if clicking outside of it
    if (this.userMenuOpen && userMenuWrapper && !userMenuWrapper.contains(target) && 
        mobileUserMenu && !mobileUserMenu.contains(target)) {
      this.userMenuOpen = false;
    }
  }

  updateNavbarStyle(): void {
    // Check if we're on home page with hero
    if (this.showHero) {
      // On home page, check scroll position
      const scrollPosition = window.scrollY;
      this.isDarkBackground = scrollPosition > 100; // After scrolling past hero
    } else {
      // On other pages, always use dark background
      this.isDarkBackground = true;
    }
  }

  logout(): void {
    this.authService.logout();
    this.notification.authSuccess('You have been logged out successfully', 'Goodbye!');
  }

  scrollToProducts(): void {
    const productSection = document.getElementById('products-section');
    if (productSection) {
      productSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  openCartDrawer(): void {
    // Allow both authenticated and guest users to view cart
    this.cartDrawerService.openDrawer();
  }

  goToCart(): void {
    // Allow both authenticated and guest users to view cart
    this.router.navigate(['/cart']);
  }
}

