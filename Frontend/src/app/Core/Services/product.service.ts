import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';



@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private http = inject(HttpClient);
  private baseUrl = `${environment.apiBaseUrl}/Product`; 

  constructor() { }


  getAllProducts(): Observable<any> {
    return this.http.get(`${this.baseUrl}/all`);
  }

  getProductById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/ById/${id}`);
  }

  createProduct(productData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/add`, productData);
  }

  updateProduct(id: string, productData: any): Observable<any> {
    const updateData = { ...productData, id };
    return this.http.put(`${this.baseUrl}/update`, updateData);
  }

  deleteProduct(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/delete/${id}`);
  }
  
}
