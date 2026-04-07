import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, timer } from 'rxjs';
import { Notification, NotificationType, NotificationConfig, NotificationPosition, NotificationAction } from '../Models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class CustomNotificationService {
  private notifications$ = new BehaviorSubject<Notification[]>([]);
  private defaultConfig: NotificationConfig = {
    duration: 4000,
    persistent: false,
    position: NotificationPosition.TOP_RIGHT,
    showCloseButton: true,
    showProgressBar: true,
    enableSound: false
  };

  constructor() {}

  getNotifications(): Observable<Notification[]> {
    return this.notifications$.asObservable();
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9) + Date.now().toString(36);
  }

  private addNotification(notification: Notification): void {
    const currentNotifications = this.notifications$.value;
    
    // Limit to maximum 5 notifications
    const updatedNotifications = [notification, ...currentNotifications.slice(0, 4)];
    this.notifications$.next(updatedNotifications);

    // Auto-remove notification if not persistent
    if (!notification.persistent && notification.duration && notification.duration > 0) {
      timer(notification.duration).subscribe(() => {
        this.removeNotification(notification.id);
      });
    }
  }

  removeNotification(id: string): void {
    const currentNotifications = this.notifications$.value;
    const updatedNotifications = currentNotifications.filter(n => n.id !== id);
    this.notifications$.next(updatedNotifications);
  }

  clearAll(): void {
    this.notifications$.next([]);
  }

  // Success notifications
  success(message: string, title: string = 'Success', config?: NotificationConfig): string {
    const notification: Notification = {
      id: this.generateId(),
      type: NotificationType.SUCCESS,
      title,
      message,
      duration: config?.duration ?? this.defaultConfig.duration,
      persistent: config?.persistent ?? false,
      icon: 'check-circle',
      actions: config?.actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }

  // Error notifications
  error(message: string, title: string = 'Error', config?: NotificationConfig): string {
    const notification: Notification = {
      id: this.generateId(),
      type: NotificationType.ERROR,
      title,
      message,
      duration: config?.duration ?? 6000, // Longer for errors
      persistent: config?.persistent ?? false,
      icon: 'x-circle',
      actions: config?.actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }

  // Warning notifications
  warning(message: string, title: string = 'Warning', config?: NotificationConfig): string {
    const notification: Notification = {
      id: this.generateId(),
      type: NotificationType.WARNING,
      title,
      message,
      duration: config?.duration ?? 5000,
      persistent: config?.persistent ?? false,
      icon: 'alert-triangle',
      actions: config?.actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }

  // Info notifications
  info(message: string, title: string = 'Info', config?: NotificationConfig): string {
    const notification: Notification = {
      id: this.generateId(),
      type: NotificationType.INFO,
      title,
      message,
      duration: config?.duration ?? this.defaultConfig.duration,
      persistent: config?.persistent ?? false,
      icon: 'info',
      actions: config?.actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }

  // eCommerce-specific notifications
  cartSuccess(message: string, title: string = 'Cart Updated', config?: NotificationConfig): string {
    const notification: Notification = {
      id: this.generateId(),
      type: NotificationType.CART,
      title,
      message,
      duration: config?.duration ?? 3000,
      persistent: config?.persistent ?? false,
      icon: 'shopping-cart',
      actions: config?.actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }

  orderSuccess(message: string, title: string = 'Order Confirmed', config?: NotificationConfig): string {
    const notification: Notification = {
      id: this.generateId(),
      type: NotificationType.ORDER,
      title,
      message,
      duration: config?.duration ?? 5000,
      persistent: config?.persistent ?? false,
      icon: 'package',
      actions: config?.actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }

  authSuccess(message: string, title: string = 'Authentication', config?: NotificationConfig): string {
    const notification: Notification = {
      id: this.generateId(),
      type: NotificationType.AUTH,
      title,
      message,
      duration: config?.duration ?? 3000,
      persistent: config?.persistent ?? false,
      icon: 'user-check',
      actions: config?.actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }

  authError(message: string, title: string = 'Authentication Failed', config?: NotificationConfig): string {
    const notification: Notification = {
      id: this.generateId(),
      type: NotificationType.ERROR,
      title,
      message,
      duration: config?.duration ?? 6000,
      persistent: config?.persistent ?? false,
      icon: 'user-x',
      actions: config?.actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }

  // Quick utility methods
  loginRequired(message: string = 'Please login to continue', config?: NotificationConfig): string {
    return this.warning(message, 'Login Required', { ...config, duration: 4000 });
  }

  networkError(message: string = 'Network error. Please check your connection.', config?: NotificationConfig): string {
    return this.error(message, 'Connection Error', { ...config, duration: 7000 });
  }

  validationError(message: string = 'Please check your input and try again.', config?: NotificationConfig): string {
    return this.warning(message, 'Validation Error', { ...config, duration: 5000 });
  }

  // Persistent notifications
  persistentError(message: string, title: string = 'Critical Error', config?: NotificationConfig): string {
    return this.error(message, title, { ...config, persistent: true, duration: 0 });
  }

  persistentSuccess(message: string, title: string = 'Important', config?: NotificationConfig): string {
    return this.success(message, title, { ...config, persistent: true, duration: 0 });
  }

  // Notification with custom actions
  withActions(
    type: NotificationType,
    message: string,
    title: string,
    actions: NotificationAction[],
    config?: NotificationConfig
  ): string {
    const notification: Notification = {
      id: this.generateId(),
      type,
      title,
      message,
      duration: config?.duration ?? this.defaultConfig.duration,
      persistent: config?.persistent ?? false,
      actions,
      timestamp: new Date()
    };
    
    this.addNotification(notification);
    return notification.id;
  }
}