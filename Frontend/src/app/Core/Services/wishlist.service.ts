import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { switchMap, startWith, tap } from 'rxjs/operators';
import { environment } from '../../environment/environment';
import { AddToWishlistRequest, WishlistResponse } from '../Models/wishlist.models';

@Injectable({
  providedIn: 'root'
})
export class WishlistService {
  private baseUrl = `${environment.apiBaseUrl }/Wishlist`;
  private wishlistUpdated$ = new Subject<void>();

  constructor(private http: HttpClient) { }

  addToWishlist(productId: string): Observable<any> {
    const request: AddToWishlistRequest = {
      productId,
      userId: '' // Will be set by backend from JWT token
    };
    return this.http.post<any>(`${this.baseUrl}/add`, request).pipe(
      tap(() => this.wishlistUpdated$.next())
    );
  }

  removeFromWishlist(productId: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/remove/${productId}`).pipe(
      tap(() => this.wishlistUpdated$.next())
    );
  }

  getWishlist(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}`);
  }

  getWishlistWithUpdates(): Observable<any> {
    return this.wishlistUpdated$.pipe(
      startWith(null),
      switchMap(() => this.getWishlist())
    );
  }

  isInWishlist(productId: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/check/${productId}`);
  }
}