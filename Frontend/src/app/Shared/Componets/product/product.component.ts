import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../../Core/Services/product.service';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
})
export class ProductComponent implements OnInit {
  
  products: any[] = [];
  isLoading: boolean = false;
  errorMessage: string = '';

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;

    this.productService.getAllProducts().subscribe({
      next: (res) => {
        console.log('✅ Products loaded:', res);
        this.products = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('❌ Error loading products:', err);
        this.errorMessage = 'Failed to load products. Please try again later.';
        this.isLoading = false;
      },
    });
  }

  // Optional — handle "Add to Cart" button
  addToCart(product: any): void {
    console.log('🛒 Product added to cart:', product);
    // TODO: integrate with CartService later
  }
}
