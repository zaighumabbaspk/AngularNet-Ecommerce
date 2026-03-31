export interface OrderBase {
  subtotal: number;
  tax: number;
  shipping: number;
  total: number;
  shippingAddress: string;
  billingAddress: string;
}

export interface CreateOrder extends OrderBase {
  stripeSessionId?: string;
  orderItems: CreateOrderItem[];
}

export interface CreateOrderItem {
  productId: string;
  quantity: number;
  unitPrice: number;
}

export interface GetOrder extends OrderBase {
  id: string;
  userId: string;
  createdAt: string;
  updatedAt?: string;
  status: OrderStatus;
  stripePaymentIntentId?: string;
  stripeSessionId?: string;
  orderItems: GetOrderItem[];
  statusHistory: GetOrderStatusHistory[];
}

export interface GetOrderItem {
  id: string;
  productId: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  productName: string;
  productDescription: string;
  productImage: string;
}

export interface GetOrderStatusHistory {
  id: string;
  status: OrderStatus;
  changedAt: string;
  changedBy: string;
  notes?: string;
}

export interface UpdateOrderStatus {
  orderId: string;
  status: OrderStatus;
  notes?: string;
}

export interface OrderSummary {
  id: string;
  createdAt: string;
  status: OrderStatus;
  total: number;
  itemCount: number;
}

export enum OrderStatus {
  Pending = 0,
  Processing = 1,
  Shipped = 2,
  Delivered = 3,
  Cancelled = 4,
  PaymentFailed = 5
}

export const OrderStatusLabels = {
  [OrderStatus.Pending]: 'Pending',
  [OrderStatus.Processing]: 'Processing',
  [OrderStatus.Shipped]: 'Shipped',
  [OrderStatus.Delivered]: 'Delivered',
  [OrderStatus.Cancelled]: 'Cancelled',
  [OrderStatus.PaymentFailed]: 'Payment Failed'
};

export const OrderStatusColors = {
  [OrderStatus.Pending]: 'warning',
  [OrderStatus.Processing]: 'info',
  [OrderStatus.Shipped]: 'primary',
  [OrderStatus.Delivered]: 'success',
  [OrderStatus.Cancelled]: 'danger',
  [OrderStatus.PaymentFailed]: 'danger'
};