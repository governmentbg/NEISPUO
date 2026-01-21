import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarStar as fadCalendarStar } from '@fortawesome/pro-duotone-svg-icons/faCalendarStar';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { BasicClassNomsService } from 'projects/sb-api-client/src/api/basicClassNoms.service';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import {
  SchoolYearSettingsService,
  SchoolYearSettings_Get
} from 'projects/sb-api-client/src/api/schoolYearSettings.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class SchoolYearSettingsViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    schoolYearSettingsService: SchoolYearSettingsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const schoolYearSettingsId = tryParseInt(route.snapshot.paramMap.get('schoolYearSettingsId'));
    if (schoolYearSettingsId) {
      this.resolve(SchoolYearSettingsViewComponent, {
        schoolYear,
        instId,
        schoolYearName: `${schoolYear} - ${schoolYear + 1}`,
        schoolYearSettings: schoolYearSettingsService.get({
          schoolYear,
          instId,
          schoolYearSettingsId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(SchoolYearSettingsViewComponent, {
        schoolYear,
        instId,
        schoolYearName: `${schoolYear} - ${schoolYear + 1}`,
        schoolYearSettings: null,
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

enum AppliesToValues {
  All = 'All',
  BasicClass = 'BasicClass',
  ClassBook = 'ClassBook'
}

@Component({
  selector: 'sb-school-year-settings-view',
  templateUrl: './school-year-settings-view.component.html'
})
export class SchoolYearSettingsViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    schoolYearName: string;
    schoolYearSettings: SchoolYearSettings_Get | null;
    institutionInfo: InstitutionInfoType;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fadCalendarStar = fadCalendarStar;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly AppliesToValues_All = AppliesToValues.All;
  readonly AppliesToValues_BasicClass = AppliesToValues.BasicClass;
  readonly AppliesToValues_ClassBook = AppliesToValues.ClassBook;
  canEdit = false;
  canRemove = false;

  readonly form = this.fb.nonNullable.group({
    schoolYearStartDate: this.fb.nonNullable.control<Date | null | undefined>(null),
    firstTermEndDate: this.fb.nonNullable.control<Date | null | undefined>(null),
    secondTermStartDate: this.fb.nonNullable.control<Date | null | undefined>(null),
    schoolYearEndDate: this.fb.nonNullable.control<Date | null | undefined>(null),
    description: this.fb.nonNullable.control<string | null | undefined>(null, Validators.required),
    hasFutureEntryLock: this.fb.nonNullable.control<boolean>(false, Validators.required),
    hasPastMonthLock: this.fb.nonNullable.control<boolean>(false, Validators.required),
    pastMonthLockDay: this.fb.nonNullable.control<number | null | undefined>(null, [
      Validators.required,
      Validators.min(1),
      Validators.max(31)
    ]),
    appliesTo: this.fb.nonNullable.control<AppliesToValues>(AppliesToValues.All, Validators.required),
    basicClassIds: this.fb.nonNullable.control<number[]>([], Validators.required),
    classBookIds: this.fb.nonNullable.control<number[]>([], Validators.required)
  });

  get showPGHint() {
    const { appliesTo, basicClassIds } = this.form.getRawValue();
    return appliesTo === AppliesToValues.BasicClass && basicClassIds.some((id) => id < 0 || id === 21 || id === 32);
  }

  basicClassNomsService: INomService<number, { instId: number; schoolYear: number }>;
  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removing = false;

  constructor(
    private fb: FormBuilder,
    private schoolYearSettingsService: SchoolYearSettingsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private eventService: EventService,
    basicClassNomsService: BasicClassNomsService,
    classBookNomsService: ClassBookNomsService
  ) {
    this.basicClassNomsService = new NomServiceWithParams(basicClassNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
    this.classBookNomsService = new NomServiceWithParams(classBookNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId
    }));
  }

  ngOnInit() {
    if (this.data.schoolYearSettings != null) {
      const {
        schoolYearStartDate,
        firstTermEndDate,
        secondTermStartDate,
        schoolYearEndDate,
        description,
        hasFutureEntryLock,
        pastMonthLockDay,
        isForAllClasses,
        basicClassIds,
        classBookIds
      } = this.data.schoolYearSettings;

      this.form.setValue({
        schoolYearStartDate,
        firstTermEndDate,
        secondTermStartDate,
        schoolYearEndDate,
        description,
        hasFutureEntryLock,
        hasPastMonthLock: pastMonthLockDay != null,
        pastMonthLockDay,
        appliesTo: isForAllClasses
          ? AppliesToValues.All
          : basicClassIds.length
          ? AppliesToValues.BasicClass
          : AppliesToValues.ClassBook,
        basicClassIds,
        classBookIds
      });
    }

    this.syncAppliesTo(this.form.value.appliesTo);
    this.syncHasPastMonthLock(this.form.value.hasPastMonthLock);

    this.appliesToControl.valueChanges
      .pipe(
        tap((appliesTo: AppliesToValues) => {
          this.syncAppliesTo(appliesTo);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
    this.hasPastMonthLockControl.valueChanges
      .pipe(
        tap((hasPastMonthLock: boolean) => {
          this.syncHasPastMonthLock(hasPastMonthLock);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.canEdit =
      this.data.institutionInfo.schoolYearAllowsModifications &&
      this.data.institutionInfo.hasSchoolYearSettingsEditAccess;
    this.canRemove =
      this.data.institutionInfo.schoolYearAllowsModifications &&
      this.data.institutionInfo.hasSchoolYearSettingsRemoveAccess;
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get appliesToControl() {
    return this.form.get('appliesTo') ?? throwError("'appliesTo' control should exist");
  }

  get basicClassIdsControl() {
    return this.form.get('basicClassIds') ?? throwError("'basicClassIds' control should exist");
  }

  get classBookIdsControl() {
    return this.form.get('classBookIds') ?? throwError("'classBookIds' control should exist");
  }

  get hasPastMonthLockControl() {
    return this.form.get('hasPastMonthLock') ?? throwError("'hasPastMonthLock' control should exist");
  }

  get pastMonthLockDayControl() {
    return this.form.get('pastMonthLockDay') ?? throwError("'pastMonthLockDay' control should exist");
  }

  private syncAppliesTo(appliiesTo: AppliesToValues | undefined) {
    switch (appliiesTo) {
      case AppliesToValues.All:
      default:
        this.basicClassIdsControl.disable();
        this.classBookIdsControl.disable();
        break;
      case AppliesToValues.BasicClass:
        this.basicClassIdsControl.enable();
        this.classBookIdsControl.disable();
        break;
      case AppliesToValues.ClassBook:
        this.basicClassIdsControl.disable();
        this.classBookIdsControl.enable();
        break;
    }
  }

  private syncHasPastMonthLock(hasPastMonthLock: boolean | undefined) {
    if (hasPastMonthLock) {
      this.pastMonthLockDayControl.enable();
    } else {
      this.pastMonthLockDayControl.disable();
    }
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onEditableChange(editable: boolean) {
    // we need to sync the disabled fields on editable change
    // as the editor-panel will enable all fields

    if (editable) {
      // use setTimeout to prevent ExpressionChangedAfterItHasBeenCheckedError
      setTimeout(() => {
        this.syncAppliesTo(this.form.value.appliesTo);
        this.syncHasPastMonthLock(this.form.value.hasPastMonthLock);
      });
    }
  }

  onSave(save: SaveToken) {
    const {
      schoolYearStartDate,
      firstTermEndDate,
      secondTermStartDate,
      schoolYearEndDate,
      description,
      hasFutureEntryLock,
      hasPastMonthLock,
      pastMonthLockDay,
      appliesTo,
      basicClassIds,
      classBookIds
    } = this.form.getRawValue();

    const schoolYearSettings = {
      schoolYearStartDate,
      firstTermEndDate,
      secondTermStartDate,
      schoolYearEndDate,
      description: description ?? throwError("'description' should not be null"),
      hasFutureEntryLock: hasFutureEntryLock ?? throwError("'hasFutureEntryLock' should not be null"),
      pastMonthLockDay: hasPastMonthLock ? pastMonthLockDay : null,
      isForAllClasses: appliesTo === AppliesToValues.All,
      basicClassIds: appliesTo === AppliesToValues.BasicClass ? basicClassIds : null,
      classBookIds: appliesTo === AppliesToValues.ClassBook ? classBookIds : null
    };

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.schoolYearSettings == null) {
            return this.schoolYearSettingsService
              .createSchoolYearSettings({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createSchoolYearSettingsCommand: schoolYearSettings
              })
              .toPromise()
              .then((newSchoolYearSettingsId) => {
                this.eventService.dispatch({ type: EventType.SchoolYearSettingsUpdated });
                this.form.markAsPristine();
                this.router.navigate(['../', newSchoolYearSettingsId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              schoolYearSettingsId: this.data.schoolYearSettings.schoolYearSettingsId
            };
            return this.schoolYearSettingsService
              .update({
                updateSchoolYearSettingsCommand: schoolYearSettings,
                ...updateArgs
              })
              .toPromise()
              .then(() => {
                this.eventService.dispatch({ type: EventType.SchoolYearSettingsUpdated });
                return this.schoolYearSettingsService.get(updateArgs).toPromise();
              })
              .then((newSchoolYearSettings) => {
                this.data.schoolYearSettings = newSchoolYearSettings;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.schoolYearSettings) {
      throw new Error('onRemove requires a school year date info to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      schoolYearSettingsId: this.data.schoolYearSettings.schoolYearSettingsId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете настройките на учебната година?',
        httpAction: () => this.schoolYearSettingsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.eventService.dispatch({ type: EventType.SchoolYearSettingsUpdated });
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }
}
