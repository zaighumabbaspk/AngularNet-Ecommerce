import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-guest-order-success',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './guest-order-success.component.html',
  styleUrls: ['./guest-order-success.component.css']
})
export class GuestOrderSuccessComponent implements OnInit {
  orderNumber: string = '';
  email: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.orderNumber = params['orderNumber'] || '';
      this.email = params['email'] || '';
      
      if (!this.orderNumber || !this.email) {
        this.router.navigate(['/']);
      }
    });
  }

  trackOrder(): void {
    this.router.navigate(['/guest-order-tracking'], {
      queryParams: { email: this.email, orderNumber: this.orderNumber }
    });
  }
}