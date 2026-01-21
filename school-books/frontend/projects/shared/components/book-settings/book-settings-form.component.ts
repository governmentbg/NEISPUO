import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { faInfoCircle as fadInfoCircle } from '@fortawesome/pro-duotone-svg-icons/faInfoCircle';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { BookSettingsInfoDialogComponent } from './book-settings-info-dialog.component';

@Component({
  selector: 'sb-book-settings-form',
  templateUrl: './book-settings-form.component.html'
})
export class BookSettingsFormComponent implements OnInit, OnDestroy {
  readonly destroyed$ = new Subject<void>();

  readonly fadInfoCircle = fadInfoCircle;

  form!: UntypedFormGroup;

  constructor(
    private fb: UntypedFormBuilder,
    private dialog: MatDialog,
    private localStorageService: LocalStorageService
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      hideTransferred: [this.localStorageService.getBookTransferredHidden()],
      hideNotEnrolled: [this.localStorageService.getBookNotEnrolledHidden()],
      hideGradeless: [this.localStorageService.getBookGradelessHidden()]
    });

    this.form
      .get('hideTransferred')
      ?.valueChanges.pipe(
        tap((value) => this.localStorageService.setBookTransferredHidden(value)),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.form
      .get('hideNotEnrolled')
      ?.valueChanges.pipe(
        tap((value) => this.localStorageService.setBookNotEnrolledHidden(value)),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.form
      .get('hideGradeless')
      ?.valueChanges.pipe(
        tap((value) => this.localStorageService.setBookGradelessHidden(value)),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  stopPropagation(e: Event) {
    e.stopPropagation();
  }

  showInfo() {
    openTypedDialog(this.dialog, BookSettingsInfoDialogComponent, {});
  }
}
