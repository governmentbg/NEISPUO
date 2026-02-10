import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faBellSchool as fadBellSchool } from '@fortawesome/pro-duotone-svg-icons/faBellSchool';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import {
  ShiftsService,
  Shifts_Get,
  Shifts_GetHoursUsedInSchedule
} from 'projects/sb-api-client/src/api/shifts.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { ShiftFormValue } from 'projects/shared/components/shift-form/shift-form.component';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class ShiftViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    shiftsService: ShiftsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const shiftId = tryParseInt(route.snapshot.paramMap.get('shiftId'));
    if (shiftId) {
      this.resolve(ShiftViewComponent, {
        schoolYear,
        instId,
        shift: shiftsService.get({
          schoolYear,
          instId,
          shiftId
        }),
        hoursUsedInSchedule: shiftsService.getHoursUsedInSchedule({
          schoolYear,
          instId,
          shiftId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(ShiftViewComponent, {
        schoolYear,
        instId,
        shift: null,
        hoursUsedInSchedule: [],
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

@Component({
  selector: 'sb-shift-view',
  templateUrl: './shift-view.component.html',
  styleUrls: ['./shift-view.component.scss']
})
export class ShiftViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    hoursUsedInSchedule: Shifts_GetHoursUsedInSchedule;
    shift: Shifts_Get | null;
  };

  private readonly destroyed$ = new Subject<void>();
  canEdit = false;
  canRemove = false;

  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fadBellSchool = fadBellSchool;

  form!: FormGroup<{
    name: FormControl<string>;
    shift: FormControl<ShiftFormValue>;
  }>;

  removing = false;

  constructor(
    private fb: FormBuilder,
    private shiftsService: ShiftsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {}

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    const shift = this.data.shift;
    const shiftFormValue: ShiftFormValue = {
      isMultiday: shift?.isMultiday ?? false,
      days: shift?.days ?? [
        {
          day: 1,
          hours: [{ hourNumber: 1, startTime: '07:30', endTime: '08:10' }]
        }
      ]
    };

    this.form = this.fb.nonNullable.group({
      name: [shift?.name ?? '', Validators.required],
      shift: [shiftFormValue]
    });

    this.canEdit =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasShiftEditAccess;
    this.canRemove =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasShiftRemoveAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.getRawValue();

    const shift = {
      name: value.name,
      isMultiday: value.shift.isMultiday,
      days: value.shift.days.map((d) => ({
        ...d,
        hours: d.hours.map((h) => ({
          hourNumber: h.hourNumber ?? throwError('hourNumber should not be null'),
          startTime: h.startTime ?? throwError('startTime should not be null'),
          endTime: h.endTime ?? throwError('endTime should not be null')
        }))
      }))
    };

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.shift == null) {
            return this.shiftsService
              .createShift({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createShiftCommand: shift
              })
              .toPromise()
              .then((newShiftId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newShiftId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              shiftId: this.data.shift.shiftId
            };
            return this.shiftsService
              .update({
                updateShiftCommand: shift,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.shiftsService.get(updateArgs).toPromise())
              .then((newShift) => {
                this.data.shift = newShift;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.shift) {
      throw new Error('onRemove requires a shift to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      shiftId: this.data.shift.shiftId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете смяната?',
        errorsMessage: 'Не може да изтриете смяната, защото:',
        httpAction: () => this.shiftsService.remove(removeParams).toPromise()
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
