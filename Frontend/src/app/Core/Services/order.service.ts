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
  OrderStatus,
  GetOrderStatusHistory
} from '../Models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiBaseUrl}/Order`;

  constructor(private http: HttpClient) {}

  // Create order (direct creation)
  createOrder(createOrder: CreateOrder): Observable<ServiceResponse<GetOrder>> {
    return this.http.post<ServiceResponse<GetOrder>>(`${this.apiUrl}/create`, createOrder);
  }

  // Get user orders (alternative endpoint)
  getUserOrders(): Observable<ServiceResponse<OrderSummary[]>> {
    return this.http.get<ServiceResponse<OrderSummary[]>>(`${this.apiUrl}/user-orders`);
  }

  // Get specific order by ID
  getOrder(orderId: string): Observable<ServiceResponse<GetOrder>> {
    return this.http.get<ServiceResponse<GetOrder>>(`${this.apiUrl}/${orderId}`);
  }

  // Get user's order summaries
  getMyOrders(): Observable<ServiceResponse<OrderSummary[]>> {
    return this.http.get<ServiceResponse<OrderSummary[]>>(`${this.apiUrl}/my-orders`);
  }

  // Get user's orders with full details
  getMyOrdersWithDetails(): Observable<ServiceResponse<GetOrder[]>> {
    return this.http.get<ServiceResponse<GetOrder[]>>(`${this.apiUrl}/my-orders/details`);
  }

  // Create order from cart
  createOrderFromCart(createOrder: CreateOrder): Observable<ServiceResponse<GetOrder>> {
    return this.http.post<ServiceResponse<GetOrder>>(`${this.apiUrl}/create-from-cart`, createOrder);
  }

  // Update order status (user)
  updateOrderStatus(updateStatus: UpdateOrderStatus): Observable<ServiceResponse<any>> {
    return this.http.put<ServiceResponse<any>>(`${this.apiUrl}/update-status`, updateStatus);
  }

  // Admin: Update order status by order ID
  updateOrderStatusAdmin(orderId: string, updateStatus: UpdateOrderStatus): Observable<ServiceResponse<any>> {
    return this.http.put<ServiceResponse<any>>(`${this.apiUrl}/${orderId}/status`, updateStatus);
  }

  // Admin: Get orders by status
  getOrdersByStatus(status: OrderStatus): Observable<ServiceResponse<GetOrder[]>> {
    return this.http.get<ServiceResponse<GetOrder[]>>(`${this.apiUrl}/admin/status/${status}`);
  }

  // Admin: Get orders by date range
  getOrdersByDateRange(startDate: Date, endDate: Date): Observable<ServiceResponse<GetOrder[]>> {
    const params = new HttpParams()
      .set('startDate', startDate.toISOString())
      .set('endDate', endDate.toISOString());
    
    return this.http.get<ServiceResponse<GetOrder[]>>(`${this.apiUrl}/admin/date-range`, { params });
  }

  // Admin: Get all orders
  getAllOrders(): Observable<ServiceResponse<GetOrder[]>> {
    return this.http.get<ServiceResponse<GetOrder[]>>(`${this.apiUrl}/all`);
  }

  // Get order status history
  getOrderStatusHistory(orderId: string): Observable<ServiceResponse<GetOrderStatusHistory[]>> {
    return this.http.get<ServiceResponse<GetOrderStatusHistory[]>>(`${this.apiUrl}/${orderId}/history`);
  }
}