import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartDrawerService {
  private isOpenSubject = new BehaviorSubject<boolean>(false);
  public isOpen$ = this.isOpenSubject.asObservable();

  constructor() {}

  openDrawer(): void {
    this.isOpenSubject.next(true);
  }

  closeDrawer(): void {
    this.isOpenSubject.next(false);
  }

  toggleDrawer(): void {
    this.isOpenSubject.next(!this.isOpenSubject.value);
  }

  isDrawerOpen(): boolean {
    return this.isOpenSubject.value;
  }
}
