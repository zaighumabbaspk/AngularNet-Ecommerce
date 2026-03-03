import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../../Core/Services/product.service';
import { CategoryService } from '../../../Core/Services/category.service';
import { AuthService } from '../../../Core/Services/auth.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../Core/Models/product.model';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
})
export class ProductComponent implements OnInit {

  products: Product[] = [];
  filteredProducts: any[] = [];
  categories: any[] = [];

  selectedCategoryId: string = '';
  searchQuery: string = '';

  priceMinLimit = 0;
  priceMaxLimit = 1000;

  minPrice: number = 0;
  maxPrice: number = 1000;

  rangeFillLeft = 0; 
  rangeFillRight = 100;

  isLoading: boolean = false;
  errorMessage: string = '';

  productForm: FormGroup;
  showForm: boolean = false;
  editMode: boolean = false;
  editingProductId: string | null = null;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      image: ['', Validators.required],
      quantity: [0, [Validators.required, Validators.min(0)]],
      categoryId: ['']
    });
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.productService.getAllProducts().subscribe({
      next: (res) => {
        this.products = res;
        console.log('Products loaded:', this.products);
        this.setPriceLimits();
        this.applyFilters();
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load products.';
        this.isLoading = false;
      }
    });
  }

  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe({
      next: (res) => this.categories = res
    });
  }

  setPriceLimits(): void {
    if (this.products.length === 0) {
      this.priceMinLimit = 0;
      this.priceMaxLimit = 1000;
      this.minPrice = 0;
      this.maxPrice = 1000;
      this.updateRangeFill();
      return;
    }
    const prices = this.products.map(p => p.price);
    this.priceMinLimit = Math.floor(Math.min(...prices));
    this.priceMaxLimit = Math.ceil(Math.max(...prices));

    this.minPrice = this.priceMinLimit;
    this.maxPrice = this.priceMaxLimit;
    
    this.updateRangeFill();
  }

  filterByCategory(categoryId: string): void {
    this.selectedCategoryId = categoryId;
    this.applyFilters();
  }

  onMinSliderChange(event: any): void {
    const value = Number(event.target.value);
    if (value <= this.maxPrice) {
      this.minPrice = value;
    } else {
      this.minPrice = this.maxPrice;
      event.target.value = this.maxPrice;
    }
    this.applyFilters();
    this.updateRangeFill();
  }

  onMaxSliderChange(event: any): void {
    const value = Number(event.target.value);
    if (value >= this.minPrice) {
      this.maxPrice = value;
    } else {
      this.maxPrice = this.minPrice;
      event.target.value = this.minPrice;
    }
    this.applyFilters();
    this.updateRangeFill();
  }

  onInputChange(): void {
    
    if (this.minPrice < this.priceMinLimit) this.minPrice = this.priceMinLimit;
    if (this.minPrice > this.priceMaxLimit) this.minPrice = this.priceMaxLimit;
    if (this.maxPrice > this.priceMaxLimit) this.maxPrice = this.priceMaxLimit;
    if (this.maxPrice < this.priceMinLimit) this.maxPrice = this.priceMinLimit;

  
    if (this.minPrice > this.maxPrice) {
      [this.minPrice, this.maxPrice] = [this.maxPrice, this.minPrice];
    }

    this.applyFilters();
    this.updateRangeFill();
  }

  updateRangeFill(): void {
    const totalRange = this.priceMaxLimit - this.priceMinLimit;
    this.rangeFillLeft = ((this.minPrice - this.priceMinLimit) / totalRange) * 100;
    this.rangeFillRight = ((this.maxPrice - this.priceMinLimit) / totalRange) * 100;
  }

  applyFilters(): void {
    this.filteredProducts = this.products.filter(p => {
      const matchCategory =
        !this.selectedCategoryId ||
        p.categoryId === this.selectedCategoryId;

      const matchMin = p.price >= this.minPrice;
      const matchMax = p.price <= this.maxPrice;

      const matchSearch = !this.searchQuery ||
        p.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        p.description.toLowerCase().includes(this.searchQuery.toLowerCase());

      return matchCategory && matchMin && matchMax && matchSearch;
    });
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.applyFilters();
  }

  addToCart(product: any): void {
    console.log('Added to cart:', product);
  }

  openAddForm(): void {
    this.showForm = true;
    this.editMode = false;
    this.editingProductId = null;
    this.productForm.reset();
  }

  openEditForm(product: any): void {
    this.showForm = true;
    this.editMode = true;
    this.editingProductId = product.id;
    this.productForm.patchValue(product);
  }

  submitForm(): void {
    if (this.productForm.invalid) return;

    const data = this.productForm.value;

    if (this.editMode && this.editingProductId) {
      this.productService.updateProduct(this.editingProductId, data)
        .subscribe(() => {
          this.loadProducts();
          this.cancelForm();
        });
    } else {
      this.productService.createProduct(data)
        .subscribe(() => {
          this.loadProducts();
          this.cancelForm();
        });
    }
  }

  deleteProduct(id: string): void {
    if (!confirm('Delete this product?')) return;

    this.productService.deleteProduct(id)
      .subscribe(() => this.loadProducts());
  }

  cancelForm(): void {
    this.showForm = false;
    this.editMode = false;
    this.editingProductId = null;
  }
}