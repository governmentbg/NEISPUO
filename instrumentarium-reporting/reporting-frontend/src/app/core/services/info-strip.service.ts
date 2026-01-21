import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class InfoStripService {
  private _isVisible = new BehaviorSubject<boolean>(true);
  private readonly HIDE_DURATION_MS = 7 * 24 * 60 * 60 * 1000;

  constructor() {
    this.checkVisibility();
  }

  get isVisible$(): Observable<boolean> {
    return this._isVisible.asObservable();
  }

  get isVisible(): boolean {
    return this._isVisible.value;
  }

  private checkVisibility(): void {
    const hiddenUntil = localStorage.getItem('info-strip-hidden-until');
    if (hiddenUntil) {
      const hiddenUntilTimestamp = parseInt(hiddenUntil, 10);
      const currentTimestamp = Date.now();
      
      if (currentTimestamp < hiddenUntilTimestamp) {
        this._isVisible.next(false);
      } else {
        localStorage.removeItem('info-strip-hidden-until');
        this._isVisible.next(true);
      }
    } else {
      this._isVisible.next(true);
    }
  }

  hide(): void {
    const hideUntilTimestamp = Date.now() + this.HIDE_DURATION_MS;
    this._isVisible.next(false);
    localStorage.setItem('info-strip-hidden-until', hideUntilTimestamp.toString());
  }

  show(): void {
    this._isVisible.next(true);
    localStorage.removeItem('info-strip-hidden-until');
  }

}
