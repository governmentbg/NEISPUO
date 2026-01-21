import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faTasksAlt as fadTasksAlt } from '@fortawesome/pro-duotone-svg-icons/faTasksAlt';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { format } from 'date-fns';
import { bg } from 'date-fns/locale';
import { ClassBooksService, ClassBooks_GetRemovedStudents } from 'projects/sb-api-client/src/api/classBooks.service';
import {
  MyHourService,
  MyHour_GetTeacherLesson,
  MyHour_GetTeacherLessonAbsences,
  MyHour_GetTeacherLessonGrades
} from 'projects/sb-api-client/src/api/myHour.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ClassBookInfoType, extendClassBookInfo, resolveWithRemovedStudents } from 'projects/shared/utils/book';
import { formatDate } from 'projects/shared/utils/date';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { map } from 'rxjs/operators';
import { MyHourLessonViewMode } from 'src/app/teacher/my-hour/my-hour-lesson-view-content/my-hour-lesson-view-content.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookVerificationLessonViewSkeletonComponent extends SkeletonComponentBase {
  constructor(classBooksService: ClassBooksService, myHourService: MyHourService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const month = tryParseInt(route.snapshot.paramMap.get('month')) ?? throwParamError('month');
    const day = tryParseInt(route.snapshot.paramMap.get('day')) ?? throwParamError('day');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const scheduleLessonId =
      tryParseInt(route.snapshot.paramMap.get('scheduleLessonId')) ?? throwParamError('scheduleLessonId');
    const date = new Date(year, month - 1, day);

    this.resolve(
      BookVerificationLessonViewComponent,
      resolveWithRemovedStudents(
        classBooksService,
        (data) => data.teacherLesson.students,
        (data) => [
          ...data.absences.map((a) => ({
            personId: a.personId
          })),
          ...data.grades.map((g) => ({
            personId: g.personId
          }))
        ],
        {
          schoolYear,
          instId,
          classBookId,
          year,
          month,
          day,
          date,
          scheduleLessonId,
          classBookInfo: classBooksService
            .get({
              schoolYear,
              instId,
              classBookId
            })
            .pipe(map(extendClassBookInfo)),
          teacherLesson: myHourService.getTeacherLesson({
            schoolYear,
            instId,
            classBookId,
            scheduleLessonId
          }),
          absences: myHourService.getTeacherLessonAbsences({
            schoolYear,
            instId,
            classBookId,
            scheduleLessonId
          }),
          grades: myHourService.getTeacherLessonGrades({
            schoolYear,
            instId,
            classBookId,
            scheduleLessonId
          })
        }
      )
    );
  }
}

@Component({
  selector: 'sb-book-verification-lesson-view',
  templateUrl: './book-verification-lesson-view.component.html'
})
export class BookVerificationLessonViewComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    year: number;
    month: number;
    day: number;
    date: Date;
    scheduleLessonId: number;
    classBookInfo: ClassBookInfoType;
    teacherLesson: MyHour_GetTeacherLesson;
    absences: MyHour_GetTeacherLessonAbsences;
    grades: MyHour_GetTeacherLessonGrades;
    removedStudents: ClassBooks_GetRemovedStudents;
  };

  readonly fadTasksAlt = fadTasksAlt;
  readonly fasArrowLeft = fasArrowLeft;

  readonly MyHourLessonViewMode = MyHourLessonViewMode.BookVerification;

  templateData!: ReturnType<typeof getTemplateData>;

  ngOnInit() {
    this.templateData = getTemplateData(this.data);
  }
}

function getTemplateData(data: BookVerificationLessonViewComponent['data']) {
  const date = new Date(data.year, data.month - 1, data.day);

  return {
    dateName: formatDate(date),
    lessonName: `${data.classBookInfo.fullBookName} #${data.teacherLesson.hourNumber} (${data.teacherLesson.startTime} - ${data.teacherLesson.endTime})`,
    monthName: format(date, 'MMMM', { locale: bg })
  };
}
