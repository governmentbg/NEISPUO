import { Component, Inject, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import type { ErrorResponse, SuccessResponse, UppyFile } from '@uppy/core';
import { Locale, Uppy } from '@uppy/core';
// @ts-ignore: missing definitions, we can safely use any, as the structure is very simple
import * as BgLocale from '@uppy/locales/lib/bg_BG';
import XHRUpload from '@uppy/xhr-upload';
import { OAuthService } from 'angular-oauth2-oidc';
import { TopicPlanItemsExcelService } from 'projects/sb-api-client/src/api/topicPlanItemsExcel.service';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { TypedDialog } from 'projects/shared/utils/dialog';
import { Subject } from 'rxjs';
import { environment } from 'src/environments/environment';

export type TopicPlanViewImportDialogData = {
  schoolYear: number;
  instId: number;
  topicPlanId: number;
};

type File = {
  blobId: number;
  fileName: string;
  location: string;
};

type UppyError = Error & { details?: string | undefined | null };

@Component({
  selector: 'sb-topic-plan-view-import-dialog',
  templateUrl: './topic-plan-view-import-dialog.component.html'
})
export class TopicPlanViewImportDialogComponent implements TypedDialog<TopicPlanViewImportDialogData, void>, OnDestroy {
  d!: TopicPlanViewImportDialogData;
  r!: void;

  private readonly destroyed$ = new Subject<void>();

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;
  readonly templateUrl = 'assets/static/topic_plan_template_v1.xlsx';
  fasFileExcel = fasFileExcel;
  readonly BgLocale: Locale = {
    strings: {
      ...BgLocale.strings,
      dropHereOr: 'За да прикачите файл го преместете с мишката тук или %{browse}',
      browse: 'изберете чрез диалог'
    }
  };

  file: File | null = null;

  saving = false;

  uppy: Uppy;
  uppyRestrictionFailed: (file: UppyFile | undefined, error: UppyError) => void;
  uppyUploadSuccessCallback: (file: UppyFile, response: SuccessResponse) => void;
  uppyUploadErrorCallback: (file: UppyFile, error: UppyError, response: ErrorResponse | undefined) => void;
  uppyErrorCallback: (error: UppyError) => void;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: TopicPlanViewImportDialogData,
    private topicPlanItemsExcelService: TopicPlanItemsExcelService,
    private actionService: ActionService,
    private dialogRef: MatDialogRef<TopicPlanViewImportDialogComponent>,
    private eventService: EventService,
    oauthService: OAuthService
  ) {
    this.uppy = new Uppy({
      id: Date.now().toString(),
      debug: !environment.production,
      autoProceed: true,
      restrictions: {
        maxFileSize: environment.topicPlanImportMaxFileSizeInBytes,
        maxNumberOfFiles: 1,
        allowedFileTypes: ['.xlsx']
      },
      locale: this.BgLocale
    });

    this.uppy.use(XHRUpload, {
      endpoint: environment.blobServerPath,
      headers: (file) => ({
        authorization: 'Bearer ' + oauthService.getAccessToken()
      })
    });

    this.uppyRestrictionFailed = (file: UppyFile | undefined, error: Error) =>
      this.onUppyRestrictionFailed(file, error);
    this.uppyUploadSuccessCallback = (file: UppyFile, response: SuccessResponse) =>
      this.onUppyUploadSuccess(file, response);
    this.uppyUploadErrorCallback = (file: UppyFile, error: UppyError, response: ErrorResponse | undefined) =>
      this.onUppyUploadError(file, error, response);
    this.uppyErrorCallback = (error: UppyError) => this.onUppyError(error);
    this.uppy.on('restriction-failed', this.uppyRestrictionFailed);
    this.uppy.on('upload-success', this.uppyUploadSuccessCallback);
    this.uppy.on('upload-error', this.uppyUploadErrorCallback);
    this.uppy.on('error', this.uppyErrorCallback);
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();

    this.uppy.off('restriction-failed', this.uppyRestrictionFailed);
    this.uppy.off('upload-success', this.uppyUploadSuccessCallback);
    this.uppy.off('upload-error', this.uppyUploadErrorCallback);
    this.uppy.off('error', this.uppyErrorCallback);
    this.uppy.close();
  }

  onSave() {
    // stop any uploads in progress
    this.uppy.cancelAll();

    if (this.saving || this.file == null) {
      return;
    }

    this.saving = true;

    this.actionService
      .execute({
        httpAction: () => {
          return this.topicPlanItemsExcelService
            .importFromExcelFile({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              topicPlanId: this.data.topicPlanId,
              blobId: this.file!.blobId
            })
            .toPromise()
            .then(() => {
              this.dialogRef.close(true);
            });
        }
      })
      .finally(() => {
        this.saving = false;
      });
  }

  onRemoveFile(blobId: number) {
    this.file = null;
  }

  onUppyRestrictionFailed(file: UppyFile | undefined, error: UppyError) {
    this.showError(error.message);
  }

  onUppyUploadSuccess(file: UppyFile, response: SuccessResponse) {
    this.file = { blobId: response.body.blobId, fileName: response.body.name, location: response.body.location };

    // remove the file from Uppy so that the file can be uploaded again
    this.uppy.removeFile(file.id);
  }

  onUppyUploadError(file: UppyFile, error: UppyError, response: ErrorResponse | undefined) {
    // remove the file from Uppy so that the file can be uploaded again and the progressbar is hidden
    this.uppy.removeFile(file.id);

    // the error will be shown to the user in the onUppyError callback
  }

  onUppyError(error: UppyError) {
    this.showError(`${error.message} ${error.details}`);
  }

  showError(errorMessage: string) {
    this.eventService.dispatch({ type: EventType.SnackbarError, args: { message: errorMessage } });
  }
}
