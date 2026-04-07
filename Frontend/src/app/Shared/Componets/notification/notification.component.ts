import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { Notification } from '../../../Core/Models/notification.model';
import { timer, Subscription } from 'rxjs';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div 
      class="notification notification-{{ notification.type }}"
      [@slideIn]="animationState"
      (@slideIn.done)="onAnimationDone($event)"
      [class.notification-persistent]="notification.persistent"
    >
      <!-- Progress bar -->
      <div 
        *ngIf="!notification.persistent && showProgress" 
        class="notification-progress"
        [style.animation-duration.ms]="notification.duration"
      ></div>
      
      <!-- Close button -->
      <button 
        *ngIf="showCloseButton"
        class="notification-close"
        (click)="close()"
        aria-label="Close notification"
      >
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="18" y1="6" x2="6" y2="18"></line>
          <line x1="6" y1="6" x2="18" y2="18"></line>
        </svg>
      </button>

      <!-- Content -->
      <div class="notification-content">
        <!-- Icon -->
        <div class="notification-icon">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <!-- Success icon -->
            <path *ngIf="notification.icon === 'check-circle'" d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
            <polyline *ngIf="notification.icon === 'check-circle'" points="9,11 12,14 22,4"></polyline>
            
            <!-- Error icon -->
            <circle *ngIf="notification.icon === 'x-circle'" cx="12" cy="12" r="10"></circle>
            <line *ngIf="notification.icon === 'x-circle'" x1="15" y1="9" x2="9" y2="15"></line>
            <line *ngIf="notification.icon === 'x-circle'" x1="9" y1="9" x2="15" y2="15"></line>
            
            <!-- Warning icon -->
            <path *ngIf="notification.icon === 'alert-triangle'" d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z"></path>
            <line *ngIf="notification.icon === 'alert-triangle'" x1="12" y1="9" x2="12" y2="13"></line>
            <line *ngIf="notification.icon === 'alert-triangle'" x1="12" y1="17" x2="12.01" y2="17"></line>
            
            <!-- Info icon -->
            <circle *ngIf="notification.icon === 'info'" cx="12" cy="12" r="10"></circle>
            <line *ngIf="notification.icon === 'info'" x1="12" y1="16" x2="12" y2="12"></line>
            <line *ngIf="notification.icon === 'info'" x1="12" y1="8" x2="12.01" y2="8"></line>
            
            <!-- Cart icon -->
            <path *ngIf="notification.icon === 'shopping-cart'" d="m2 2 3.6 7.59-1.35 2.45c-.16.28-.25.61-.25.96 0 1.1.9 2 2 2h12"></path>
            <circle *ngIf="notification.icon === 'shopping-cart'" cx="9" cy="20" r="1"></circle>
            <circle *ngIf="notification.icon === 'shopping-cart'" cx="20" cy="20" r="1"></circle>
            <path *ngIf="notification.icon === 'shopping-cart'" d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
            
            <!-- Package icon -->
            <path *ngIf="notification.icon === 'package'" d="m7.5 4.27 4.5 2.6 4.5-2.6"></path>
            <path *ngIf="notification.icon === 'package'" d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"></path>
            <path *ngIf="notification.icon === 'package'" d="v13l4-2.5V9L12 7l-4 2v4.5l4 2.5z"></path>
            
            <!-- User check icon -->
            <path *ngIf="notification.icon === 'user-check'" d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path>
            <circle *ngIf="notification.icon === 'user-check'" cx="9" cy="7" r="4"></circle>
            <polyline *ngIf="notification.icon === 'user-check'" points="16,11 18,13 22,9"></polyline>
            
            <!-- User X icon -->
            <path *ngIf="notification.icon === 'user-x'" d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path>
            <circle *ngIf="notification.icon === 'user-x'" cx="9" cy="7" r="4"></circle>
            <line *ngIf="notification.icon === 'user-x'" x1="17" y1="8" x2="22" y2="13"></line>
            <line *ngIf="notification.icon === 'user-x'" x1="22" y1="8" x2="17" y2="13"></line>
          </svg>
        </div>

        <!-- Text content -->
        <div class="notification-text">
          <div class="notification-title">{{ notification.title }}</div>
          <div class="notification-message">{{ notification.message }}</div>
        </div>
      </div>

      <!-- Actions -->
      <div *ngIf="notification.actions && notification.actions.length > 0" class="notification-actions">
        <button 
          *ngFor="let action of notification.actions"
          class="notification-action notification-action-{{ action.style || 'secondary' }}"
          (click)="executeAction(action)"
        >
          {{ action.label }}
        </button>
      </div>
    </div>
  `,
  styleUrls: ['./notification.component.css'],
  animations: [
    trigger('slideIn', [
      state('in', style({ transform: 'translateX(0)', opacity: 1 })),
      transition('void => *', [
        style({ transform: 'translateX(100%)', opacity: 0 }),
        animate('300ms cubic-bezier(0.25, 0.8, 0.25, 1)')
      ]),
      transition('* => void', [
        animate('300ms cubic-bezier(0.25, 0.8, 0.25, 1)', 
          style({ transform: 'translateX(100%)', opacity: 0 }))
      ])
    ])
  ]
})
export class NotificationComponent implements OnInit, OnDestroy {
  @Input() notification!: Notification;
  @Input() showCloseButton = true;
  @Input() showProgress = true;
  @Output() closed = new EventEmitter<string>();

  animationState = 'in';
  private progressSubscription?: Subscription;

  ngOnInit() {
    // Start progress animation if not persistent
    if (!this.notification.persistent && this.notification.duration && this.notification.duration > 0) {
      this.progressSubscription = timer(this.notification.duration).subscribe(() => {
        this.close();
      });
    }
  }

  ngOnDestroy() {
    if (this.progressSubscription) {
      this.progressSubscription.unsubscribe();
    }
  }

  close() {
    this.animationState = 'out';
  }

  onAnimationDone(event: any) {
    if (event.toState === 'void') {
      this.closed.emit(this.notification.id);
    }
  }

  executeAction(action: any) {
    action.action();
    this.close();
  }
}