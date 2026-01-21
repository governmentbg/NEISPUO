import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
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
  TeacherAbsencesService,
  TeacherAbsences_Get,
  TeacherAbsences_GetSchedule
} from 'projects/sb-api-client/src/api/teacherAbsences.service';
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
import { formatDate } from 'projects/shared/utils/date';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { dayNames, getLessonName, getLessonNameShort } from 'projects/shared/utils/schedule';
import { ArrayElementType } from 'projects/shared/utils/type';
import { enumFromStringValue, throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { BehaviorSubject, combineLatest, forkJoin, from, Observable, of, Subject } from 'rxjs';
import { catchError, startWith, switchMap, takeUntil, tap } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import {
  ReplType,
  TeacherAbsenceViewDialogComponent,
  TeacherAbsenceViewDialogResultHour
} from '../teacher-absence-view-dialog/teacher-absence-view-dialog.component';
import { TeacherAbsenceViewSelectHoursDialogComponent } from '../teacher-absence-view-select-hours-dialog/teacher-absence-view-select-hours-dialog.component';
import { TeacherAbsencesMode } from '../teacher-absences/teacher-absences.component';
import { TeacherReplHoursViewDialogComponent } from './../teacher-repl-hours-view-dialog/teacher-repl-hours-view-dialog.component';

type TeacherAbsenceHour = ArrayElementType<TeacherAbsences_Get['hours']>;
type TeacherAbsenceScheduleHour = ArrayElementType<ArrayElementType<TeacherAbsences_GetSchedule['slots']>['hours']>;

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class TeacherAbsenceViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    teacherAbsencesService: TeacherAbsencesService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const teacherAbsenceId = tryParseInt(route.snapshot.paramMap.get('teacherAbsenceId'));
    const mode =
      enumFromStringValue(TeacherAbsencesMode, route.snapshot.paramMap.get('mode')) ?? throwParamError('mode');

    if (teacherAbsenceId) {
      this.resolve(TeacherAbsenceViewComponent, {
        schoolYear,
        instId,
        mode,
        teacherAbsenceId,
        teacherAbsence: teacherAbsencesService.get({
          schoolYear,
          instId,
          teacherAbsenceId
        }),
        schedule: teacherAbsencesService.getSchedule({
          schoolYear,
          instId,
          teacherAbsenceId
        }),
        institutionInfo: from(institutionInfo)
      });
    } else {
      this.resolve(TeacherAbsenceViewComponent, {
        schoolYear,
        instId,
        mode,
        teacherAbsenceId,
        teacherAbsence: null,
        schedule: null,
        institutionInfo: from(institutionInfo)
      });
    }
  }
}

@Component({
  selector: 'sb-teacher-absence-view',
  templateUrl: './teacher-absence-view.component.html',
  styleUrls: ['./teacher-absence-view.component.scss']
})
export class TeacherAbsenceViewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    mode: TeacherAbsencesMode;
    teacherAbsenceId: number | null;
    teacherAbsence: TeacherAbsences_Get | null;
    schedule: TeacherAbsences_GetSchedule | null;
    institutionInfo: InstitutionInfoType;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly TeacherAbsencesMode = TeacherAbsencesMode;

  readonly fadCalendarMinus = fadCalendarMinus;
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
  scheduleIsDirty = false;
  hasInvalidHours = false;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;

  schedule: TeacherAbsences_GetSchedule | null = null;
  teacherAbsenceHours: TeacherAbsenceHour[] = [];

  readonly form = this.fb.group({
    teacherPersonId: [this.initialTeacherPersonId, Validators.required],
    startDate: [this.initialStartDate, Validators.required],
    endDate: [this.initialEndDate, Validators.required],
    reason: [null, Validators.required]
  });

  constructor(
    private fb: UntypedFormBuilder,
    private teacherAbsencesService: TeacherAbsencesService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog,
    instTeacherNomsService: InstTeacherNomsService
  ) {
    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: this.data.instId,
      schoolYear: this.data.schoolYear,
      includeNoReplacementTeachers: true
    }));
  }

  ngOnInit() {
    if (this.data.teacherAbsence != null) {
      this.form.setValue({
        teacherPersonId: this.data.teacherAbsence.teacherPersonId,
        startDate: this.data.teacherAbsence.startDate,
        endDate: this.data.teacherAbsence.endDate,
        reason: this.data.teacherAbsence.reason
      });

      this.teacherAbsenceHours = this.data.teacherAbsence.hours;
    }

    this.editable$ = new BehaviorSubject(this.data.teacherAbsence == null);
    this.canEdit =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasTeacherAbsencesEditAccess;
    this.canRemove =
      this.data.institutionInfo.schoolYearAllowsModifications &&
      this.data.institutionInfo.hasTeacherAbsencesRemoveAccess;
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
        switchMap(([editable, teacherPersonId, startDate, endDate]): Observable<TeacherAbsences_GetSchedule | null> => {
          if (!editable) {
            return of(this.data.schedule);
          } else if (this.data.teacherAbsenceId != null) {
            return this.teacherAbsencesService
              .getTeacherScheduleForAbsence({
                instId: this.data.instId,
                schoolYear: this.data.schoolYear,
                teacherAbsenceId: this.data.teacherAbsenceId
              })
              .pipe(
                catchError((err) => {
                  GlobalErrorHandler.instance.handleError(err);
                  return of(null);
                })
              );
          } else if (teacherPersonId != null && startDate != null && endDate != null) {
            return this.teacherAbsencesService
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
        }),
        tap((schedule) => {
          this.loadingSchedule = false;
          this.schedule = schedule;

          const lessonIdsSet =
            this.schedule?.slots.reduce((set, slot) => {
              slot.hours.forEach((h) => set.add(h.scheduleLessonId));
              return set;
            }, new Set<number>()) ?? new Set<number>();

          // remove teacherAbsenceHours that have no corresponding lesson in the schedule as they won't be shown anyway
          this.teacherAbsenceHours = this.teacherAbsenceHours.filter((h) => lessonIdsSet.has(h.scheduleLessonId));

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
    return this.form.dirty || this.scheduleIsDirty;
  }

  onEditableChange(editable: boolean) {
    this.editable$.next(editable);
  }

  onSave(save: SaveToken) {
    if (!this.teacherAbsenceHours.length) {
      save.done(false);
      return;
    }

    const teacherAbsence = {
      ...this.form.value,
      hours: this.teacherAbsenceHours.map((h) => ({
        scheduleLessonId: h.scheduleLessonId,
        replTeacherPersonId: h.replTeacherPersonId,
        replTeacherIsNonSpecialist: h.replTeacherIsNonSpecialist,
        extReplTeacherName: h.extReplTeacherName
      }))
    };

    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.teacherAbsenceId == null) {
            return this.teacherAbsencesService
              .createTeacherAbsence({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                createTeacherAbsenceCommand: teacherAbsence
              })
              .toPromise()
              .then((newTeacherAbsenceId) => {
                this.form.markAsPristine();
                this.scheduleIsDirty = false;
                this.router.navigate(['../', newTeacherAbsenceId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              teacherAbsenceId: this.data.teacherAbsenceId
            };
            return this.teacherAbsencesService
              .update({
                updateTeacherAbsenceCommand: teacherAbsence,
                ...updateArgs
              })
              .toPromise()
              .then(() =>
                forkJoin({
                  newTeacherAbsence: this.teacherAbsencesService.get(updateArgs),
                  newSchedule: this.teacherAbsencesService.getSchedule(updateArgs)
                }).toPromise()
              )
              .then(({ newTeacherAbsence, newSchedule }) => {
                this.data.teacherAbsence = newTeacherAbsence;
                this.data.schedule = newSchedule;

                this.teacherAbsenceHours = this.data.teacherAbsence.hours;
                this.scheduleIsDirty = false;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.teacherAbsenceId) {
      throw new Error('onRemove requires a teacherAbsence to have been loaded.');
    }

    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      teacherAbsenceId: this.data.teacherAbsenceId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете учителското отсъствие?',
        errorsMessage: 'Не може да изтриете учителското отсъствие, защото:',
        httpAction: () => this.teacherAbsencesService.remove(removeParams).toPromise()
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
      const absenceHour = group.absenceHours.find((tah) => tah.scheduleLessonId === h.scheduleLessonId);

      return {
        date: h.date,
        scheduleLessonId: h.scheduleLessonId,
        replType: !absenceHour
          ? ReplType.Unset
          : !absenceHour.replTeacherPersonId && !absenceHour.extReplTeacherName
          ? ReplType.EmptyHour
          : absenceHour.extReplTeacherName
          ? ReplType.ExtTeacher
          : !absenceHour.replTeacherIsNonSpecialist
          ? ReplType.Specialist
          : ReplType.NonSpecialist,
        replTeacherPersonId: absenceHour?.replTeacherPersonId,
        extReplTeacherName: absenceHour?.extReplTeacherName,
        isReadOnly: !!h.isInUse || !!absenceHour?.isInUse,
        isValid: !!h.isClassBookValid,
        hasNoReplacementTeacher: !!h.curriculumTeachers.some((ct) => ct.markedAsNoReplacement)
      };
    });

    openTypedDialog(this.dialog, TeacherAbsenceViewDialogComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        editable: this.editable$.value,
        singleHourMode: false,
        group,
        hours
      }
    })
      .afterClosed()
      .toPromise()
      .then((dialogHours) => {
        if (!dialogHours) return;

        const createTeacherAbsenceHour = (dh: TeacherAbsenceViewDialogResultHour): TeacherAbsenceHour => {
          const hasReplTeacher = dh.replType === ReplType.Specialist || dh.replType === ReplType.NonSpecialist;

          return {
            scheduleLessonId: dh.scheduleLessonId,
            replTeacherPersonId: hasReplTeacher ? dh.replTeacherPersonId : null,
            replTeacherFirstName: hasReplTeacher ? dh.replTeacherFirstName : null,
            replTeacherLastName: hasReplTeacher ? dh.replTeacherLastName : null,
            replTeacherIsNonSpecialist: hasReplTeacher ? dh.replType === ReplType.NonSpecialist : null,
            extReplTeacherName: dh.extReplTeacherName,
            isInUse: false
          };
        };

        const newHours = [];

        for (const absenceHour of this.teacherAbsenceHours) {
          const dialogHourIndex = dialogHours.findIndex((dh) => dh.scheduleLessonId === absenceHour.scheduleLessonId);

          const dialogHour = dialogHourIndex > -1 ? dialogHours.splice(dialogHourIndex, 1)[0] : null;

          if (absenceHour.isInUse || !dialogHour) {
            newHours.push(absenceHour);
          } else if (dialogHour.replType === ReplType.Unset) {
            continue;
          } else {
            newHours.push(createTeacherAbsenceHour(dialogHour));
          }
        }

        for (const dialogHour of dialogHours) {
          if (dialogHour.replType !== ReplType.Unset) {
            newHours.push(createTeacherAbsenceHour(dialogHour));
          }
        }

        this.teacherAbsenceHours = newHours;
        this.updateMappedSchedule();
        this.scheduleIsDirty = true;
      });
  }

  viewReplHours(group: MappedScheduleCurriculumGroup) {
    const hours = group.curriculumHours.map((h) => {
      return {
        absenceId: h.teacherAbsenceId,
        date: formatDate(h.date)
      };
    });

    const absences = groupBy(hours, (a) => a.absenceId).map(([absenceId, dates]) => {
      return {
        absenceId: absenceId,
        dates: dates.map((d) => d.date).join(', ')
      };
    });

    openTypedDialog(this.dialog, TeacherReplHoursViewDialogComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        group,
        absences: absences,
        route: this.route
      }
    });
  }

  fillAll() {
    openTypedDialog(this.dialog, TeacherAbsenceViewDialogComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        editable: this.editable$.value,
        hours: [
          {
            date: null,
            scheduleLessonId: null,
            replType: ReplType.Unset,
            replTeacherPersonId: null,
            extReplTeacherName: null,
            isReadOnly: false,
            isValid: true,
            hasNoReplacementTeacher: false
          }
        ],
        singleHourMode: true,
        group: null
      }
    })
      .afterClosed()
      .toPromise()
      .then((dialogHours) => {
        if (!dialogHours) return;

        const dialogHour = dialogHours[0];

        const allHours =
          this.schedule?.slots.reduce(
            (r, s) => r.concat(s.hours.filter((h) => !h.isReplHour)),
            new Array<TeacherAbsenceScheduleHour>()
          ) ?? [];

        const newHours = [];

        for (const hour of allHours) {
          const absenceHour = this.teacherAbsenceHours.find((tah) => tah.scheduleLessonId === hour.scheduleLessonId);

          if (absenceHour && (absenceHour.isInUse || !hour.isClassBookValid)) {
            newHours.push(absenceHour);
          } else if (dialogHour.replType === ReplType.Unset || hour.isInUse || !hour.isClassBookValid) {
            continue;
          } else {
            const hasReplTeacher =
              dialogHour.replType === ReplType.Specialist || dialogHour.replType === ReplType.NonSpecialist;

            newHours.push({
              scheduleLessonId: hour.scheduleLessonId,
              replTeacherPersonId: hasReplTeacher ? dialogHour.replTeacherPersonId : null,
              replTeacherFirstName: hasReplTeacher ? dialogHour.replTeacherFirstName : null,
              replTeacherLastName: hasReplTeacher ? dialogHour.replTeacherLastName : null,
              replTeacherIsNonSpecialist: hasReplTeacher ? dialogHour.replType === ReplType.NonSpecialist : null,
              extReplTeacherName: dialogHour.extReplTeacherName,
              isInUse: false
            });
          }
        }

        this.teacherAbsenceHours = newHours;
        this.updateMappedSchedule();
        this.scheduleIsDirty = true;
      });
  }

  fillSelected() {
    openTypedDialog(this.dialog, TeacherAbsenceViewSelectHoursDialogComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        mappedSchedule: this.mappedSchedule
      }
    })
      .afterClosed()
      .toPromise()
      .then((dialogResult) => {
        if (!dialogResult) return;

        const newHours: TeacherAbsenceHour[] = [];

        for (const scheduleLessonId of dialogResult.scheduleLessonIds) {
          const absenceHour = this.teacherAbsenceHours.find((tah) => tah.scheduleLessonId === scheduleLessonId);

          const hasReplTeacher =
            dialogResult.replType === ReplType.Specialist || dialogResult.replType === ReplType.NonSpecialist;

          if (absenceHour) {
            const hourIndex = this.teacherAbsenceHours.indexOf(absenceHour);

            if (dialogResult.replType === ReplType.Unset) {
              this.teacherAbsenceHours.splice(hourIndex, 1);
            } else if (dialogResult.replType === ReplType.EmptyHour) {
              this.teacherAbsenceHours[hourIndex] = {
                scheduleLessonId: scheduleLessonId
              };
            } else {
              this.teacherAbsenceHours[hourIndex] = {
                scheduleLessonId: scheduleLessonId,
                replTeacherPersonId: hasReplTeacher ? dialogResult.replTeacherPersonId : null,
                replTeacherFirstName: hasReplTeacher ? dialogResult.replTeacherFirstName : null,
                replTeacherLastName: hasReplTeacher ? dialogResult.replTeacherLastName : null,
                replTeacherIsNonSpecialist: hasReplTeacher ? dialogResult.replType === ReplType.NonSpecialist : null,
                extReplTeacherName: dialogResult.extReplTeacherName,
                isInUse: false
              };
            }
          } else {
            newHours.push({
              scheduleLessonId: scheduleLessonId,
              replTeacherPersonId: hasReplTeacher ? dialogResult.replTeacherPersonId : null,
              replTeacherFirstName: hasReplTeacher ? dialogResult.replTeacherFirstName : null,
              replTeacherLastName: hasReplTeacher ? dialogResult.replTeacherLastName : null,
              replTeacherIsNonSpecialist: hasReplTeacher ? dialogResult.replType === ReplType.NonSpecialist : null,
              extReplTeacherName: dialogResult.extReplTeacherName,
              isInUse: false
            });
          }
        }

        this.teacherAbsenceHours = this.teacherAbsenceHours.concat(newHours);
        this.updateMappedSchedule();
        this.scheduleIsDirty = true;
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
          if (group.teachers.length && !predicate(group)) {
            return false;
          }
        }
      }
    }

    return true;
  }

  updateMappedSchedule() {
    this.mappedSchedule = mapSchedule(this.schedule, this.teacherAbsenceHours);
    this.syncExpandedCollapsed();
  }
}

export type MappedSchedule = ReturnType<typeof mapSchedule>;
export type MappedScheduleCurriculumGroup = ArrayElementType<
  ArrayElementType<ArrayElementType<NonNullable<MappedSchedule>['slotsByNumberByDay']>>['curriculumGroups']
>;

function mapSchedule(schedule: TeacherAbsences_GetSchedule | null, teacherAbsenceHours: TeacherAbsenceHour[]) {
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

            const absenceHours = teacherAbsenceHours.filter(
              (tah) => curriculumHours.find((h) => h.scheduleLessonId === tah.scheduleLessonId) != null
            );

            const teachers = groupBy(
              absenceHours,
              (h) => `${h.replTeacherPersonId || h.extReplTeacherName || 'empty'}_${!!h.replTeacherIsNonSpecialist}`
            ).map(([, group]) => ({
              id: group[0].replTeacherPersonId,
              firstName: group[0].replTeacherFirstName,
              lastName: group[0].replTeacherLastName,
              isNonSpecialist: group[0].replTeacherIsNonSpecialist,
              extReplTeacherName: group[0].extReplTeacherName,
              count: group.length
            }));

            return {
              dayName: dayNames[day],
              shiftHour,
              classBookFullName: curriculumHour.classBookFullName,
              isValid: curriculumHour.isClassBookValid,
              subject: getLessonName(curriculumHour),
              subjectShort: getLessonNameShort(curriculumHour),

              curriculumHours,
              absenceHours,

              teachers: teachers,
              isExpanded: teachers.length > 0,
              isReplHour: curriculumHour.isReplHour,
              replHourAbsenceId: curriculumHour.teacherAbsenceId,
              hasNoReplacementTeacher: curriculumHours.some((h) =>
                h.curriculumTeachers.some((ct) => ct.markedAsNoReplacement)
              )
            };
          })
          .filter((group) => group.isValid || group.absenceHours.length > 0);

        return {
          day,
          slotNumber,
          curriculumGroups
        };
      })
    )
  };
}
