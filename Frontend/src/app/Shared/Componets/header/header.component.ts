import { Component, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 

declare var feather: any;

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent  {
  searchOpen = false;

  toggleSearch() {
    this.searchOpen = !this.searchOpen;
    console.log('Search toggled:', this.searchOpen); // For debugging
    setTimeout(() => {
      feather.replace(); // ✅ Refresh icons after DOM update
    }, 0);
  }

 
}
