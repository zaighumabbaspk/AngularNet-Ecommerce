import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { ServiceResponse } from '../Models/checkout.model';
import { 
  GetOrder, 
  OrderSummary, 
  CreateOrder, 
  UpdateOrderStatus, 
  OrderStatus
} from '../Models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiBaseUrl}/Order`;

  constructor(private http: HttpClient) {}

  // Get specific order by ID
  getOrder(orderId: string): Observable<ServiceResponse<GetOrder>> {
    return this.http.get<ServiceResponse<GetOrder>>(`${this.apiUrl}/${orderId}`);
  }

  // Get user's order summaries
  getMyOrders(): Observable<ServiceResponse<OrderSummary[]>> {
    return this.http.get<ServiceResponse<OrderSummary[]>>(`${this.apiUrl}/my-orders`);
  }

  getMyOrdersWithDetails(): Observable<ServiceResponse<GetOrder[]>> {
    return this.http.get<ServiceResponse<GetOrder[]>>(`${this.apiUrl}/my-orders/details`);
  }

  // Create order from cart
  createOrderFromCart(createOrder: CreateOrder): Observable<ServiceResponse<GetOrder>> {
    return this.http.post<ServiceResponse<GetOrder>>(`${this.apiUrl}/create-from-cart`, createOrder);
  }

  // Update order status
  updateOrderStatus(updateStatus: UpdateOrderStatus): Observable<ServiceResponse<any>> {
    return this.http.put<ServiceResponse<any>>(`${this.apiUrl}/update-status`, updateStatus);
  }

  getOrdersByStatus(status: OrderStatus): Observable<ServiceResponse<GetOrder[]>> {
    return this.http.get<ServiceResponse<GetOrder[]>>(`${this.apiUrl}/admin/status/${status}`);
  }

  getOrdersByDateRange(startDate: Date, endDate: Date): Observable<ServiceResponse<GetOrder[]>> {
    const params = new HttpParams()
      .set('startDate', startDate.toISOString())
      .set('endDate', endDate.toISOString());
    
    return this.http.get<ServiceResponse<GetOrder[]>>(`${this.apiUrl}/admin/date-range`, { params });
  }
}