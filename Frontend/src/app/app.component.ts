import { Component, AfterViewInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Shared/Componets/header/header.component';
import { FooterComponent } from './Shared/Componets/footer/footer.component';
import { CartDrawerComponent } from './Shared/Componets/cart-drawer/cart-drawer.component';
import { NotificationContainerComponent } from './Shared/Componets/notification-container/notification-container.component';
import { environment } from './environment/environment';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, CartDrawerComponent, NotificationContainerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements AfterViewInit {
  title = 'e-commerce-angular';

  ngAfterViewInit(): void {

  }
}
