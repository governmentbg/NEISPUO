import { Component, OnDestroy } from '@angular/core';
import { Event, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'sb-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnDestroy {
  protected readonly destroyed$ = new Subject<void>();

  constructor(router: Router) {
    if (!environment.production) {
      router.events
        .pipe(
          tap((e: Event) => {
            const d = new Date();
            const timestamp = `${d.toLocaleDateString()} ${d.toLocaleTimeString()}`;
            // eslint-disable-next-line no-restricted-syntax
            console.debug(timestamp, e.toString(), e);
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
