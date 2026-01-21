import { Component, Inject, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormRecord, NgForm, Validators } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPencilAlt as fadPencilAlt } from '@fortawesome/pro-duotone-svg-icons/faPencilAlt';
import { faPlusCircle as fadPlusCircle } from '@fortawesome/pro-duotone-svg-icons/faPlusCircle';
import { faSpinnerThird as fadSpinnerThird } from '@fortawesome/pro-duotone-svg-icons/faSpinnerThird';
import { faTrashAlt as fadTrashAlt } from '@fortawesome/pro-duotone-svg-icons/faTrashAlt';
import { faUsers as fadUsers } from '@fortawesome/pro-duotone-svg-icons/faUsers';
import { faUserSlash as fadUserSlash } from '@fortawesome/pro-duotone-svg-icons/faUserSlash';
import { faCalendarCheck as farCalendarCheck } from '@fortawesome/pro-regular-svg-icons/faCalendarCheck';
import { faAlignJustify as fasAlignJustify } from '@fortawesome/pro-solid-svg-icons/faAlignJustify';
import { faAlignSlash as fasAlignSlash } from '@fortawesome/pro-solid-svg-icons/faAlignSlash';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faChevronLeft as fasChevronLeft } from '@fortawesome/pro-solid-svg-icons/faChevronLeft';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { faUndo as fasUndo } from '@fortawesome/pro-solid-svg-icons/faUndo';
import { faUserEdit as fasUserEdit } from '@fortawesome/pro-solid-svg-icons/faUserEdit';
import { add, addWeeks, format, getISOWeek, getISOWeekYear, isSameDay, setISODay, setISOWeek } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  ClassBookStudentNomsService,
  ClassBookStudentNoms_GetNomsByTerm
} from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import {
  SchedulesService,
  Schedules_GetClassBookScheduleForWeek
} from 'projects/sb-api-client/src/api/schedules.service';
import {
  TopicsService,
  Topics_CreateTopicsRequestParams,
  Topics_GetAllForWeek,
  Topics_RemoveTopicsRequestParams
} from 'projects/sb-api-client/src/api/topics.service';
import { TopicsDplrService, TopicsDplr_GetAllForWeek } from 'projects/sb-api-client/src/api/topicsDplr.service';
import { TopicsGetAllForWeekVOStudent } from 'projects/sb-api-client/src/model/topicsGetAllForWeekVOStudent';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { PaginatorItem } from 'projects/shared/components/paginator/paginator-item';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { max, min, range, stableSort } from 'projects/shared/utils/array';
import { assert } from 'projects/shared/utils/assert';
import { ClassBookInfoType, UNDO_INTERVAL_IN_MINUTES } from 'projects/shared/utils/book';
import { formatNullableDate } from 'projects/shared/utils/date';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { expiredAt } from 'projects/shared/utils/rxjs';
import { extendScheduleHour } from 'projects/shared/utils/schedule';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Observable, of, Subject } from 'rxjs';
import { catchError, finalize, switchMap, takeUntil, tap } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';
import { AddTopicDplrDialogSkeletonComponent } from './add-topic-dplr-dialog/add-topic-dplr-dialog.component';

export enum TopicsDplrEditMode {
  View = 'View',
  New = 'New',
  Remove = 'Remove',
  RemoveTopicDplr = 'RemoveTopicDplr'
}

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class TopicsEditDplrSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    topicsService: TopicsService,
    topicsDplrService: TopicsDplrService,
    schedulesService: SchedulesService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const weekNumber = tryParseInt(route.snapshot.paramMap.get('weekNumber')) ?? throwParamError('weekNumber');
    const mode = route.snapshot.data.mode as TopicsDplrEditMode;

    this.resolve(TopicsEditDplrComponent, {
      schoolYear,
      instId,
      classBookId,
      year,
      weekNumber,
      mode,
      classBookInfo: from(classBookInfo),
      individualCurriculumStudents: classBookStudentNomsService.getNomsByTerm({
        instId,
        schoolYear,
        classBookId,
        showOnlyWithIndividualCurriculumSchedule: true
      }),
      topics: topicsService.getAllForWeek({
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber
      }),
      topicsDplr: topicsDplrService.getAllForWeek({
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber
      }),
      curriculumsWithTopicPlan: topicsService.getCurriculumsWithTopicPlan({
        schoolYear,
        instId,
        classBookId
      }),
      schedule: schedulesService.getClassBookScheduleForWeek({
        schoolYear,
        instId,
        classBookId,
        year,
        weekNumber
      })
    });
  }
}

type ScheduleHour = ArrayElementType<Schedules_GetClassBookScheduleForWeek['hours']>;
// schedule hour with only hourNumber required
type PartialScheduleHour = Partial<ScheduleHour> & { hourNumber: number };
type ScheduleHourWithTopic = ArrayElementType<ArrayElementType<ReturnType<typeof getScheduleByDay>>['hours']>;
type CreateTopicsCommandTopic = ArrayElementType<Topics_CreateTopicsRequestParams['createTopicsCommand']['topics']>;

@Component({
  selector: 'sb-topics-edit-dplr',
  templateUrl: './topics-edit-dplr.component.html',
  styleUrls: ['./topics-edit-dplr.component.scss']
})
export class TopicsEditDplrComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    year: number;
    weekNumber: number;
    mode: TopicsDplrEditMode;
    classBookInfo: ClassBookInfoType;
    individualCurriculumStudents: ClassBookStudentNoms_GetNomsByTerm;
    topics: Topics_GetAllForWeek;
    topicsDplr: TopicsDplr_GetAllForWeek;
    curriculumsWithTopicPlan: number[];
    schedule: Schedules_GetClassBookScheduleForWeek;
  };

  @ViewChild('ngForm') ngForm!: NgForm;

  readonly destroyed$ = new Subject<void>();

  readonly TOPICS_EDIT_MODE_VIEW = TopicsDplrEditMode.View;
  readonly TOPICS_EDIT_MODE_NEW = TopicsDplrEditMode.New;
  readonly TOPICS_EDIT_MODE_REMOVE = TopicsDplrEditMode.Remove;
  readonly TOPICS_EDIT_MODE_REMOVE_TOPIC_DPLR = TopicsDplrEditMode.RemoveTopicDplr;

  readonly farCalendarCheck = farCalendarCheck;
  readonly fasPlus = fasPlus;
  readonly fadPlusCircle = fadPlusCircle;
  readonly fadUsers = fadUsers;
  readonly fadUserSlash = fadUserSlash;
  readonly fasUserEdit = fasUserEdit;
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasUndo = fasUndo;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fadSpinnerThird = fadSpinnerThird;
  readonly fadTrashAlt = fadTrashAlt;
  readonly fasAlignSlash = fasAlignSlash;
  readonly fasAlignJustify = fasAlignJustify;
  readonly fasCheck = fasCheck;
  readonly fadPencilAlt = fadPencilAlt;
  readonly fasArrowLeft = fasArrowLeft;

  readonly form = new FormRecord<FormControl<string | number[] | null>>({});

  weekPaginatorItems!: PaginatorItem[];
  individualCurriculumStudentControl = new FormControl<number | null>(null);
  selectedSchedule!: Schedules_GetClassBookScheduleForWeek | null;
  scheduleByDay!: ReturnType<typeof getScheduleByDay>;
  hasPastMonthLockMessage: string | null = null;
  modeTitle!: string;

  removedTopics: Topics_RemoveTopicsRequestParams['removeTopicsCommand']['topics'] = [];

  removingAct: { [key: number]: boolean } = {};

  canCreate = false;
  canRemove = false;

  constructor(
    private topicsService: TopicsService,
    private topicsDplsService: TopicsDplrService,
    private schedulesService: SchedulesService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    const weekStart = setISODay(setISOWeek(new Date(this.data.year, 7, 7), this.data.weekNumber), 1);
    const weekEnd = setISODay(setISOWeek(new Date(this.data.year, 7, 7), this.data.weekNumber), 7);

    const prevWeekStart = addWeeks(weekStart, -1);
    const prevWeekYear = getISOWeekYear(prevWeekStart);
    const prevWeekNumber = getISOWeek(prevWeekStart);

    const nextWeekStart = addWeeks(weekStart, 1);
    const nextWeekYear = getISOWeekYear(nextWeekStart);
    const nextWeekNumber = getISOWeek(nextWeekStart);

    this.weekPaginatorItems = [
      {
        icon: fasChevronLeft,
        routeCommands: ['../../', prevWeekYear, prevWeekNumber]
      },
      {
        text: `${format(weekStart, 'dd.MM', { locale: bg })} - ${format(weekEnd, 'dd.MM', { locale: bg })}`
      },
      {
        icon: fasChevronRight,
        routeCommands: ['../../', nextWeekYear, nextWeekNumber]
      }
    ];

    this.individualCurriculumStudentControl.valueChanges
      .pipe(
        switchMap((studentId) => {
          if (studentId) {
            return this.schedulesService
              .getClassBookScheduleForWeek({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                year: this.data.year,
                weekNumber: this.data.weekNumber,
                personId: studentId
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
          this.selectedSchedule = schedule;
          this.scheduleByDay = getScheduleByDay(this.data, schedule);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    this.scheduleByDay = getScheduleByDay(this.data, null);

    this.modeTitle =
      this.data.mode === TopicsDplrEditMode.New
        ? 'Въвеждане на теми'
        : this.data.mode === TopicsDplrEditMode.Remove
        ? 'Премахване на теми'
        : this.data.mode === TopicsDplrEditMode.RemoveTopicDplr
        ? 'Премахване на часове'
        : 'Въвеждане/Премахване на теми';

    const isClassBookLocked =
      this.data.classBookInfo.bookAllowsModifications &&
      this.data.classBookInfo.firstEditableMonthStartDate != null &&
      this.data.classBookInfo.firstEditableMonthStartDate > weekStart;

    if (isClassBookLocked) {
      this.hasPastMonthLockMessage = `${this.modeTitle} преди ${formatNullableDate(
        this.data.classBookInfo.firstEditableMonthStartDate
      )} е забранено`;
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  isTakenChange(event: MatCheckboxChange, hour: ScheduleHourWithTopic) {
    hour.isTaken = event.checked;
    this.setupTopicPlan(hour);
  }

  toggleUseTopicPlan(hour: ScheduleHourWithTopic) {
    hour.useTopicPlan = !hour.useTopicPlan;
    this.setupTopicPlan(hour);
  }

  setupTopicPlan(hour: ScheduleHourWithTopic) {
    if (hour.useTopicPlan) {
      this.setupFormControl(hour, 'topicPlanItemFormControl');
    } else {
      this.setupFormControl(hour, 'topicFormControl');
    }
  }

  setupFormControl(
    hour: ScheduleHourWithTopic,
    formControlKey: keyof Pick<ScheduleHourWithTopic, 'topicFormControl' | 'topicPlanItemFormControl'>
  ) {
    const scheduleLessonId = hour.scheduleLessonId?.toString() ?? throwError('hour must have scheduleLessonId');
    if (hour.isTaken) {
      let formControl: FormControl<string> | FormControl<number[] | null> | null;
      if (formControlKey === 'topicFormControl') {
        hour[formControlKey] = formControl = new FormControl('', {
          nonNullable: true,
          validators: [Validators.required]
        });
      } else {
        hour[formControlKey] = formControl = new FormControl<number[] | null>(null, {
          nonNullable: true,
          validators: [Validators.required]
        });
        hour.topicPlanItemsLoading = true;
        hour.topicPlanItems$ = this.topicsService
          .getCurriculumTopiPlan({
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            curriculumId: hour.curriculumId ?? throwError('hour must have curriculumId')
          })
          .pipe(finalize(() => (hour.topicPlanItemsLoading = false)));
      }

      if (this.form.contains(scheduleLessonId)) {
        this.form.setControl(scheduleLessonId, formControl);
      } else {
        this.form.addControl(scheduleLessonId, formControl);
      }
    } else {
      this.form.removeControl(scheduleLessonId);
    }
  }

  onTopicPlanSelectOpened() {
    // the dropdown is not rendered yet
    setTimeout(() => {
      const takenIconElements = document.querySelectorAll('#ng-select-dropdown-default .ng-fa-icon.icon-taken');
      if (!takenIconElements.length) {
        return;
      }

      const lastTakenIconElement = takenIconElements[takenIconElements.length - 1];
      const lastTakenOptionOffsetTop = lastTakenIconElement.parentElement!.offsetTop;

      const scrollHost = document.querySelector('#ng-select-dropdown-default .ng-dropdown-panel-items.scroll-host');
      if (!scrollHost) {
        return;
      }

      scrollHost.scrollTo({ top: lastTakenOptionOffsetTop, behavior: 'smooth' });
    }, 0);
  }

  remove(hour: ScheduleHourWithTopic) {
    assert(hour.topicId);
    assert(hour.scheduleLessonId);

    hour.isRemoved = true;

    this.removedTopics.push({
      topicId: hour.topicId,
      scheduleLessonId: hour.scheduleLessonId
    });
  }

  removeTopicsDplr(topicDplrId: number) {
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете избраният часа?',
        errorsMessage: 'Не може да изтриете часа, защото:',
        httpAction: () =>
          this.topicsDplsService
            .removeTopicDplr({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              removeTopicDplrCommand: {
                topicDplrId
              }
            })
            .toPromise()
      })
      .then((done) => {
        if (done) {
          this.data.topicsDplr.splice(
            this.data.topicsDplr.findIndex((t) => t.topicDplrId === topicDplrId),
            1
          );
          this.scheduleByDay = getScheduleByDay(this.data, null);
        }
      });
  }

  undoRemove(hour: ScheduleHourWithTopic) {
    assert(hour.topicId);

    hour.isRemoved = false;

    this.removedTopics.splice(
      this.removedTopics.findIndex(({ topicId }) => topicId === hour.topicId),
      1
    );
  }

  shouldPreventLeave() {
    return this.form.dirty || this.removedTopics.length > 0;
  }

  onSave(save: SaveToken) {
    let result: Promise<void>;

    switch (this.data.mode) {
      case TopicsDplrEditMode.New: {
        const addedTopics = Object.entries(this.form.controls)
          .filter(([, control]) => control.enabled)
          .map(([key, control]): CreateTopicsCommandTopic => {
            const scheduleLessonId = tryParseInt(key) ?? throwError('Key is not valid scheduleLessonId');
            const scheduleHour =
              (this.selectedSchedule ?? this.data.schedule).hours.find(
                (h) => h.scheduleLessonId === scheduleLessonId
              ) ?? throwError('No hour found by scheduleLessonId');

            let classBookTopicPlanItemIds: number[] | null = null;
            let title: string | null = null;

            if (typeof control.value === 'string') {
              title = control.value;
            } else if (control.value instanceof Array) {
              classBookTopicPlanItemIds = control.value as number[];
            } else {
              throw new Error('Invalid control value');
            }

            return {
              title,
              classBookTopicPlanItemIds,
              date: scheduleHour.date,
              scheduleLessonId: scheduleHour.scheduleLessonId,
              teacherAbsenceId: scheduleHour.teacherAbsenceId
            };
          });

        if (addedTopics.length === 0) {
          result = Promise.resolve();
        } else {
          result = this.topicsService
            .createTopics({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              createTopicsCommand: {
                topics: addedTopics
              }
            })
            .toPromise();
        }

        break;
      }

      case TopicsDplrEditMode.Remove: {
        if (this.removedTopics.length === 0) {
          result = Promise.resolve();
        } else {
          result = this.topicsService
            .removeTopics({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              removeTopicsCommand: {
                topics: this.removedTopics
              }
            })
            .toPromise();
        }

        break;
      }
    }

    this.actionService
      .execute({
        httpAction: () =>
          result.then(() => {
            this.form.markAsPristine();
            this.removedTopics = [];
            this.router.navigate(['../../../', this.data.year, this.data.weekNumber], { relativeTo: this.route });
          })
      })
      .then((success) => save.done(success));
  }

  openAddTopicDplrDialog(date: Date) {
    openTypedDialog(this.dialog, AddTopicDplrDialogSkeletonComponent, {
      data: {
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        date: date
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.topicsDplsService
            .getAllForWeek({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              year: this.data.year,
              weekNumber: this.data.weekNumber
            })
            .toPromise()
            .then((topicsDplr) => {
              this.data.topicsDplr = topicsDplr;
              this.scheduleByDay = getScheduleByDay(this.data, null);
            });
        }

        return Promise.resolve();
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));
  }

  getStudentsNames(students: TopicsGetAllForWeekVOStudent[]): string[] {
    return students.map((student) => `${student.studentFirstName} ${student.studentLastName}`);
  }
}

function getScheduleByDay(
  data: TopicsEditDplrComponent['data'],
  studentSchedule: Schedules_GetClassBookScheduleForWeek | null
) {
  const schedule = studentSchedule ?? data.schedule;
  const scheduleIncludesWeekend = schedule.hours.find((hour) => hour.day > 5) != null;

  return range(1, scheduleIncludesWeekend ? 7 : 5).map((day) => {
    const date = setISODay(setISOWeek(new Date(data.year, 7, 7), data.weekNumber), day);
    const isOffDay = schedule.offDays.includes(day);

    const dateTopics = data.topics.filter((t) => isSameDay(t.date, date));
    let hours: PartialScheduleHour[] = !isOffDay ? schedule.hours.filter((h) => h.day === day) : [];

    const bookAllowsAttendanceAbsenceTopicModifications =
      data.classBookInfo.checkBookAllowsAttendanceAbsenceTopicModifications(date);

    // add missing hour numbers
    if (hours.length) {
      const minHourNumber = min(hours, (h) => h.hourNumber) as number;
      const maxHourNumber = max(hours, (h) => h.hourNumber) as number;

      for (const hourNumber of range(minHourNumber, maxHourNumber)) {
        if (!hours.find((rh) => rh.hourNumber === hourNumber)) {
          hours.push({
            hourNumber
          });
        }
      }

      hours = stableSort(hours, (h1, h2) => h1.hourNumber - h2.hourNumber);
    }

    const topics = {
      date,
      day,
      dateString: format(date, 'dd.MM.yyyy', { locale: bg }),
      dayString: format(date, 'EEEE', { locale: bg }),
      isOffDay,
      canCreateTopicDplr: bookAllowsAttendanceAbsenceTopicModifications,
      hours: hours.map((h) => {
        const topic = dateTopics.find((t) => t.scheduleLessonId === h.scheduleLessonId);
        const hasTopicPlan = h.curriculumId != null ? data.curriculumsWithTopicPlan.includes(h.curriculumId) : false;

        return {
          ...extendScheduleHour(h),
          isTaken: topic != null,
          scheduleLessonId: h.scheduleLessonId ?? null,
          topicId: topic?.topicId ?? null,
          topicDplrId: null,
          topicTitles: topic?.titles ?? [],
          topicTeacher: topic?.teachers?.length
            ? topic.teachers
                .map((t) => `${t.teacherFirstName} ${t.teacherLastName}` + (t.isReplTeacher ? '(зам.)' : ''))
                .join(', ')
            : null,
          topicTeacherShort: topic?.teachers?.length
            ? topic.teachers.map((t) => t.teacherLastName + (t.isReplTeacher ? '(зам.)' : '')).join(', ')
            : null,
          topicStudents: topic?.students.length ? topic.students : [],
          hasTopicPlan,
          useTopicPlan: hasTopicPlan,
          topicPlanItems$: <
            Observable<Array<{ classBookTopicPlanItemId: number; title: string; taken: boolean }>> | null
          >null,
          topicPlanItemsLoading: <boolean | null>null,
          topicFormControl: <FormControl<string> | null>null,
          topicPlanItemFormControl: <FormControl<number[] | null> | null>null,
          canCreate: topic == null && bookAllowsAttendanceAbsenceTopicModifications && h.hasTopicCreateAccess,
          canRemove: topic != null && bookAllowsAttendanceAbsenceTopicModifications && topic.hasRemoveAccess,
          canRemoveTopicDplr: false,
          canUndo: topic != null && bookAllowsAttendanceAbsenceTopicModifications && topic.hasUndoAccess,
          undoExpired$:
            topic != null && bookAllowsAttendanceAbsenceTopicModifications
              ? expiredAt(add(topic.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }))
              : of(true),
          isRemoved: false
        };
      })
    };

    let dateTopicsDplr = data.topicsDplr.filter((t) => isSameDay(t.date, date));
    dateTopicsDplr = stableSort(dateTopicsDplr, (t1, t2) => t1.hourNumber - t2.hourNumber);

    const topicsDplr = {
      date,
      day,
      dateString: format(date, 'dd.MM.yyyy', { locale: bg }),
      dayString: format(date, 'EEEE', { locale: bg }),
      isOffDay,
      canCreateTopicDplr: bookAllowsAttendanceAbsenceTopicModifications,
      hours: dateTopicsDplr.map((h) => {
        const topicDplr = dateTopicsDplr.find((t) => t.topicDplrId === h.topicDplrId);

        return {
          ...extendScheduleHour(h),
          isTaken: topicDplr != null,
          scheduleLessonId: null,
          topicId: null,
          topicDplrId: h.topicDplrId,
          topicTitles: topicDplr?.title ? [topicDplr.title] : [],
          topicTeacher: topicDplr?.teachers?.length
            ? topicDplr.teachers.map((t) => `${t.teacherFirstName} ${t.teacherLastName}`).join(', ')
            : null,
          topicTeacherShort: topicDplr?.teachers?.length
            ? topicDplr.teachers.map((t) => t.teacherLastName).join(', ')
            : null,
          extTeacherName: null,
          isEmptyHour: null,
          topicStudents: topicDplr?.students.length ? topicDplr.students : [],
          hasTopicPlan: false,
          useTopicPlan: false,
          topicPlanItems$: <
            Observable<Array<{ classBookTopicPlanItemId: number; title: string; taken: boolean }>> | null
          >null,
          topicPlanItemsLoading: <boolean | null>null,
          topicFormControl: <FormControl<string> | null>null,
          topicPlanItemFormControl: <FormControl<number[] | null> | null>null,
          canCreate: topicDplr == null && bookAllowsAttendanceAbsenceTopicModifications && h.hasTopicCreateAccess,
          canRemove: false,
          canRemoveTopicDplr:
            topicDplr != null && bookAllowsAttendanceAbsenceTopicModifications && topicDplr.hasTopicRemoveAccess,
          canUndo: topicDplr != null && bookAllowsAttendanceAbsenceTopicModifications && topicDplr.hasUndoAccess,
          undoExpired$:
            topicDplr != null && bookAllowsAttendanceAbsenceTopicModifications
              ? expiredAt(add(topicDplr.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }))
              : of(true),
          isRemoved: false
        };
      })
    };

    // Concatenate topics and topicsDplr hours with consistent types
    return {
      ...topics,
      isDplrTopics: topicsDplr.hours.length > 0 ? true : false,
      hours: topicsDplr.hours.length > 0 ? [...topicsDplr.hours] : [...topics.hours, ...topicsDplr.hours]
    };
  });
}
