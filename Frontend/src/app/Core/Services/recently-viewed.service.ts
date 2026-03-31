import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { AddRecentlyViewedRequest, RecentlyViewedResponse } from '../Models/recently-viewed.models';

@Injectable({
  providedIn: 'root'
})
export class RecentlyViewedService {
  private baseUrl = `${environment.apiBaseUrl}/RecentlyViewed`;

  constructor(private http: HttpClient) { }

  addRecentlyViewed(productId: string): Observable<any> {
    const request: AddRecentlyViewedRequest = {
      productId,
      userId: '' // Will be set by backend from JWT token
    };
    return this.http.post<any>(`${this.baseUrl}/add`, request);
  }

  getRecentlyViewed(limit: number = 10): Observable<any> {
    let params = new HttpParams().set('limit', limit.toString());
    return this.http.get<any>(`${this.baseUrl}`, { params });
  }

  clearRecentlyViewed(): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/clear`);
  }
}