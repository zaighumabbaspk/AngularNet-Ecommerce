import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable, Subscription } from 'rxjs';
import { CustomNotificationService } from '../../../Core/Services/custom-notification.service';
import { Notification, NotificationPosition } from '../../../Core/Models/notification.model';
import { NotificationComponent } from '../notification/notification.component';

@Component({
  selector: 'app-notification-container',
  standalone: true,
  imports: [CommonModule, NotificationComponent],
  template: `
    <div 
      class="notification-container"
      [class]="'notification-container-' + position"
    >
      <app-notification
        *ngFor="let notification of notifications; trackBy: trackByFn"
        [notification]="notification"
        (closed)="onNotificationClosed($event)"
      ></app-notification>
    </div>
  `,
  styleUrls: ['./notification-container.component.css']
})
export class NotificationContainerComponent implements OnInit, OnDestroy {
  notifications: Notification[] = [];
  position: NotificationPosition = NotificationPosition.TOP_RIGHT;
  private subscription?: Subscription;

  constructor(private notificationService: CustomNotificationService) {}

  ngOnInit() {
    this.subscription = this.notificationService.getNotifications().subscribe(
      notifications => {
        this.notifications = notifications;
      }
    );
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  onNotificationClosed(id: string) {
    this.notificationService.removeNotification(id);
  }

  trackByFn(index: number, item: Notification): string {
    return item.id;
  }
}