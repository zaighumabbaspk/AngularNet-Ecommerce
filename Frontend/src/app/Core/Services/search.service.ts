import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { SearchRequest, SearchResponse, AutocompleteRequest, AutocompleteResponse } from '../Models/search.models';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private baseUrl = `${environment.apiBaseUrl}/Search`;

  constructor(private http: HttpClient) { }

  searchProducts(request: SearchRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/products`, request);
  }

  getAutocomplete(query: string, limit: number = 10): Observable<any> {
    let params = new HttpParams()
      .set('query', query)
      .set('limit', limit.toString());

    return this.http.get<any>(`${this.baseUrl}/autocomplete`, { params });
  }

  getPopularSearchTerms(limit: number = 10): Observable<any> {
    let params = new HttpParams().set('limit', limit.toString());
    return this.http.get<any>(`${this.baseUrl}/popular-terms`, { params });
  }

  trackSearch(searchTerm: string, resultCount: number, userId?: string): Observable<any> {
    const request = {
      searchTerm,
      resultCount,
      userId
    };
    return this.http.post<any>(`${this.baseUrl}/track`, request);
  }
}