import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarStar as fadCalendarStar } from '@fortawesome/pro-duotone-svg-icons/faCalendarStar';
import { faUserMinus as fadUserMinus } from '@fortawesome/pro-duotone-svg-icons/faUserMinus';
import { faUserPlus as fadUserPlus } from '@fortawesome/pro-duotone-svg-icons/faUserPlus';
import { faCalendarCheck as farCalendarCheck } from '@fortawesome/pro-regular-svg-icons/faCalendarCheck';
import { faCalendarMinus as farCalendarMinus } from '@fortawesome/pro-regular-svg-icons/faCalendarMinus';
import { faCalendarPlus as farCalendarPlus } from '@fortawesome/pro-regular-svg-icons/faCalendarPlus';
import { faClock as farClock } from '@fortawesome/pro-regular-svg-icons/faClock';
import { faMinus as fasMinus } from '@fortawesome/pro-solid-svg-icons/faMinus';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { faUsersClass as fasUsersClass } from '@fortawesome/pro-solid-svg-icons/faUsersClass';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import {
  LectureSchedulesService,
  LectureSchedules_Get,
  LectureSchedules_GetSchedule
} from 'projects/sb-api-client/src/api/lectureSchedules.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { groupBy } from 'projects/shared/utils/array';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { dayNames, getLessonName, getLessonNameShort } from 'projects/shared/utils/schedule';
import { ArrayElementType } from 'projects/shared/utils/type';
import { enumFromStringValue, throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { BehaviorSubject, combineLatest, forkJoin, from, Observable, of, Subject } from 'rxjs';
import { catchError, startWith, switchMap, takeUntil, tap } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { LectureScheduleViewDialogComponent } from '../lecture-schedule-view-dialog/lecture-schedule-view-dialog.component';
import { LectureSchedulesMode } from '../lecture-schedules/lecture-schedules.component';

type LectureScheduleHour = ArrayElementType<LectureSchedules_Get['hours']>;

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class LectureScheduleViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    lectureSchedulesService: LectureSchedulesService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const lectureScheduleId = tryParseInt(route.snapshot.paramMap.get('lectureScheduleId'));
    const mode =
      enumFromStringValue(LectureSchedulesMode, route.snapshot.paramMap.get('mode')) ?? throwParamError('mode');

    if (lectureScheduleId) {
      this.resolve(LectureScheduleViewComponent, {
        schoolYear,
        instId,
        mode,
        lectureScheduleId,
        lectureSchedule: lectureSchedulesService.get({
          schoolYear,
          instId,
          lectureScheduleId
        }),
        schedule: lectureSchedulesService.getSchedule({
          schoolYear,
          instId,
          lectureScheduleId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(LectureScheduleViewComponent, {
        schoolYear,
        instId,
        mode,
        lectureScheduleId,
        lectureSchedule: null,
        schedule: null,
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

@Component({
  selector: 'sb-lecture-schedule-view',
  templateUrl: './lecture-schedule-view.component.html',
  styleUrls: ['./lecture-schedule-view.component.scss']
})
export class LectureScheduleViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    mode: LectureSchedulesMode;
    lectureScheduleId: number | null;
    lectureSchedule: LectureSchedules_Get | null;
    schedule: LectureSchedules_GetSchedule | null;
    institutionInfo: InstitutionInfoType;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly LectureSchedulesMode = LectureSchedulesMode;

  readonly fadCalendarStar = fadCalendarStar;
  readonly fadUserMinus = fadUserMinus;
  readonly fadUserPlus = fadUserPlus;
  readonly farCalendarPlus = farCalendarPlus;
  readonly farCalendarMinus = farCalendarMinus;
  readonly farCalendarCheck = farCalendarCheck;
  readonly farClock = farClock;
  readonly fasMinus = fasMinus;
  readonly fasPlus = fasPlus;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasUsersClass = fasUsersClass;

  readonly initialTeacherPersonId = null;
  readonly initialStartDate = null;
  readonly initialEndDate = null;

  editable$!: BehaviorSubject<boolean>;
  mappedSchedule!: MappedSchedule;
  loadingSchedule = false;
  canEdit = false;
  canRemove = false;
  removing = false;
  allGroupsExpanded = false;
  allGroupsCollapsed = false;
  hasInvalidHours = false;

  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;

  schedule: LectureSchedules_GetSchedule | null = null;
  lectureScheduleHours: LectureScheduleHour[] = [];

  readonly form = this.fb.group({
    teacherPersonId: [this.initialTeacherPersonId, Validators.required],
    orderNumber: [null, Validators.required],
    orderDate: [null, Validators.required],
    startDate: [this.initialStartDate, Validators.required],
    endDate: [this.initialEndDate, Validators.required]
  });

  constructor(
    private fb: UntypedFormBuilder,
    private lectureSchedulesService: LectureSchedulesService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog,
    instTeacherNomsService: InstTeacherNomsService
  ) {
    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear
    }));
  }

  ngOnInit() {
    if (this.data.lectureSchedule != null) {
      this.lectureScheduleHours = this.data.lectureSchedule!.hours;

      this.form.setValue({
        teacherPersonId: this.data.lectureSchedule!.teacherPersonId,
        orderNumber: this.data.lectureSchedule!.orderNumber,
        orderDate: this.data.lectureSchedule!.orderDate,
        startDate: this.data.lectureSchedule!.startDate,
        endDate: this.data.lectureSchedule!.endDate
      });
    }

    this.editable$ = new BehaviorSubject(this.data.lectureSchedule == null);
    this.canEdit =
      this.data.institutionInfo.schoolYearAllowsModifications &&
      this.data.institutionInfo.hasLectureSchedulesEditAccess;
    this.canRemove =
      this.data.institutionInfo.schoolYearAllowsModifications &&
      this.data.institutionInfo.hasLectureSchedulesRemoveAccess;
    this.hasInvalidHours =
      this.data.schedule?.slots.some((slot) => slot.hours.some((h) => !h.isClassBookValid)) ?? false;

    // update schedule
    combineLatest([
      this.editable$,
      (this.form.get('teacherPersonId')?.valueChanges as Observable<number | null>).pipe(
        startWith(this.initialTeacherPersonId) // force prettier new line
      ),
      (this.form.get('startDate')?.valueChanges as Observable<Date | null>).pipe(
        startWith(this.initialStartDate) // force prettier new line
      ),
      (this.form.get('endDate')?.valueChanges as Observable<Date | null>).pipe(
        startWith(this.initialEndDate) // force prettier new line
      )
    ])
      .pipe(
        tap(() => {
          this.loadingSchedule = true;
        }),
        switchMap(
          ([editable, teacherPersonId, startDate, endDate]): Observable<LectureSchedules_GetSchedule | null> => {
            if (!editable) {
              return of(this.data.schedule);
            } else if (this.data.lectureScheduleId != null) {
              return this.lectureSchedulesService
                .getTeacherScheduleForLectureSchedule({
                  instId: this.data.instId,
                  schoolYear: this.data.schoolYear,
                  lectureScheduleId: this.data.lectureScheduleId
                })
                .pipe(
                  catchError((err) => {
                    GlobalErrorHandler.instance.handleError(err);
                    return of(null);
                  })
                );
            } else if (teacherPersonId != null && startDate != null && endDate != null) {
              return this.lectureSchedulesService
                .getTeacherScheduleForPeriod({
                  instId: this.data.instId,
                  schoolYear: this.data.schoolYear,
                  teacherPersonId,
                  startDate,
                  endDate
                })
                .pipe(
                  catchError((err) => {
                    GlobalErrorHandler.instance.handleError(err);
                    return of(null);
                  })
                );
            } else {
              return of(null);
            }
          }
        ),
        tap((schedule) => {
          this.loadingSchedule = false;
          this.schedule = schedule;

          const lessonIdsSet =
            this.schedule?.slots.reduce((set, slot) => {
              slot.hours.forEach((h) => set.add(h.scheduleLessonId));
              return set;
            }, new Set<number>()) ?? new Set<number>();

          // remove lectureScheduleHours that have no corresponding lesson in the schedule as they won't be shown anyway
          this.lectureScheduleHours = this.lectureScheduleHours.filter((h) => lessonIdsSet.has(h.scheduleLessonId));

          this.updateMappedSchedule();
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();

    this.editable$.complete();
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onEditableChange(editable: boolean) {
    this.editable$.next(editable);
  }

  onSave(save: SaveToken) {
    if (!this.lectureScheduleHours.length) {
      save.done(false);
      return;
    }

    const value = this.form.value;
    const lectureSchedule = {
      teacherPersonId: <number>value.teacherPersonId,
      orderNumber: <string>value.orderNumber,
      orderDate: <Date>value.orderDate,
      startDate: <Date>value.startDate,
      endDate: <Date>value.endDate,
      scheduleLessonIds: this.lectureScheduleHours.map((h) => h.scheduleLessonId)
    };

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.lectureScheduleId == null) {
            return this.lectureSchedulesService
              .createLectureSchedule({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createLectureScheduleCommand: lectureSchedule
              })
              .toPromise()
              .then((newLectureScheduleId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newLectureScheduleId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              lectureScheduleId: this.data.lectureScheduleId
            };
            return this.lectureSchedulesService
              .update({
                updateLectureScheduleCommand: lectureSchedule,
                ...updateArgs
              })
              .toPromise()
              .then(() =>
                forkJoin({
                  newLectureSchedule: this.lectureSchedulesService.get(updateArgs),
                  newSchedule: this.lectureSchedulesService.getSchedule(updateArgs)
                }).toPromise()
              )
              .then(({ newLectureSchedule, newSchedule }) => {
                this.data.lectureSchedule = newLectureSchedule;
                this.data.schedule = newSchedule;

                this.lectureScheduleHours = this.data.lectureSchedule.hours;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.lectureScheduleId) {
      throw new Error('onRemove requires a lectureSchedule to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      lectureScheduleId: this.data.lectureScheduleId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете лекторския график?',
        errorsMessage: 'Не може да изтриете лекторския график, защото:',
        httpAction: () => this.lectureSchedulesService.remove(removeParams).toPromise()
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

  viewGroup(group: MappedScheduleCurriculumGroup) {
    const hours = group.curriculumHours.map((h) => {
      const lectureHour = group.lectureHours.find((tah) => tah.scheduleLessonId === h.scheduleLessonId);
      return {
        date: h.date,
        scheduleLessonId: h.scheduleLessonId,
        isLectureHour: lectureHour != null,
        isValid: !!h.isClassBookValid
      };
    });

    openTypedDialog(this.dialog, LectureScheduleViewDialogComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        editable: this.editable$.value,
        group,
        hours
      }
    })
      .afterClosed()
      .toPromise()
      .then((dialogHours) => {
        if (!dialogHours) return;

        const newHours = [];

        for (const lectureHour of this.lectureScheduleHours) {
          const dialogHourIndex = dialogHours.findIndex((dh) => dh.scheduleLessonId === lectureHour.scheduleLessonId);

          const dialogHour = dialogHourIndex > -1 ? dialogHours.splice(dialogHourIndex, 1)[0] : null;

          if (!dialogHour) {
            newHours.push(lectureHour);
          } else if (dialogHour.isLectureHour === false) {
            continue;
          } else {
            newHours.push({ scheduleLessonId: dialogHour.scheduleLessonId, date: dialogHour.date });
          }
        }

        for (const dialogHour of dialogHours) {
          if (dialogHour.isLectureHour) {
            newHours.push({ scheduleLessonId: dialogHour.scheduleLessonId, date: dialogHour.date });
          }
        }

        this.lectureScheduleHours = newHours;
        this.updateMappedSchedule();
      });
  }

  toggleGroup(event: Event, group: MappedScheduleCurriculumGroup) {
    event.stopPropagation();

    group.isExpanded = !group.isExpanded;

    this.syncExpandedCollapsed();
  }

  expandAllGroups() {
    this.toggleAllGroups(true);
  }

  collapseAllGroups() {
    this.toggleAllGroups(false);
  }

  syncExpandedCollapsed() {
    this.allGroupsExpanded = this.checkAllGroups((g) => g.isExpanded);
    this.allGroupsCollapsed = this.checkAllGroups((g) => !g.isExpanded);
  }

  toggleAllGroups(isExpanded: boolean) {
    for (const shiftRow of this.mappedSchedule!.slotsByNumberByDay) {
      for (const slot of shiftRow) {
        for (const group of slot.curriculumGroups) {
          group.isExpanded = isExpanded;
        }
      }
    }

    this.syncExpandedCollapsed();
  }

  checkAllGroups(predicate: (g: MappedScheduleCurriculumGroup) => boolean) {
    for (const shiftRow of this.mappedSchedule?.slotsByNumberByDay ?? []) {
      for (const slot of shiftRow) {
        for (const group of slot.curriculumGroups) {
          if (group.lectureHours.length && !predicate(group)) {
            return false;
          }
        }
      }
    }

    return true;
  }

  updateMappedSchedule() {
    this.mappedSchedule = mapSchedule(this.schedule, this.lectureScheduleHours);
    this.syncExpandedCollapsed();
  }
}

type MappedSchedule = ReturnType<typeof mapSchedule>;
export type MappedScheduleCurriculumGroup = ArrayElementType<
  ArrayElementType<ArrayElementType<NonNullable<MappedSchedule>['slotsByNumberByDay']>>['curriculumGroups']
>;

function mapSchedule(schedule: LectureSchedules_GetSchedule | null, lectureScheduleHours: LectureScheduleHour[]) {
  if (!schedule) {
    return null;
  }

  const scheduleIncludesWeekend = schedule.slots.find((slot) => slot.day > 5) != null;

  return {
    shiftHours: schedule.shiftHours,
    scheduleIncludesWeekend,
    slotsByNumberByDay: schedule.shiftHours.map((shiftHour) =>
      Array.from({ length: scheduleIncludesWeekend ? 7 : 5 }, (v, dayIndex) => {
        const day = dayIndex + 1;
        const slotNumber = shiftHour.slotNumber;

        const slot =
          schedule.slots.find((slot) => slot.day === day && slot.slotNumber === slotNumber) ??
          throwError('The slot must always exist');

        const curriculumGroups = groupBy(slot.hours, (h) => `${h.classBookId}_${h.curriculumId}_${h.studentPersonId}`)
          .map(([, curriculumHours]) => {
            const curriculumHour = curriculumHours[0];

            const lectureHours = lectureScheduleHours.filter(
              (lsh) => curriculumHours.find((h) => h.scheduleLessonId === lsh.scheduleLessonId) != null
            );

            return {
              dayName: dayNames[day],
              shiftHour,
              classBookFullName: curriculumHour.classBookFullName,
              isValid: curriculumHour.isClassBookValid,
              subject: getLessonName(curriculumHour),
              subjectShort: getLessonNameShort(curriculumHour),

              curriculumHours,
              lectureHours,

              isExpanded: lectureHours.length > 0
            };
          })
          .filter((group) => group.isValid || group.lectureHours.length > 0);

        return {
          day,
          slotNumber,
          curriculumGroups
        };
      })
    )
  };
}
