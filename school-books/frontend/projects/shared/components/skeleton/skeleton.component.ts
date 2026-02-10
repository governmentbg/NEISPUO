import { ComponentRef, inject, Type } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ICanDeactivateComponent } from 'projects/shared/guards/deactivate-guard';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import {
  ConfirmDialogComponent,
  ConfirmDialogResult
} from 'projects/shared/services/action-service/confirm-dialog/confirm-dialog.component';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { getUnexpectedErrorMessage } from 'projects/shared/utils/error';
import { forkJoin, Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';

export const SIMPLE_SKELETON_TEMPLATE =
  '<sb-simple-skeleton-template (componentCreated)="onComponentCreated($event)" [component]="component" [inputs]="inputs" [ready]="ready" [error]="error" [errorMessage]="errorMessage"></sb-simple-skeleton-template>';

export const SIMPLE_PAGE_SKELETON_TEMPLATE =
  '<sb-simple-page-skeleton-template (componentCreated)="onComponentCreated($event)" [component]="component" [inputs]="inputs" [ready]="ready" [error]="error" [errorMessage]="errorMessage"></sb-simple-page-skeleton-template>';

export const SIMPLE_TAB_SKELETON_TEMPLATE =
  '<sb-simple-tab-skeleton-template (componentCreated)="onComponentCreated($event)" [component]="component" [inputs]="inputs" [ready]="ready" [error]="error" [errorMessage]="errorMessage"></sb-simple-tab-skeleton-template>';

export const SIMPLE_DIALOG_SKELETON_TEMPLATE =
  '<sb-simple-dialog-skeleton-template [component]="component" [inputs]="inputs" [ready]="ready" [error]="error" [errorMessage]="errorMessage"></sb-simple-dialog-skeleton-template>';

export const APP_CHROME_SKELETON_TEMPLATE =
  '<sb-app-chrome-skeleton-template [component]="component" [inputs]="inputs" [ready]="ready" [error]="error" [errorMessage]="errorMessage"></sb-app-chrome-skeleton-template>';

type DataType<T> = T extends { data: infer TData } ? TData : never;

type ObservableDictionary<T> = {
  [K in keyof T]: Observable<T[K]> | T[K];
};

export interface IShouldPreventLeave {
  shouldPreventLeave: () => boolean | string;
}

export abstract class SkeletonComponentBase implements ICanDeactivateComponent {
  private readonly DEFAULT_DIALOG_MESSAGE = 'Имате незапазени промени. Сигурни ли сте, че искате да напуснете екрана?';
  private dialog = inject(MatDialog);

  component!: any;
  inputs!: any;
  ready = false;
  error = false;
  leaveAlreadyConfirmed = true; //workaround to show only one message - when there are redirects, canDeactivate is triggered multiple times
  errorMessage?: string | null;
  componentRef: ComponentRef<any> | null | undefined;

  onComponentCreated(compRef: ComponentRef<any>) {
    this.componentRef = compRef;
  }

  resolve<TComponent extends { data: any }>(
    comp: Type<TComponent>,
    data: Observable<DataType<TComponent>> | ObservableDictionary<DataType<TComponent>>
  ) {
    if (data == null) {
      throw new Error('No data object provided');
    }

    let result$: Observable<TComponent>;
    if (data instanceof Observable) {
      result$ = data;
    } else {
      const obsDataEntries = Object.entries(data).filter(([k, v]) => v instanceof Observable);
      if (!obsDataEntries.length) {
        // synchronous path
        this.inputs = { data };
        this.component = comp;
        this.ready = true;
        return;
      }

      const mapedData = Object.fromEntries(
        // pipe each observable in the data object through a take(1)
        // as forkJoin completes when the underlying observables complete
        // but we are interested in the first values only
        obsDataEntries.map(([k, v]) => [k, (v as Observable<any>).pipe(take(1))])
      );

      result$ = forkJoin(mapedData)
        // merge the observable properties with the rest in the data object
        .pipe(map((obsData) => Object.assign(data, obsData)));
    }
    result$.pipe(take(1)).subscribe(
      (data) => {
        this.inputs = { data };
        this.component = comp;
        this.ready = true;
      },
      (err) => {
        GlobalErrorHandler.instance.handleError(err, true);

        this.ready = true;
        this.error = true;
        this.errorMessage = getUnexpectedErrorMessage(err);
      }
    );
  }

  canDeactivate() {
    if (this.componentRef?.instance == null || typeof this.componentRef.instance.shouldPreventLeave !== 'function') {
      return true;
    } else {
      const preventLeaveResponse = this.componentRef.instance.shouldPreventLeave();
      const showMessage =
        typeof preventLeaveResponse === 'string' || (typeof preventLeaveResponse === 'boolean' && preventLeaveResponse);
      const message = typeof preventLeaveResponse === 'string' ? preventLeaveResponse : this.DEFAULT_DIALOG_MESSAGE;
      if (showMessage && this.leaveAlreadyConfirmed) {
        return openTypedDialog(this.dialog, ConfirmDialogComponent, {
          data: { message: message }
        })
          .afterClosed()
          .toPromise()
          .then((result) => {
            this.leaveAlreadyConfirmed = result !== ConfirmDialogResult.Ok;
            return result === ConfirmDialogResult.Ok;
          });
      }
      return true;
    }
  }
}
