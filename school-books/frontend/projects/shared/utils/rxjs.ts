import { fromEvent, MonoTypeOperatorFunction, Observable, of } from 'rxjs';
import { debounce, filter } from 'rxjs/operators';

export function debounceUntilVisible<T>(): MonoTypeOperatorFunction<T> {
  const hasVisibilityApi = typeof document.hidden !== 'undefined';

  const pageVisible$ = fromEvent(document, 'visibilitychange').pipe(
    filter(() => document.visibilityState === 'visible')
  );

  return debounce((_) => (hasVisibilityApi && document.hidden ? pageVisible$ : of(true)));
}

/**
 * If dueDate has passed emits a single 'true' value and completes.
 * If dueDate has not passed emits 'false' and then, when dueDate passes emits 'true' and completes.
 */
export function expiredAt(dueDate: Date): Observable<boolean> {
  return new Observable((subscriber) => {
    let timeoutId: ReturnType<typeof setTimeout> | null = null;
    const timeLeft = dueDate.getTime() - Date.now();
    const isExpired = timeLeft <= 0;

    subscriber.next(isExpired);

    if (!isExpired) {
      timeoutId = setTimeout(() => {
        subscriber.next(true);
        subscriber.complete();
      }, timeLeft);
    } else {
      subscriber.complete();
    }

    return function unsubscribe() {
      if (timeoutId != null) {
        clearTimeout(timeoutId);
      }
    };
  });
}
