import { animate, style, transition, trigger } from '@angular/animations';
import { Component, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCheckSquare as fadCheckSquare } from '@fortawesome/pro-duotone-svg-icons/faCheckSquare';
import { faSpinnerThird as fadSpinnerThird } from '@fortawesome/pro-duotone-svg-icons/faSpinnerThird';
import { faTasksAlt as fadTasksAlt } from '@fortawesome/pro-duotone-svg-icons/faTasksAlt';
import { faCalendarMinus as farCalendarMinus } from '@fortawesome/pro-regular-svg-icons/faCalendarMinus';
import { faCalendarPlus as farCalendarPlus } from '@fortawesome/pro-regular-svg-icons/faCalendarPlus';
import { faCircle as farCircle } from '@fortawesome/pro-regular-svg-icons/faCircle';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheckCircle as fasCheckCircle } from '@fortawesome/pro-solid-svg-icons/faCheckCircle';
import { faChevronRight as fasChevronRight } from '@fortawesome/pro-solid-svg-icons/faChevronRight';
import { faUsersClass as fasUsersClass } from '@fortawesome/pro-solid-svg-icons/faUsersClass';
import { format } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  BookVerificationService,
  BookVerification_GetOffDaysForDay,
  BookVerification_GetScheduleLessonsForDay,
  BookVerification_UpdateIsVerifiedRequestParams
} from 'projects/sb-api-client/src/api/bookVerification.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { groupBy } from 'projects/shared/utils/array';
import { classBookHasTab } from 'projects/shared/utils/book-tabs';
import { formatDate } from 'projects/shared/utils/date';
import { getLessonName, getLessonNameShort } from 'projects/shared/utils/schedule';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookVerificationDayViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    bookVerificationService: BookVerificationService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const month = tryParseInt(route.snapshot.paramMap.get('month')) ?? throwParamError('month');
    const day = tryParseInt(route.snapshot.paramMap.get('day')) ?? throwParamError('day');
    const classBookId = tryParseInt(route.snapshot.queryParamMap.get('c'));
    const teacherPersonId = tryParseInt(route.snapshot.queryParamMap.get('t'));

    this.resolve(BookVerificationDayViewComponent, {
      schoolYear,
      instId,
      year,
      month,
      day,
      scheduleLessons: bookVerificationService.getScheduleLessonsForDay({
        schoolYear,
        instId,
        year,
        month,
        day,
        classBookId,
        teacherPersonId
      }),
      institutionInfo: from(institutionInfo),
      offDays: bookVerificationService.getOffDaysForDay({
        schoolYear,
        instId,
        year,
        month,
        day,
        classBookId
      })
    });
  }
}

type Group = ArrayElementType<ReturnType<typeof getTemplateData>['groups']>;
type ScheduleLesson = ArrayElementType<Group['scheduleLessons']>;

@Component({
  selector: 'sb-book-verification-day-view',
  templateUrl: './book-verification-day-view.component.html',
  styleUrls: ['./book-verification-day-view.component.scss'],
  animations: [
    trigger('inOutAnimation', [
      transition(':enter', [
        style({ 'max-height': 0, opacity: 0, 'margin-bottom': 0 }),
        animate('0.2s ease-in', style({ 'max-height': 400, opacity: 1, 'margin-bottom': '16px' }))
      ]),
      transition(':leave', [
        style({ 'max-height': 400, opacity: 1, 'margin-bottom': '16px' }),
        animate('0.2s ease-out', style({ 'max-height': 0, opacity: 0, 'margin-bottom': 0 }))
      ])
    ])
  ]
})
export class BookVerificationDayViewComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    year: number;
    month: number;
    day: number;
    scheduleLessons: BookVerification_GetScheduleLessonsForDay;
    institutionInfo: InstitutionInfoType;
    offDays: BookVerification_GetOffDaysForDay;
  };

  readonly fadTasksAlt = fadTasksAlt;
  readonly fadSpinnerThird = fadSpinnerThird;
  readonly farCalendarMinus = farCalendarMinus;
  readonly farCalendarPlus = farCalendarPlus;
  readonly farCircle = farCircle;
  readonly fasArrowLeft = fasArrowLeft;
  readonly fasUsersClass = fasUsersClass;
  readonly fasCheckCircle = fasCheckCircle;
  readonly fadCheckSquare = fadCheckSquare;
  readonly fasChevronRight = fasChevronRight;

  canEdit = false;
  allGroupsExpanded = false;
  allGroupsCollapsed = false;

  templateData!: ReturnType<typeof getTemplateData>;

  constructor(private bookVerificationService: BookVerificationService, private actionService: ActionService) {}

  ngOnInit() {
    this.canEdit =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasVerificationWriteAccess;
    this.templateData = getTemplateData(this.data);
    this.syncExpandedCollapsed();
  }

  expandAllGroups() {
    this.toggleAllGroups(true);
  }

  collapseAllGroups() {
    this.toggleAllGroups(false);
  }

  toggleAllGroups(isExpanded: boolean) {
    for (const group of this.templateData.groups) {
      group.isExpanded = isExpanded;
    }

    this.syncExpandedCollapsed();
  }

  syncExpandedCollapsed() {
    this.allGroupsExpanded = this.checkAllGroups((g) => g.isExpanded);
    this.allGroupsCollapsed = this.checkAllGroups((g) => !g.isExpanded);
  }

  checkAllGroups(predicate: (g: Group) => boolean) {
    for (const group of this.templateData.groups) {
      if (!predicate(group)) {
        return false;
      }
    }

    return true;
  }

  toggleGroupExpanded(group: Group) {
    group.isExpanded = !group.isExpanded;

    this.syncExpandedCollapsed();
  }

  verifyGroup(group: Group) {
    group.verifying = of(true).pipe(delay(1000));

    const updateTakenParams: BookVerification_UpdateIsVerifiedRequestParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      updateIsVerifiedScheduleLessonCommand: {
        scheduleLessons: group.scheduleLessons.map((sl) => ({
          scheduleLessonId: sl.scheduleLessonId,
          isVerified: true
        }))
      }
    };

    this.actionService
      .execute({
        httpAction: () => this.bookVerificationService.updateIsVerified(updateTakenParams).toPromise()
      })
      .then((done) => {
        if (done) {
          for (const scheduleLesson of group.scheduleLessons) {
            scheduleLesson.isVerified = true;
          }
          group.lessonsVerified = group.scheduleLessons.length;
          group.isExpanded = false;

          group.verifying = null;
        } else {
          group.verifying = null;
        }
      });
  }

  setIsVerified(scheduleLesson: ScheduleLesson, group: Group, isVerified: boolean) {
    scheduleLesson.verifying = of(true).pipe(delay(1000));

    const updateTakenParams: BookVerification_UpdateIsVerifiedRequestParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      updateIsVerifiedScheduleLessonCommand: {
        scheduleLessons: [
          {
            scheduleLessonId: scheduleLesson.scheduleLessonId,
            isVerified
          }
        ]
      }
    };

    this.actionService
      .execute({
        httpAction: () => this.bookVerificationService.updateIsVerified(updateTakenParams).toPromise()
      })
      .then((done) => {
        if (done) {
          scheduleLesson.isVerified = isVerified;
          if (isVerified) {
            group.lessonsVerified++;
            if (group.lessonsVerified === group.lessonsTotal) {
              group.isExpanded = false;
            }
          } else {
            group.lessonsVerified--;
          }

          scheduleLesson.verifying = null;
        } else {
          scheduleLesson.verifying = null;
        }
      });
  }
}

function getTemplateData(data: BookVerificationDayViewComponent['data']) {
  const date = new Date(data.year, data.month - 1, data.day);

  const classBooks = [
    ...new Map(
      data.scheduleLessons.map((d) => [
        d.classBookId,
        { classBookId: d.classBookId, classBookFullName: d.classBookFullName, classBookType: d.classBookType }
      ])
    )
  ];
  const scheduleLessonsByClassBook = new Map(groupBy(data.scheduleLessons, (d) => d.classBookId));

  return {
    dateName: formatDate(date),
    monthName: format(date, 'MMMM', { locale: bg }),
    groups: classBooks.map(([classBookId, classBook], i) => {
      const scheduleLessons = (scheduleLessonsByClassBook.get(classBookId) ?? throwError('Missing classBookId')).map(
        (sl) => ({
          ...sl,
          teacherNames:
            sl.extReplTeacherName ?? sl.teachers.map((t) => `${t.teacherFirstName} ${t.teacherLastName}`).join(','),
          lessonName: getLessonName(sl),
          lessonNameShort: getLessonNameShort(sl),
          verifying: null as Observable<boolean> | null
        })
      );

      const offDay = data.offDays.find((od) => od.classBookId === classBookId);

      const classBookHasAbsences = classBookHasTab(false, { bookType: classBook.classBookType }, 'absences');
      const classBookHasAbsencesDplr = classBookHasTab(false, { bookType: classBook.classBookType }, 'absencesDplr');
      const classBookHasGrades = classBookHasTab(false, { bookType: classBook.classBookType }, 'grades');

      const lessonsTotal = scheduleLessons.length;
      const lessonsTaken = scheduleLessons.filter((sl) => sl.isTaken).length;
      const lessonsVerified = scheduleLessons.filter((sl) => sl.isVerified).length;

      return {
        isOffDay: offDay != null,
        offDayPeriod:
          offDay != null
            ? offDay.from === offDay.to
              ? formatDate(offDay.from)
              : formatDate(offDay.from) + '-' + formatDate(offDay.to)
            : null,
        description: offDay?.description,
        hasData: scheduleLessons.some((sl) => sl.isTaken || sl.absencesCount || sl.lateAbsencesCount || sl.gradesCount),
        classBookId,
        classBookFullName: classBook.classBookFullName,
        classBookHasAbsences,
        classBookHasAbsencesDplr,
        classBookHasGrades,
        isExpanded: lessonsVerified !== lessonsTotal,
        lessonsTotal,
        lessonsTaken,
        lessonsVerified,
        scheduleLessons,
        verifying: null as Observable<boolean> | null
      };
    })
  };
}
