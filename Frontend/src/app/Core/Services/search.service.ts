import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environment/environment';
import { SearchRequest, SearchResponse, AutocompleteRequest, AutocompleteResponse } from '../Models/search.models';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private baseUrl = `${environment.apiBaseUrl}/Search`;

  constructor(private http: HttpClient) { }

  searchProducts(request: SearchRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/products`, request)
      .pipe(catchError(this.handleError));
  }

  getAutocomplete(query: string, limit: number = 10): Observable<any> {
    let params = new HttpParams()
      .set('query', query)
      .set('limit', limit.toString());

    return this.http.get<any>(`${this.baseUrl}/autocomplete`, { params })
      .pipe(catchError(this.handleError));
  }

  getPopularSearchTerms(limit: number = 10): Observable<any> {
    let params = new HttpParams().set('limit', limit.toString());
    return this.http.get<any>(`${this.baseUrl}/popular-terms`, { params })
      .pipe(catchError(this.handleError));
  }

  trackSearch(searchTerm: string, resultCount: number, userId?: string): Observable<any> {
    const request = {
      searchTerm,
      resultCount,
      userId
    };
    return this.http.post<any>(`${this.baseUrl}/track`, request)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    console.error('Search service error:', error);
    
    let errorMessage = 'An error occurred';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      errorMessage = error.error?.message || `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    return throwError(() => new Error(errorMessage));
  }
}