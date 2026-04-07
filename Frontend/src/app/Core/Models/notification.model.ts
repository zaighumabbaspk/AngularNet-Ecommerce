export interface Notification {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  duration?: number;
  persistent?: boolean;
  icon?: string;
  actions?: NotificationAction[];
  timestamp: Date;
}

export interface NotificationAction {
  label: string;
  action: () => void;
  style?: 'primary' | 'secondary' | 'danger';
}

export enum NotificationType {
  SUCCESS = 'success',
  ERROR = 'error',
  WARNING = 'warning',
  INFO = 'info',
  CART = 'cart',
  ORDER = 'order',
  AUTH = 'auth'
}

export interface NotificationConfig {
  duration?: number;
  persistent?: boolean;
  position?: NotificationPosition;
  showCloseButton?: boolean;
  showProgressBar?: boolean;
  enableSound?: boolean;
  actions?: NotificationAction[];
}

export enum NotificationPosition {
  TOP_RIGHT = 'top-right',
  TOP_LEFT = 'top-left',
  TOP_CENTER = 'top-center',
  BOTTOM_RIGHT = 'bottom-right',
  BOTTOM_LEFT = 'bottom-left',
  BOTTOM_CENTER = 'bottom-center'
}