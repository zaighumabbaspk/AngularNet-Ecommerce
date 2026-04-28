import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { ServiceResponse } from '../Models/checkout.model';

export interface UserRoleInfo {
  email: string;
  role: string;
}

export interface UserDebugInfo {
  email: string;
  userId: string;
  userName: string;
  primaryRole: string;
  allRoles: string[];
  roleCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiBaseUrl}/Admin`;

  constructor(private http: HttpClient) {}

  // Make a user admin by email
  makeUserAdmin(email: string): Observable<ServiceResponse<any>> {
    return this.http.post<ServiceResponse<any>>(`${this.apiUrl}/make-admin/${email}`, {});
  }

  // Get user role by email
  getUserRole(email: string): Observable<UserRoleInfo> {
    return this.http.get<UserRoleInfo>(`${this.apiUrl}/user-role/${email}`);
  }

  // Debug user information
  debugUser(email: string): Observable<UserDebugInfo> {
    return this.http.get<UserDebugInfo>(`${this.apiUrl}/debug-user/${email}`);
  }

  // Fix admin role for a user
  fixAdminRole(email: string): Observable<ServiceResponse<any>> {
    return this.http.post<ServiceResponse<any>>(`${this.apiUrl}/fix-admin-role/${email}`, {});
  }
}