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

  getProductById(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/ById/${id}`);
  }
}
