import { ChangeDetectionStrategy, Component, HostBinding, Input, OnDestroy, OnInit } from '@angular/core';
import { faCog as fadCog } from '@fortawesome/pro-duotone-svg-icons/faCog';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { merge, Subject } from 'rxjs';
import { filter, take, takeUntil, tap } from 'rxjs/operators';

// TODO remove this banner after a while - at sometime in 2023
@Component({
  selector: 'sb-transferred-students-banner',
  templateUrl: './transferred-students-banner.component.html',
  styleUrls: ['./transferred-students-banner.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TransferredStudentsBannerComponent implements OnInit, OnDestroy {
  @Input() small = false;

  readonly fadCog = fadCog;
  readonly fasTimes = fasTimes;

  readonly destroyed$ = new Subject<void>();

  @HostBinding('hidden')
  hidden = false;

  constructor(private localStorageService: LocalStorageService) {}

  ngOnInit() {
    this.hidden = this.localStorageService.getTransferredStudentsBannerHidden();

    // Hide this banner if the user changes the "bookTransferredHidden" setting.
    // We assume that since the user changed this setting, he/she knows
    // about it and does not need to be reminded anymore.
    this.localStorageService.bookTransferredHidden$
      .pipe(
        filter((bookTransferredHidden) => bookTransferredHidden),
        tap(() => this.hideBanner()),
        take(1),
        takeUntil(
          merge(
            // unsubscribe if the user hides the banner himself
            this.localStorageService.transferredStudentsBannerHidden$.pipe(
              filter((transferredStudentsBannerHidden) => transferredStudentsBannerHidden)
            ),
            this.destroyed$
          )
        )
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  hideBanner() {
    this.hidden = true;
    this.localStorageService.setTransferredStudentsBannerHidden(true);
  }
}
