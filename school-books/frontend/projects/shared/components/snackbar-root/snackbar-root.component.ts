import { Component, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';

@Component({
  selector: 'sb-snackbar-root',
  templateUrl: './snackbar-root.component.html'
})
export class SnackbarRootComponent implements OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  constructor(matSnackBar: MatSnackBar, eventService: EventService) {
    eventService
      .on([EventType.SnackbarError, EventType.SnackbarWarning])
      .pipe(
        tap(({ type, args }) => {
          const { message } = args as { message?: string | null };
          if (message) {
            matSnackBar.open(message, 'OK', {
              duration: 30000, // 30sec
              verticalPosition: 'top',
              horizontalPosition: 'right',
              panelClass:
                type === EventType.SnackbarError
                  ? ['snackbar-error']
                  : type === EventType.SnackbarWarning
                  ? ['snackbar-warning']
                  : undefined
            });
          }
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
