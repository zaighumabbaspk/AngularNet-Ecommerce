import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../Core/Services/product.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ToastrService } from 'ngx-toastr';
import { Product } from '../../../Core/Models/product.model';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {

  product?: Product;
  isLoading: boolean = true;
  selectedQuantity: number = 1;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.loadProduct(id);
      }
    });
  }

  loadProduct(id: string) {
    this.isLoading = true;

    this.productService.getProductById(id).subscribe({
      next: (res: Product) => {
        this.product = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading product:', err);
        this.isLoading = false;
        this.toastr.error(
          'Failed to load product details. Please try again.',
          'Error',
          {
            timeOut: 4000,
            progressBar: true
          }
        );
      }
    });
  }

  increaseQuantity() {
    if (this.product && this.selectedQuantity < this.product.quantity) {
      this.selectedQuantity++;
    } 
    else if (this.product && this.selectedQuantity >= this.product.quantity) {
      this.toastr.warning(
        'Cannot exceed available stock',
        'Stock Limit',
        {
          timeOut: 2000,
          progressBar: true
        }
      );
    }
  }

  decreaseQuantity() {
    if (this.selectedQuantity > 1) {
      this.selectedQuantity--;
    } else {
      this.toastr.warning(
        'Minimum quantity is 1',
        'Limit',
        {
          timeOut: 2000,
          progressBar: true
        }
      );
    }
  }
}