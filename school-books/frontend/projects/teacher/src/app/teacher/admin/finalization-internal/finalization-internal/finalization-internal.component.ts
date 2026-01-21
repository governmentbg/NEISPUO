import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faExclamationCircle as fadExclamationCircle } from '@fortawesome/pro-duotone-svg-icons/faExclamationCircle';
import { faKey as fadKey } from '@fortawesome/pro-duotone-svg-icons/faKey';
import { faLock as fadLock } from '@fortawesome/pro-duotone-svg-icons/faLock';
import { faSpinnerThird as fadSpinnerThird } from '@fortawesome/pro-duotone-svg-icons/faSpinnerThird';
import { faFilePdf as fasFilePdf } from '@fortawesome/pro-solid-svg-icons/faFilePdf';
import { faKey as fasKey } from '@fortawesome/pro-solid-svg-icons/faKey';
import { faLock as fasLock } from '@fortawesome/pro-solid-svg-icons/faLock';
import {
  ClassBooksFinalizationService,
  ClassBooksFinalization_GetAllClassBooks
} from 'projects/sb-api-client/src/api/classBooksFinalization.service';
import { ClassBookPrintStatus } from 'projects/sb-api-client/src/model/classBookPrintStatus';
import { ClassBooksGetAllForFinalizationVO } from 'projects/sb-api-client/src/model/classBooksGetAllForFinalizationVO';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { getUnexpectedErrorMessage } from 'projects/shared/utils/error';
import { throwError as throwErr, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject, Subscription, timer } from 'rxjs';
import { switchMap, takeUntil, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Signer } from './signer';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class FinalizationInternalSkeletonComponent extends SkeletonComponentBase {
  constructor(classBooksFinalizationService: ClassBooksFinalizationService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.resolve(FinalizationInternalComponent, {
      schoolYear,
      instId,
      classBooks: classBooksFinalizationService.getAllClassBooks({
        schoolYear,
        instId
      }),
      signingServiceExists: Signer.signingServiceExists()
    });
  }
}

@Component({
  selector: 'sb-finalization-internal',
  templateUrl: './finalization-internal.component.html'
})
export class FinalizationInternalComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBooks: ClassBooksFinalization_GetAllClassBooks;
    signingServiceExists: boolean;
  };

  readonly CLASS_BOOK_PRINT_STATUS_PENDING = ClassBookPrintStatus.Pending;
  readonly CLASS_BOOK_PRINT_STATUS_PROCESSED = ClassBookPrintStatus.Processed;
  readonly CLASS_BOOK_PRINT_STATUS_ERRORED = ClassBookPrintStatus.Errored;

  private readonly destroyed$ = new Subject<void>();
  private readonly signer = new Signer();

  readonly fadExclamationCircle = fadExclamationCircle;
  readonly fadLock = fadLock;
  readonly fadKey = fadKey;
  readonly fadSpinnerThird = fadSpinnerThird;
  readonly fasKey = fasKey;
  readonly fasLock = fasLock;
  readonly fasFilePdf = fasFilePdf;
  readonly signingServicePage = environment.signingServerPageUrl;

  allClassBooksSelectedForFinalization!: boolean;
  allClassBooksSelectedForSigning!: boolean;
  form!: FormGroup<{
    classBooks: FormArray<
      FormGroup<{
        classBookId: FormControl<number>;
        selectedForFinalization: FormControl<boolean>;
        selectedForSigning: FormControl<boolean>;
      }>
    >;
  }>;
  finalizing = false;
  signing: Record<number, boolean> = {};
  signingError: Record<number, string> = {};
  pendingReload?: Subscription | null;

  signedTotal = 0;

  constructor(
    private fb: FormBuilder,
    private actionService: ActionService,
    private classBooksFinalizationService: ClassBooksFinalizationService
  ) {}

  ngOnInit() {
    this.form = this.fb.nonNullable.group({
      classBooks: this.fb.nonNullable.array(
        this.data.classBooks.map((cb, i) =>
          this.fb.group({
            classBookId: this.fb.nonNullable.control<number>(cb.classBookId),
            selectedForFinalization: this.fb.nonNullable.control<boolean>({
              value: false,
              disabled: cb.finalPrintStatus != null
            }),
            selectedForSigning: this.fb.nonNullable.control<boolean>({
              value: false,
              disabled:
                !this.data.signingServiceExists ||
                cb.finalPrintStatus !== ClassBookPrintStatus.Processed ||
                cb.finalPrintIsSigned === true
            })
          })
        )
      )
    });

    this.syncAllClassBooksSelected();

    this.form.valueChanges
      .pipe(
        tap(() => {
          this.syncAllClassBooksSelected();
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.updateSignedTotal();
    this.addPendingReloadIfRequired();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get classBooksFormArray() {
    return this.form.controls['classBooks'];
  }

  get canFinalize() {
    return this.classBooksFormArray.value.some((cb) => cb.selectedForFinalization === true);
  }

  get canSign() {
    return this.classBooksFormArray.value.some((cb) => cb.selectedForSigning === true);
  }

  shouldPreventLeave() {
    return this.data.classBooks.some((cb) => cb.finalPrintIsSigned !== true && this.signing[cb.classBookId])
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

    for (const cb of classBooks) {
      const classBookFormGroup = this.classBooksFormArray.controls.find(
        (p) => p.get('classBookId')?.value === cb.classBookId
      );
      if (!classBookFormGroup) {
        continue;
      }

      const canFinalize = cb.finalPrintStatus == null;
      const canSign = cb.finalPrintStatus === ClassBookPrintStatus.Processed && cb.finalPrintIsSigned !== true;

      if (canFinalize) {
        classBookFormGroup.get('selectedForFinalization')?.enable({ emitEvent: false });
      } else {
        classBookFormGroup.get('selectedForFinalization')?.disable({ emitEvent: false });
      }
      if (this.data.signingServiceExists && canSign) {
        classBookFormGroup.get('selectedForSigning')?.enable({ emitEvent: false });
      } else {
        classBookFormGroup.get('selectedForSigning')?.disable({ emitEvent: false });
      }
    }
  }

  addPendingReloadIfRequired() {
    if (this.pendingReload) {
      this.pendingReload.unsubscribe();
    }

    if (this.data.classBooks.some((p) => p.finalPrintStatus === ClassBookPrintStatus.Pending)) {
      this.pendingReload = timer(5000)
        .pipe(
          switchMap(() =>
            this.classBooksFinalizationService.getAllClassBooks({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId
            })
          ),
          tap((classBooks) => {
            this.updateClassBookList(classBooks);
            this.addPendingReloadIfRequired();
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();
    }
  }

  partialReload(classBookIds: number[] | null = null) {
    return this.classBooksFinalizationService
      .getAllClassBooks({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookIds
      })
      .toPromise()
      .then((classBooks) => {
        this.updateClassBookList(classBooks);
        this.addPendingReloadIfRequired();
      });
  }

  syncAllClassBooksSelected() {
    const classBooksForFinalization = this.classBooksFormArray.controls
      .map((c) => c.get('selectedForFinalization'))
      .filter((c) => c?.enabled);
    this.allClassBooksSelectedForFinalization =
      classBooksForFinalization.length > 0 &&
      classBooksForFinalization.length === classBooksForFinalization.filter((c) => c?.value === true).length;

    const classBooksForSigning = this.classBooksFormArray.controls
      .map((c) => c.get('selectedForSigning'))
      .filter((c) => c?.enabled);
    this.allClassBooksSelectedForSigning =
      classBooksForSigning.length > 0 &&
      classBooksForSigning.length === classBooksForSigning.filter((c) => c?.value === true).length;
  }

  toggleAllClassBooksSelectedForFinalization() {
    this.allClassBooksSelectedForFinalization = !this.allClassBooksSelectedForFinalization;

    for (const classBookControl of this.classBooksFormArray.controls) {
      const selectedForFinalizationControl = classBookControl.get('selectedForFinalization');

      if (!selectedForFinalizationControl?.enabled) {
        continue;
      }

      selectedForFinalizationControl.setValue(this.allClassBooksSelectedForFinalization, { emitEvent: false });
    }
  }

  toggleAllClassBooksSelectedForSigning() {
    this.allClassBooksSelectedForSigning = !this.allClassBooksSelectedForSigning;

    for (const classBookControl of this.classBooksFormArray.controls) {
      const selectedForSigningControl = classBookControl.get('selectedForSigning');

      if (!selectedForSigningControl?.enabled) {
        continue;
      }

      selectedForSigningControl.setValue(this.allClassBooksSelectedForSigning, { emitEvent: false });
    }
  }

  onFinalize() {
    this.finalizing = true;

    const classBookIds = this.classBooksFormArray.value
      .filter((cb) => cb.selectedForFinalization === true)
      .map((cb) => cb.classBookId ?? throwErr("'classBookId' should not be null"));

    const finalizeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      finalizeClassBooksCommand: {
        classBookIds
      }
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да приключите избраните дневници?',
        errorsMessage: 'Не може да приключите дневниците, защото:',
        httpAction: () => this.classBooksFinalizationService.finalizeClassBooks(finalizeParams).toPromise()
      })
      .then((success) => {
        if (!success) {
          return;
        }

        return this.partialReload(classBookIds);
      })
      .finally(() => {
        this.finalizing = false;
      });
  }

  onSign() {
    const classBookIds = this.classBooksFormArray.value
      .filter((cb) => cb.selectedForSigning === true)
      .map((cb) => cb.classBookId ?? throwErr("'classBookId' should not be null"));

    for (const cbId of classBookIds) {
      if (this.signing[cbId]) {
        continue;
      }

      const cb = this.data.classBooks.find((cb) => cb.classBookId === cbId);
      if (!cb) {
        throw new Error(`Class book with id ${cbId} not found`);
      }

      const blobDownloadUrl =
        cb.blobDownloadUrl ?? throwErr(`Class book with id ${cbId} does not have a blobDownloadUrl`);
      const classBookPrintId =
        cb.classBookPrintId ?? throwErr(`Class book with id ${cbId} does not have a classBookPrintId`);

      this.signing[cbId] = true;
      this.signer
        .downloadPdfBase64(blobDownloadUrl)
        .then((base64) => this.signer.signPdf(base64))
        .then((signedBase64) =>
          this.classBooksFinalizationService
            .signClassBookPrint({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: cbId,
              classBookPrintId,
              body: signedBase64
            })
            .toPromise()
        )
        .then(
          () => this.partialReload([cbId]),
          (error) => {
            if (error instanceof HttpErrorResponse) {
              if (error.status === 400 && error.error?.errorMessages?.length > 0) {
                this.signingError[cbId] = error.error.errorMessages.join('\n');
              } else {
                this.signingError[cbId] = getUnexpectedErrorMessage(error);
              }
            } else if (error instanceof Error) {
              this.signingError[cbId] = error.message;
            } else if (typeof error === 'string') {
              this.signingError[cbId] = error;
            } else {
              this.signingError[cbId] = getUnexpectedErrorMessage('unknown');
            }
          }
        )
        .finally(() => {
          this.signing[cbId] = false;
        });
    }
  }
}
