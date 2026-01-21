import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarDay as fadCalendarDay } from '@fortawesome/pro-duotone-svg-icons/faCalendarDay';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { BasicClassNomsService } from 'projects/sb-api-client/src/api/basicClassNoms.service';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import { OffDaysService, OffDays_Get } from 'projects/sb-api-client/src/api/offDays.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  INomService,
  NomServiceWithParams,
  YesNoNomsService,
  YesNoValues
} from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwError, throwParamError, truncateWithEllipsis, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class OffDayViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    offDaysService: OffDaysService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const offDayId = tryParseInt(route.snapshot.paramMap.get('offDayId'));
    if (offDayId) {
      this.resolve(OffDayViewComponent, {
        schoolYear,
        instId,
        offDay: offDaysService.get({
          schoolYear,
          instId,
          offDayId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(OffDayViewComponent, { schoolYear, instId, offDay: null, institutionInfo: from(institutionInfo) });
    }
  }
}

enum AppliesToValues {
  All = 'All',
  BasicClass = 'BasicClass',
  ClassBook = 'ClassBook'
}

@Component({
  selector: 'sb-off-day-view',
  templateUrl: './off-day-view.component.html'
})
export class OffDayViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    offDay: OffDays_Get | null;
    institutionInfo: InstitutionInfoType;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fadCalendarDay = fadCalendarDay;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly yesNoNomsService = new YesNoNomsService();
  readonly AppliesToValues_All = AppliesToValues.All;
  readonly AppliesToValues_BasicClass = AppliesToValues.BasicClass;
  readonly AppliesToValues_ClassBook = AppliesToValues.ClassBook;
  canEdit = false;
  canRemove = false;
  truncatedDescription = '';

  readonly form = this.fb.nonNullable.group({
    from: this.fb.nonNullable.control<Date | null>(null, Validators.required),
    to: this.fb.nonNullable.control<Date | null>(null, Validators.required),
    description: this.fb.nonNullable.control<string | null>(null, Validators.required),
    appliesTo: this.fb.nonNullable.control<AppliesToValues>(AppliesToValues.All, Validators.required),
    basicClassIds: this.fb.nonNullable.control<number[]>([], Validators.required),
    classBookIds: this.fb.nonNullable.control<number[]>([], Validators.required),
    isPgOffProgramDay: this.fb.nonNullable.control<YesNoValues>(YesNoValues.Yes, Validators.required)
  });

  basicClassNomsService: INomService<number, { instId: number; schoolYear: number }>;
  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;
  removing = false;

  constructor(
    private fb: FormBuilder,
    private offDaysService: OffDaysService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
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
    if (this.data.offDay != null) {
      const { from, to, description, isForAllClasses, basicClassIds, classBookIds, isPgOffProgramDay } =
        this.data.offDay;
      this.form.setValue({
        from,
        to,
        description,
        appliesTo: isForAllClasses
          ? AppliesToValues.All
          : basicClassIds.length
          ? AppliesToValues.BasicClass
          : AppliesToValues.ClassBook,
        basicClassIds,
        classBookIds,
        isPgOffProgramDay: isPgOffProgramDay ? YesNoValues.Yes : YesNoValues.No
      });

      this.truncatedDescription = truncateWithEllipsis(description, 100);
    }

    this.syncAppliesTo(this.form.value.appliesTo);

    this.appliesToControl.valueChanges
      .pipe(
        tap((appliesTo: AppliesToValues) => {
          this.syncAppliesTo(appliesTo);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.canEdit =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasOffDayEditAccess;
    this.canRemove =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasOffDayRemoveAccess;
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
      });
    }
  }

  onSave(save: SaveToken) {
    const { from, to, description, appliesTo, basicClassIds, classBookIds, isPgOffProgramDay } =
      this.form.getRawValue();
    const offDay = {
      from: from ?? throwError("'from' should not be null"),
      to: to ?? throwError("'to' should not be null"),
      description: description ?? throwError("'description' should not be null"),
      isForAllClasses: appliesTo === AppliesToValues.All,
      basicClassIds: appliesTo === AppliesToValues.BasicClass ? basicClassIds : null,
      classBookIds: appliesTo === AppliesToValues.ClassBook ? classBookIds : null,
      isPgOffProgramDay: isPgOffProgramDay === YesNoValues.Yes
    };

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.offDay == null) {
            return this.offDaysService
              .createOffDay({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createOffDayCommand: offDay
              })
              .toPromise()
              .then((newOffDayId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newOffDayId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              offDayId: this.data.offDay.offDayId
            };
            return this.offDaysService
              .update({
                updateOffDayCommand: offDay,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.offDaysService.get(updateArgs).toPromise())
              .then((newOffDay) => {
                this.data.offDay = newOffDay;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.offDay) {
      throw new Error('onRemove requires a off day to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      offDayId: this.data.offDay.offDayId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете неучебния ден?',
        errorsMessage: 'Не може да изтриете неучебния ден, защото:',
        httpAction: () => this.offDaysService.remove(removeParams).toPromise()
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
}
