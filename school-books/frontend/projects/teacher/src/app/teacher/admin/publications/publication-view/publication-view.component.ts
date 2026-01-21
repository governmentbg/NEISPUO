import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faArchive as fasArchive } from '@fortawesome/pro-solid-svg-icons/faArchive';
import { faArrowAltCircleDown as fasArrowAltCircleDown } from '@fortawesome/pro-solid-svg-icons/faArrowAltCircleDown';
import { faArrowAltCircleUp as fasArrowAltCircleUp } from '@fortawesome/pro-solid-svg-icons/faArrowAltCircleUp';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faFile as fasFile } from '@fortawesome/pro-solid-svg-icons/faFile';
import { faInfoSquare as fadInfoSquare } from '@fortawesome/pro-solid-svg-icons/faInfoSquare';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import type { ErrorResponse, SuccessResponse, UppyFile } from '@uppy/core';
import { Locale, Uppy } from '@uppy/core';
// @ts-ignore: missing definitions, we can safely use any, as the structure is very simple
import * as BgLocale from '@uppy/locales/lib/bg_BG';
import XHRUpload from '@uppy/xhr-upload';
import { OAuthService } from 'angular-oauth2-oidc';
import { PublicationsService, Publications_Get } from 'projects/sb-api-client/src/api/publications.service';
import { PublicationStatusNomsService } from 'projects/sb-api-client/src/api/publicationStatusNoms.service';
import { PublicationTypeNomsService } from 'projects/sb-api-client/src/api/publicationTypeNoms.service';
import { PublicationsGetVOFile } from 'projects/sb-api-client/src/model/publicationsGetVOFile';
import { PublicationStatus } from 'projects/sb-api-client/src/model/publicationStatus';
import { PublicationType } from 'projects/sb-api-client/src/model/publicationType';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { environment } from 'src/environments/environment';

type UppyError = Error & { details?: string | undefined | null };

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class PublicationViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    publicationsService: PublicationsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const publicationId = tryParseInt(route.snapshot.paramMap.get('publicationId'));

    if (publicationId) {
      this.resolve(PublicationViewComponent, {
        schoolYear,
        instId,
        publication: publicationsService.get({
          schoolYear,
          instId,
          publicationId: publicationId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(PublicationViewComponent, {
        schoolYear,
        instId,
        publication: null,
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

@Component({
  selector: 'sb-publication-view',
  templateUrl: './publication-view.component.html',
  styleUrls: ['./publication-view.component.scss']
})
export class PublicationViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    publication: Publications_Get | null;
    institutionInfo: InstitutionInfoType;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fadInfoSquare = fadInfoSquare;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasArrowLeft = fasArrowLeft;
  readonly fasArchive = fasArchive;
  readonly fasArrowAltCircleUp = fasArrowAltCircleUp;
  readonly fasArrowAltCircleDown = fasArrowAltCircleDown;
  readonly fasPlus = fasPlus;
  readonly fasFile = fasFile;
  readonly BgLocale: Locale = {
    strings: {
      ...BgLocale.strings,
      dropHereOr: 'За да прикачите нови файлове ги преместете с мишката тук или %{browse}',
      browse: 'изберете чрез диалог'
    }
  };

  readonly form = this.fb.group({
    type: [null, Validators.required],
    status: [PublicationStatus.Draft, Validators.required],
    date: [new Date(), Validators.required],
    title: [null, Validators.required],
    content: [null, Validators.required]
  });

  files: PublicationsGetVOFile[] = [];

  removing = false;
  statusChanging = false;
  editable = false;
  publicationStatus = PublicationStatus;
  canEdit = false;
  canRemove = false;
  showTitleHint = false;
  titleHintChars = 50;

  publicationTypeNomsService: INomService<PublicationType, { instId: number; schoolYear: number }>;
  publicationStatusNomsService: INomService<PublicationStatus, { instId: number; schoolYear: number }>;

  uppy: Uppy;
  uppyRestrictionFailed: (file: UppyFile | undefined, error: UppyError) => void;
  uppyUploadSuccessCallback: (file: UppyFile, response: SuccessResponse) => void;
  uppyUploadErrorCallback: (file: UppyFile, error: UppyError, response: ErrorResponse | undefined) => void;
  uppyErrorCallback: (error: UppyError) => void;

  constructor(
    private fb: UntypedFormBuilder,
    private publicationsService: PublicationsService,
    publicationTypeNomsService: PublicationTypeNomsService,
    publicationStatusNomsService: PublicationStatusNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private eventService: EventService,
    oauthService: OAuthService
  ) {
    this.publicationTypeNomsService = new NomServiceWithParams(publicationTypeNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.publicationStatusNomsService = new NomServiceWithParams(publicationStatusNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));

    this.uppy = new Uppy({
      id: Date.now().toString(),
      debug: !environment.production,
      autoProceed: true,
      restrictions: { maxFileSize: environment.publicationMaxFileSizeInBytes },
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

  shouldPreventLeave() {
    if (this.form.dirty) {
      return true;
    } else if (this.uppy.getState().totalProgress > 0) {
      return 'Процесът по качване на файлове ще бъде прекъснат. Сигурни ли сте, че искате да напуснете екрана?';
    } else {
      return false;
    }
  }

  ngOnInit() {
    if (this.data.publication != null) {
      this.form.setValue({
        type: this.data.publication.type,
        status: this.data.publication.status,
        date: this.data.publication.date,
        title: this.data.publication.title,
        content: this.data.publication.content
      });
      this.files = this.data.publication.files;
      this.showTitleHint = this.data.publication.title.length > this.titleHintChars;
    }

    this.form.controls['status'].disable();

    this.form.controls['title'].valueChanges.pipe(takeUntil(this.destroyed$)).subscribe((val) => {
      const title: string | null | undefined = val;
      this.showTitleHint = !!title && title.length > this.titleHintChars;
    });

    this.canEdit =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasPublicationEditAccess;
    this.canRemove =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasPublicationRemoveAccess;
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

  onEditableChange(editable: boolean) {
    this.editable = editable;

    if (editable || !this.data.publication) {
      return;
    }

    this.publicationsService
      .get({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        publicationId: this.data.publication.publicationId
      })
      .toPromise()
      .then((p) => {
        this.data.publication = p;
        this.files = p.files;
        this.showTitleHint = p.title.length > this.titleHintChars;
      });
  }

  onSave(save: SaveToken) {
    const value = this.form.value;

    // stop any uploads in progress
    this.uppy.cancelAll();

    const publication = {
      type: <PublicationType>value.type,
      date: <Date>value.date,
      title: value.title,
      content: value.content,
      files: this.files
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.publication == null) {
            return this.publicationsService
              .create({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createPublicationCommand: publication
              })
              .toPromise()
              .then((newPublicationId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newPublicationId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              publicationId: this.data.publication.publicationId
            };
            return this.publicationsService
              .update({
                updatePublicationCommand: publication,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.publicationsService.get(updateArgs).toPromise())
              .then((newPublication) => {
                this.data.publication = newPublication;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onChangeStatus(newStatus: PublicationStatus) {
    if (!this.data.publication) {
      throw new Error('onChangeStatus requires a publication to have been loaded.');
    }

    this.statusChanging = true;
    const changeStatusParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      publicationId: this.data.publication.publicationId
    };

    this.actionService
      .execute({
        httpAction: () =>
          this.publicationsService
            .changeStatus({
              changePublicationStatusCommand: { status: newStatus },
              ...changeStatusParams
            })
            .toPromise()
            .then(() => this.publicationsService.get(changeStatusParams).toPromise())
            .then((newPublication) => {
              this.form.get('status')?.setValue(newPublication.status);
              this.data.publication = newPublication;
            })
      })
      .finally(() => {
        this.statusChanging = false;
      });
  }

  onRemove() {
    if (!this.data.publication) {
      throw new Error('onRemove requires a publication to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      publicationId: this.data.publication.publicationId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете публикацията?',
        errorsMessage: 'Не може да изтриете публикацията, защото:',
        httpAction: () => this.publicationsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }

  onRemoveFile(blobId: number) {
    this.files.splice(
      this.files.findIndex((f) => f.blobId === blobId),
      1
    );
  }

  onUppyRestrictionFailed(file: UppyFile | undefined, error: UppyError) {
    this.showError(error.message);
  }

  onUppyUploadSuccess(file: UppyFile, response: SuccessResponse) {
    this.files.push({ blobId: response.body.blobId, fileName: response.body.name, location: response.body.location });

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

  showError(message: string) {
    this.eventService.dispatch({ type: EventType.SnackbarError, args: { message } });
  }
}
