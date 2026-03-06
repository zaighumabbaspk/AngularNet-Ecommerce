import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { AuthService } from '../../../Core/Services/auth.service';
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
  isDarkNavbarNeeded = false;
  whiteBackgroundPages = ['/about', '/login', '/signup', '/forgot-password', '/reset-password', '/verify-email'];

  constructor(public authService: AuthService, private toastr: ToastrService, private router: Router) {
    // hide hero section on any route except home
    // apply dark navbar on white background pages
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showHero = event.urlAfterRedirects === '/' || event.urlAfterRedirects === '';
        this.isDarkNavbarNeeded = this.whiteBackgroundPages.some(page => event.urlAfterRedirects.startsWith(page));
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
}
