import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faExclamationCircle as fadExclamationCircle } from '@fortawesome/pro-duotone-svg-icons/faExclamationCircle';
import { faKey as fadKey } from '@fortawesome/pro-duotone-svg-icons/faKey';
import { faLock as fadLock } from '@fortawesome/pro-duotone-svg-icons/faLock';
import { faSpinnerThird as fadSpinnerThird } from '@fortawesome/pro-duotone-svg-icons/faSpinnerThird';
import { faCheckCircle as fasCheckCircle } from '@fortawesome/pro-solid-svg-icons/faCheckCircle';
import type { ErrorResponse, FileProgress, SuccessResponse, UppyFile } from '@uppy/core';
import { Locale, Uppy } from '@uppy/core';
// @ts-ignore: missing definitions, we can safely use any, as the structure is very simple
import * as BgLocale from '@uppy/locales/lib/bg_BG';
import XHRUpload from '@uppy/xhr-upload';
import { OAuthService } from 'angular-oauth2-oidc';
import {
  ClassBooksFinalizationService,
  ClassBooksFinalization_GetAllClassBooks
} from 'projects/sb-api-client/src/api/classBooksFinalization.service';
import { ClassBooksGetAllForFinalizationVO } from 'projects/sb-api-client/src/model/classBooksGetAllForFinalizationVO';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { getUnexpectedErrorMessage } from 'projects/shared/utils/error';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { environment } from 'src/environments/environment';

type UppyError = Error & { details?: string | undefined | null };
type UppyFileWithUniqueId = UppyFile & { uniqueId?: string };
@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class FinalizationExternalSkeletonComponent extends SkeletonComponentBase {
  constructor(classBooksFinalizationService: ClassBooksFinalizationService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.resolve(FinalizationExternalComponent, {
      schoolYear,
      instId,
      classBooks: classBooksFinalizationService.getAllClassBooks({
        schoolYear,
        instId
      })
    });
  }
}

@Component({
  selector: 'sb-finalization-external',
  templateUrl: './finalization-external.component.html'
})
export class FinalizationExternalComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBooks: ClassBooksFinalization_GetAllClassBooks;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fadExclamationCircle = fadExclamationCircle;
  readonly fadLock = fadLock;
  readonly fadKey = fadKey;
  readonly fadSpinnerThird = fadSpinnerThird;
  readonly fasCheckCircle = fasCheckCircle;
  readonly BgLocale: Locale = {
    strings: {
      ...BgLocale.strings,
      dropHereOr: 'За да прикачите подписани дневници ги преместете с мишката тук или %{browse}',
      browse: 'изберете чрез диалог'
    }
  };

  uppy: Uppy;
  uppyRestrictionFailedCallback: (file: UppyFile | undefined, error: UppyError) => void;
  uppyFileAddedCallback: (file: UppyFile) => void;
  uppyUploadProgressCallback: (file: UppyFile, progress: FileProgress) => void;
  uppyUploadSuccessCallback: (file: UppyFile, response: SuccessResponse) => void;
  uppyUploadErrorCallback: (file: UppyFile, error: UppyError, response: ErrorResponse | undefined) => void;

  files: {
    uniqueId: string;
    fileName: string;
    classBookName: string | null;
    error: string | null;
    pending: boolean;
    bytesUploaded: number;
    bytesTotal: number;
  }[] = [];

  signedTotal = 0;

  constructor(private eventService: EventService, route: ActivatedRoute, oauthService: OAuthService) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const apiBasePath = environment.apiBasePath.replace(/\/$/, '');

    this.uppy = new Uppy({
      id: Date.now().toString(),
      debug: !environment.production,
      autoProceed: true,
      restrictions: { maxFileSize: environment.publicationMaxFileSizeInBytes },
      locale: this.BgLocale
    });
    this.uppy.use(XHRUpload, {
      fieldName: 'SignedClassBookPrintFile',
      endpoint: `${apiBasePath}/api/classbooksfinalization/${schoolYear}/${instId}/finalizeclassbookwithsignedpdf`,
      headers: (file) => ({
        authorization: 'Bearer ' + oauthService.getAccessToken()
      })
    });

    this.uppyRestrictionFailedCallback = (file: UppyFile | undefined, error: Error) =>
      this.onUppyRestrictionFailed(file, error);
    this.uppyFileAddedCallback = (file: UppyFile) => this.onUppyFileAdded(file);
    this.uppyUploadProgressCallback = (file: UppyFile, progress: FileProgress) =>
      this.onUppyUploadProgress(file, progress);
    this.uppyUploadSuccessCallback = (file: UppyFile, response: SuccessResponse) =>
      this.onUppyUploadSuccess(file, response);
    this.uppyUploadErrorCallback = (file: UppyFile, error: UppyError, response: ErrorResponse | undefined) =>
      this.onUppyUploadError(file, error, response);
    this.uppy.on('restriction-failed', this.uppyRestrictionFailedCallback);
    this.uppy.on('file-added', this.uppyFileAddedCallback);
    this.uppy.on('upload-progress', this.uppyUploadProgressCallback);
    this.uppy.on('upload-success', this.uppyUploadSuccessCallback);
    this.uppy.on('upload-error', this.uppyUploadErrorCallback);
  }

  ngOnInit() {
    this.updateSignedTotal();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();

    this.uppy.off('restriction-failed', this.uppyRestrictionFailedCallback);
    this.uppy.off('file-added', this.uppyFileAddedCallback);
    this.uppy.off('upload-progress', this.uppyUploadProgressCallback);
    this.uppy.off('upload-success', this.uppyUploadSuccessCallback);
    this.uppy.off('upload-error', this.uppyUploadErrorCallback);
    this.uppy.close();
  }

  shouldPreventLeave() {
    return this.uppy.getState().totalProgress > 0
      ? 'Процесът по подписване/качване ще бъде прекъснат. Желаете ли да продължите?'
      : false;
  }

  updateSignedTotal() {
    this.signedTotal = this.data.classBooks.filter((cb) => cb.finalPrintIsSigned === true).length;
  }

  updateClassBookList(classBooks: ClassBooksGetAllForFinalizationVO[]) {
    // Update the data.classBooks array with the new data
    // only for the classBooks that are already in the array.
    // This effectively makes the array unaffected by the
    // addition/removal of classBooks by other users and allows
    // for partial updates.
    const map = new Map();
    this.data.classBooks.forEach((cb) => map.set(cb.classBookId, cb));
    classBooks.forEach((cb) => {
      if (map.has(cb.classBookId)) {
        map.set(cb.classBookId, cb);
      }
    });

    this.data.classBooks = Array.from(map.values());
    this.updateSignedTotal();
  }

  onUppyRestrictionFailed(file: UppyFile | undefined, error: UppyError) {
    this.eventService.dispatch({ type: EventType.SnackbarError, args: { message: error.message } });
  }

  onUppyFileAdded(file: UppyFile) {
    // set a uniqueId for the file as the 'id' is not unique and depends
    // entirely on the path, filename, size and type of the file
    // so droping the same file will always produce the same id
    const fileWithUniqueId = file as UppyFileWithUniqueId;
    fileWithUniqueId.uniqueId = file.id + new Date().getTime().toString();

    this.files.push({
      uniqueId: fileWithUniqueId.uniqueId,
      fileName: file.name,
      classBookName: null,
      error: null,
      pending: true,
      bytesUploaded: 0,
      bytesTotal: file.size
    });
  }

  onUppyUploadProgress(file: UppyFileWithUniqueId, progress: FileProgress) {
    const f = this.files.find((f) => f.uniqueId === file.uniqueId) ?? throwError('File not found');
    f.bytesUploaded = progress.bytesUploaded;
  }

  onUppyUploadSuccess(file: UppyFileWithUniqueId, response: SuccessResponse) {
    // remove the file from Uppy so that the file can be uploaded again
    this.uppy.removeFile(file.id);

    const classBook: ClassBooksGetAllForFinalizationVO = response.body;
    this.updateClassBookList([classBook]);

    const f = this.files.find((f) => f.uniqueId === file.uniqueId) ?? throwError('File not found');
    f.classBookName = classBook.fullBookName;
    f.pending = false;
  }

  onUppyUploadError(file: UppyFileWithUniqueId, error: UppyError, response: ErrorResponse | undefined) {
    // remove the file from Uppy so that the file can be uploaded again and the progressbar is hidden
    this.uppy.removeFile(file.id);

    let errorMessage: string | null = null;
    if (response) {
      if (response?.status === 400 && response.body?.errorMessages?.length > 0) {
        errorMessage = response.body.errorMessages.join('\n');
      } else {
        errorMessage = getUnexpectedErrorMessage('upload_error');
        GlobalErrorHandler.instance.handleError(response, true);
      }
    } else {
      if (error.details) {
        errorMessage = `${errorMessage} ${error.details}`;
      } else {
        errorMessage = error.message;
      }
    }

    const f = this.files.find((f) => f.uniqueId === file.uniqueId) ?? throwError('File not found');
    f.error = errorMessage;
    f.pending = false;
  }
}
