import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { getRequestId, getUnexpectedErrorMessage } from 'projects/shared/utils/error';
import { ConfirmDialogComponent, ConfirmDialogResult } from './confirm-dialog/confirm-dialog.component';
import { ErrorsDialogComponent } from './errors-dialog/errors-dialog.component';

@Injectable()
export class ActionService {
  constructor(private dialog: MatDialog) {}

  execute(options: {
    confirmMessage?: string | null;
    errorsMessage?: string;
    httpAction: () => Promise<unknown>;
  }): Promise<boolean> {
    let confirmPromise: Promise<boolean>;
    if (options.confirmMessage) {
      confirmPromise = openTypedDialog(this.dialog, ConfirmDialogComponent, {
        data: { message: options.confirmMessage }
      })
        .afterClosed()
        .toPromise()
        .then((result) => result === ConfirmDialogResult.Ok)
        .catch((err) => {
          GlobalErrorHandler.instance.handleError(err);
          return false;
        });
    } else {
      confirmPromise = Promise.resolve(true);
    }

    return confirmPromise.then((confirmed) => {
      if (!confirmed) {
        return Promise.resolve(false); // the user canceled the action
      }

      return options.httpAction().then(
        () => true, // success
        (resp: HttpErrorResponse) => {
          if (resp.status === 400 && resp.error?.errorMessages?.length > 0) {
            openTypedDialog(this.dialog, ErrorsDialogComponent, {
              data: {
                header: options.errorsMessage || 'Възникнаха следните валидационни грешки:',
                errorMessages: resp.error.errorMessages
              }
            });
          } else if (resp.status === 403) {
            const requestId = getRequestId(resp);
            openTypedDialog(this.dialog, ConfirmDialogComponent, {
              data: {
                message: requestId
                  ? `Нямате права да извършите действието. Номер на грешка: ${requestId}`
                  : 'Нямате права да извършите действието.',
                okBtnHidden: true,
                cancelBtnText: 'OK'
              }
            });
          } else if (resp.status === 508) {
            const requestId = getRequestId(resp);
            openTypedDialog(this.dialog, ConfirmDialogComponent, {
              data: {
                message:
                  'Възникна грешка при обработката на заявката поради високо натоварване на системата.\n' +
                  'Моля, опитайте отново по-късно или в по-ненатоварен час.\n' +
                  'Ако проблемът продължава, моля подайте заявка в системата за поддръжка\n' +
                  `${requestId ? 'Номер на грешка: ' + requestId : ''}`,
                okBtnHidden: true,
                cancelBtnText: 'OK'
              }
            });
          } else {
            const errorMessage = getUnexpectedErrorMessage(resp);

            openTypedDialog(this.dialog, ConfirmDialogComponent, {
              data: {
                message: errorMessage,
                okBtnHidden: true,
                cancelBtnText: 'OK'
              }
            });

            GlobalErrorHandler.instance.handleError(resp, true);
          }

          return false; // an error ocurred
        }
      );
    });
  }
}
