import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { filter } from 'rxjs/operators';

export enum EventType {
  ClassBooksUpdated = 1,
  SchoolYearSettingsUpdated = 2,
  SnackbarError = 3,
  SnackbarWarning = 4,
  ConversationRead = 5
}

export type Event = {
  type: EventType;
  args?: unknown;
};

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly eventSubject$ = new Subject<Event>();

  on(eventType: EventType | EventType[]): Observable<Event> {
    if (Array.isArray(eventType)) {
      return this.eventSubject$.pipe(filter((e) => eventType.indexOf(e.type) !== -1));
    }
    return this.eventSubject$.pipe(filter((e) => e.type === eventType));
  }

  dispatch(event: Event): void {
    this.eventSubject$.next(event);
  }
}
