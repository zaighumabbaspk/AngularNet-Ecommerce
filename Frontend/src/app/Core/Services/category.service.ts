import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(
    private http: HttpClient
  ) {}

  private baseUrl = `${environment.apiBaseUrl}/Category`;

 getAllCategories() : Observable<any> {
     return this.http.get(`${this.baseUrl}/all`);
 }
 
}
