import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';
import { ServiceResponse } from '../Models/checkout.model';

export interface Category {
  id: string;
  name: string;
  description?: string;
}

export interface CreateCategory {
  name: string;
  description?: string;
}

export interface UpdateCategory {
  id: string;
  name: string;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(
    private http: HttpClient
  ) {}

  private baseUrl = `${environment.apiBaseUrl}/Category`;

  // Get all categories
  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/all`);
  }

  // Get category by ID
  getCategoryById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.baseUrl}/ById/${id}`);
  }

  // Create new category (Admin only)
  createCategory(category: CreateCategory): Observable<ServiceResponse<any>> {
    return this.http.post<ServiceResponse<any>>(`${this.baseUrl}/add`, category);
  }

  // Update category (Admin only)
  updateCategory(category: UpdateCategory): Observable<ServiceResponse<any>> {
    return this.http.put<ServiceResponse<any>>(`${this.baseUrl}/update`, category);
  }

  // Delete category (Admin only)
  deleteCategory(id: string): Observable<ServiceResponse<any>> {
    return this.http.delete<ServiceResponse<any>>(`${this.baseUrl}/delete/${id}`);
  }
}
