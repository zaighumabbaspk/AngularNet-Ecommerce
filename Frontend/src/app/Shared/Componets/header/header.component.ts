import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../Core/Services/auth.service';

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

  constructor(public authService: AuthService) {}

  toggleSearch() {
    this.searchOpen = !this.searchOpen;
    console.log('Search toggled:', this.searchOpen);
    setTimeout(() => {
      feather.replace();
    }, 0);
  }

  logout(): void {
    this.authService.logout();
  }

  scrollToProducts(): void {
    const productSection = document.getElementById('products-section');
    if (productSection) {
      productSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}
