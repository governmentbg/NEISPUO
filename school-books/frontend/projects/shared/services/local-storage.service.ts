import { Injectable } from '@angular/core';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { defer, Subject } from 'rxjs';
import { filter, map, startWith } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  private readonly BOOK_TRANSFERRED_HIDDEN_KEY = 'BOOK_TRANSFERRED_HIDDEN_KEY';
  private readonly BOOK_NOT_ENROLLED_HIDDEN_KEY = 'BOOK_NOT_ENROLLED_HIDDEN_KEY';
  private readonly BOOK_GRADELESS_HIDDEN_KEY = 'BOOK_GRADELESS_HIDDEN_KEY';
  private readonly TRANSFERRED_STUDENTS_BANNER_HIDDEN_KEY = 'TRANSFERRED_STUDENTS_BANNER_HIDDEN_KEY';
  private readonly ABSENCES_UNDO_BANNER_HIDDEN_KEY = 'ABSENCES_UNDO_BANNER_HIDDEN_KEY';
  private readonly ALLOW_SEND_GRADE_EMAILS_KEY = 'ALLOW_SEND_GRADE_EMAILS_KEY';
  private readonly ALLOW_SEND_ABSENCE_EMAILS_KEY = 'ALLOW_SEND_ABSENCE_EMAILS_KEY';
  private readonly ALLOW_SEND_REMARK_EMAILS_KEY = 'ALLOW_SEND_REMARK_EMAILS_KEY';
  private readonly MOBILE_APP_BANNER_HIDDEN_KEY = 'MOBILE_APP_BANNER_HIDDEN_KEY';
  private readonly DPLR_ABSENCES_BANNER_HIDDEN_KEY = 'DPLR_ABSENCES_BANNER_HIDDEN_KEY';

  private localStorage: Storage | null = null;
  private valueChanged$: Subject<[string, string | boolean]> = new Subject<[string, string | boolean]>();

  constructor() {
    try {
      this.localStorage = window.localStorage;
    } catch (e) {
      // access denied
      // pass error to GlobalErrorHandler, but do not show it (mark as presented)
      GlobalErrorHandler.instance.handleError(e, true);
    }
  }

  getBookTransferredHidden = () => this.getBooleanItem(this.BOOK_TRANSFERRED_HIDDEN_KEY) ?? false;
  setBookTransferredHidden = (value: boolean) => this.setItem(this.BOOK_TRANSFERRED_HIDDEN_KEY, value);
  bookTransferredHidden$ = defer(() =>
    this.valueChanged$.pipe(
      filter(([key, _]) => key === this.BOOK_TRANSFERRED_HIDDEN_KEY),
      map(([_, value]) => value as boolean),
      startWith(this.getBookTransferredHidden())
    )
  );

  getBookNotEnrolledHidden = () => this.getBooleanItem(this.BOOK_NOT_ENROLLED_HIDDEN_KEY) ?? true;
  setBookNotEnrolledHidden = (value: boolean) => this.setItem(this.BOOK_NOT_ENROLLED_HIDDEN_KEY, value);
  bookNotEnrolledHidden$ = defer(() =>
    this.valueChanged$.pipe(
      filter(([key, _]) => key === this.BOOK_NOT_ENROLLED_HIDDEN_KEY),
      map(([_, value]) => value as boolean),
      startWith(this.getBookNotEnrolledHidden())
    )
  );

  getBookGradelessHidden = () => this.getBooleanItem(this.BOOK_GRADELESS_HIDDEN_KEY) ?? false;
  setBookGradelessHidden = (value: boolean) => this.setItem(this.BOOK_GRADELESS_HIDDEN_KEY, value);
  bookGradelessHidden$ = defer(() =>
    this.valueChanged$.pipe(
      filter(([key, _]) => key === this.BOOK_GRADELESS_HIDDEN_KEY),
      map(([_, value]) => value as boolean),
      startWith(this.getBookGradelessHidden())
    )
  );

  getTransferredStudentsBannerHidden = () => this.getBooleanItem(this.TRANSFERRED_STUDENTS_BANNER_HIDDEN_KEY) ?? false;
  setTransferredStudentsBannerHidden = (value: boolean) =>
    this.setItem(this.TRANSFERRED_STUDENTS_BANNER_HIDDEN_KEY, value);
  transferredStudentsBannerHidden$ = defer(() =>
    this.valueChanged$.pipe(
      filter(([key, _]) => key === this.TRANSFERRED_STUDENTS_BANNER_HIDDEN_KEY),
      map(([_, value]) => value as boolean),
      startWith(this.getTransferredStudentsBannerHidden())
    )
  );

  getAbsencesUndoBannerHidden = () => this.getBooleanItem(this.ABSENCES_UNDO_BANNER_HIDDEN_KEY) ?? false;
  setAbsencesUndoBannerHidden = (value: boolean) => this.setItem(this.ABSENCES_UNDO_BANNER_HIDDEN_KEY, value);
  absencesUndoBannerHidden$ = defer(() =>
    this.valueChanged$.pipe(
      filter(([key, _]) => key === this.ABSENCES_UNDO_BANNER_HIDDEN_KEY),
      map(([_, value]) => value as boolean),
      startWith(this.getAbsencesUndoBannerHidden())
    )
  );

  getMobileAppBannerHidden = () => this.getBooleanItem(this.MOBILE_APP_BANNER_HIDDEN_KEY) ?? false;
  setMobileAppBannerHidden = (value: boolean) => this.setItem(this.MOBILE_APP_BANNER_HIDDEN_KEY, value);
  mobileAppBannerHidden$ = defer(() =>
    this.valueChanged$.pipe(
      filter(([key, _]) => key === this.MOBILE_APP_BANNER_HIDDEN_KEY),
      map(([_, value]) => value as boolean),
      startWith(this.getMobileAppBannerHidden())
    )
  );

  getDplrAbsencesBannerHidden = () => this.getBooleanItem(this.DPLR_ABSENCES_BANNER_HIDDEN_KEY) ?? false;
  setDplrAbsencesBannerHidden = (value: boolean) => this.setItem(this.DPLR_ABSENCES_BANNER_HIDDEN_KEY, value);
  dplrAbsencesBannerHidden$ = defer(() =>
    this.valueChanged$.pipe(
      filter(([key, _]) => key === this.DPLR_ABSENCES_BANNER_HIDDEN_KEY),
      map(([_, value]) => value as boolean),
      startWith(this.getDplrAbsencesBannerHidden())
    )
  );

  private setItem(key: string, value: string | boolean): void {
    this.valueChanged$.next([key, value]);

    if (!this.localStorage) {
      return;
    }

    try {
      if (typeof value === 'boolean') {
        this.localStorage.setItem(key, value.toString());
      } else {
        this.localStorage.setItem(key, value);
      }
    } catch (e) {
      // storage full
      // pass error to GlobalErrorHandler, but do not show it (mark as presented)
      GlobalErrorHandler.instance.handleError(e, true);
    }
  }

  private getItem(key: string): string | null {
    if (!this.localStorage) {
      return null;
    }

    return this.localStorage.getItem(key);
  }

  private getBooleanItem(key: string): boolean | null {
    if (!this.localStorage) {
      return null;
    }

    const value = this.localStorage.getItem(key);

    if (value === true.toString()) {
      return true;
    } else if (value === false.toString()) {
      return false;
    } else {
      return null;
    }
  }
}
