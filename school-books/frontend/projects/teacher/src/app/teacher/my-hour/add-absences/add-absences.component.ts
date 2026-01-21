import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faChalkboardTeacher as fadChalkboardTeacher } from '@fortawesome/pro-duotone-svg-icons/faChalkboardTeacher';
import { faCog as fadCog } from '@fortawesome/pro-duotone-svg-icons/faCog';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import { add, isValid, parse } from 'date-fns';
import { AbsencesService } from 'projects/sb-api-client/src/api/absences.service';
import { ClassBooksService } from 'projects/sb-api-client/src/api/classBooks.service';
import {
  MyHourService,
  MyHour_GetTeacherLesson,
  MyHour_GetTeacherLessonAbsences
} from 'projects/sb-api-client/src/api/myHour.service';
import { AbsenceType } from 'projects/sb-api-client/src/model/absenceType';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { CreateAbsenceCommand } from 'projects/sb-api-client/src/model/createAbsenceCommand';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { assert } from 'projects/shared/utils/assert';
import {
  ClassBookInfoType,
  extendClassBookInfo,
  getTermFromDate,
  UNDO_INTERVAL_IN_MINUTES
} from 'projects/shared/utils/book';
import { formatDate, isExpired } from 'projects/shared/utils/date';
import { expiredAt } from 'projects/shared/utils/rxjs';
import { throwError, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { combineLatest, merge, of, Subject } from 'rxjs';
import { filter, map, take, takeUntil, tap } from 'rxjs/operators';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class AddAbsencesSkeletonComponent extends SkeletonComponentBase {
  constructor(route: ActivatedRoute, myHourService: MyHourService, classBooksService: ClassBooksService) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const scheduleLessonId =
      tryParseInt(route.snapshot.paramMap.get('scheduleLessonId')) ?? throwParamError('scheduleLessonId');
    const date = parse(route.snapshot.paramMap.get('date') ?? throwParamError('date'), 'yyyy-MM-dd', new Date(), {});
    if (!isValid(date)) {
      throwParamError('date');
    }

    this.resolve(AddAbsencesComponent, {
      schoolYear,
      instId,
      classBookId,
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
      })
    });
  }
}

type StudentAbsenceForm = {
  personId: FormControl<number>;
  absenceType: FormControl<AbsenceType | null>;
};

@Component({
  selector: 'sb-add-absences',
  templateUrl: './add-absences.component.html'
})
export class AddAbsencesComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    date: Date;
    scheduleLessonId: number;
    classBookInfo: ClassBookInfoType;
    teacherLesson: MyHour_GetTeacherLesson;
    absences: MyHour_GetTeacherLessonAbsences;
  };

  readonly fadChalkboardTeacher = fadChalkboardTeacher;
  readonly fadCog = fadCog;
  readonly fasTimes = fasTimes;

  readonly ABSENCE_TYPE_LATE = AbsenceType.Late;
  readonly ABSENCE_TYPE_UNEXCUSED = AbsenceType.Unexcused;
  readonly ABSENCE_TYPE_EXCUSED = AbsenceType.Excused;
  readonly ABSENCE_TYPE_DPLR_ABSENCE = AbsenceType.DplrAbsence;
  readonly ABSENCE_TYPE_DPLR_ATTENDANCE = AbsenceType.DplrAttendance;

  readonly CLASS_BOOK_TYPE_BOOK_DPLR = ClassBookType.Book_DPLR;

  readonly destroyed$ = new Subject<void>();

  saving = false;
  hasComments = false;
  templateData!: ReturnType<typeof getTemplateData>;
  form!: FormGroup<{
    students: FormArray<FormGroup<StudentAbsenceForm>>;
  }>;
  hourDescriptionShort = '';
  hourDescription = '';
  absencesUndoBannerHidden = false;
  isDplr = false;

  constructor(
    public errorStateMatcher: ErrorStateMatcher,
    public localStorageService: LocalStorageService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private absencesService: AbsencesService
  ) {}

  ngOnInit() {
    this.hourDescriptionShort = `${formatDate(this.data.date)} час #${this.data.teacherLesson.hourNumber}`;
    this.hourDescription = `${this.hourDescriptionShort} (${this.data.teacherLesson.startTime} - ${this.data.teacherLesson.endTime})`;
    this.isDplr = this.data.classBookInfo.bookType === ClassBookType.Book_DPLR;

    this.absencesUndoBannerHidden = this.localStorageService.getAbsencesUndoBannerHidden();
    this.localStorageService.absencesUndoBannerHidden$
      .pipe(
        filter((absencesUndoBannerHidden) => absencesUndoBannerHidden),
        tap(() => this.hideAbsencesUndoBanner()),
        take(1),
        takeUntil(
          merge(
            // unsubscribe if the user hides the banner himself
            this.localStorageService.absencesUndoBannerHidden$.pipe(
              filter((absencesUndoBannerHidden) => absencesUndoBannerHidden)
            ),
            this.destroyed$
          )
        )
      )
      .subscribe();

    this.form = this.fb.group({
      students: this.fb.nonNullable.array<FormGroup<StudentAbsenceForm>>([])
    });

    for (const student of this.data.teacherLesson.students) {
      const currentAbsence = this.data.absences.find((a) => a.personId === student.personId);

      this.studentsFormArray().push(
        this.fb.group<StudentAbsenceForm>({
          personId: this.fb.nonNullable.control(student.personId),
          absenceType: this.fb.control<AbsenceType | null>(currentAbsence?.type ?? null)
        })
      );
    }

    // update visible students
    combineLatest([
      this.localStorageService.bookTransferredHidden$,
      this.localStorageService.bookNotEnrolledHidden$,
      this.localStorageService.bookGradelessHidden$
    ])
      .pipe(
        tap(([bookTransferredHidden, bookNotEnrolledHidden, bookGradelessHidden]) => {
          this.templateData = getTemplateData(
            this.data,
            this.studentsFormArray(),
            bookTransferredHidden,
            bookNotEnrolledHidden,
            bookGradelessHidden
          );

          this.data.teacherLesson.students.forEach((student, i) => {
            const mappedStudent = this.templateData.students.find((s) => s.personId === student.personId);

            assert(
              mappedStudent == null || mappedStudent.studentFormIndex === i,
              'Mapped student formIndex not in sync'
            );

            if (mappedStudent) {
              this.studentForm(i).enable();
            } else {
              this.studentForm(i).disable();
            }
          });
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  studentsFormArray() {
    return this.form.get('students') as FormArray<FormGroup<StudentAbsenceForm>>;
  }

  studentForm(i: number) {
    return this.studentsFormArray().at(i) as FormGroup<StudentAbsenceForm>;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  hideAbsencesUndoBanner() {
    this.absencesUndoBannerHidden = true;
    this.localStorageService.setAbsencesUndoBannerHidden(true);
  }

  getAbsenceType(studentFormIndex: number) {
    return this.studentForm(studentFormIndex).value.absenceType;
  }

  setAbsenceType(studentFormIndex: number, absenceType: AbsenceType | null) {
    this.studentForm(studentFormIndex).patchValue({ absenceType });
  }

  onSave(save: SaveToken) {
    const absences: { personId: number; type?: AbsenceType | null; undoAbsenceId?: number | null }[] = [];

    for (const student of this.form.value.students ?? []) {
      assert(student.personId);

      const currentAbsence = this.data.absences.find((a) => a.personId === student.personId);

      if (
        currentAbsence != null &&
        (!currentAbsence.hasUndoAccess ||
          isExpired(add(currentAbsence.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES })) ||
          currentAbsence.type === student.absenceType)
      ) {
        continue;
      }

      if (currentAbsence != null) {
        absences.push({ personId: student.personId, undoAbsenceId: currentAbsence.absenceId });
      }

      if (student.absenceType != null) {
        absences.push({ personId: student.personId, type: student.absenceType });
      }
    }

    const createAbsenceCommand: CreateAbsenceCommand = {
      absences:
        absences.map((a) => ({
          ...a,
          date: this.data.date,
          scheduleLessonId: this.data.scheduleLessonId,
          teacherAbsenceId: this.data.teacherLesson.teacherAbsenceId
        })) ?? []
    };

    this.actionService
      .execute({
        httpAction: () =>
          this.absencesService
            .createAbsence({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              createAbsenceCommand
            })
            .toPromise()
            .then(() => {
              this.form.markAsPristine();
              this.router.navigate(['../'], { relativeTo: this.route });
            })
      })
      .then((success) => save.done(success));
  }
}

function getTemplateData(
  data: AddAbsencesComponent['data'],
  studentsFormArray: FormArray<FormGroup<StudentAbsenceForm>>,
  bookTransferredHidden: boolean,
  bookNotEnrolledHidden: boolean,
  bookGradelessHidden: boolean
) {
  const schoolTerm = getTermFromDate(data.classBookInfo, data.date) ?? throwError('Date outside school term');

  const students = data.teacherLesson.students
    .map((s, i) => {
      const studentFormValue = studentsFormArray.at(i).value;
      const hasSelectedAbsence = studentFormValue.absenceType != null;
      const currentAbsence = data.absences.find((a) => a.personId === s.personId);

      const isGradeless =
        (schoolTerm === SchoolTerm.TermOne && s.withoutFirstTermGrade) ||
        (schoolTerm === SchoolTerm.TermTwo && s.withoutSecondTermGrade) ||
        (s.withoutFirstTermGrade && s.withoutSecondTermGrade);

      return {
        ...s,

        hasCurrentAbsence: currentAbsence != null,
        currentAbsenceType: currentAbsence?.type,
        currentAbsenceCanUndo: currentAbsence?.hasUndoAccess ?? false,
        currentAbsenceUndoExpired$:
          currentAbsence != null
            ? expiredAt(add(currentAbsence.createDate, { minutes: UNDO_INTERVAL_IN_MINUTES }))
            : of(true),
        hasSelectedAbsence,
        studentFormIndex: i,
        isGradeless,
        abnormalStatus: s.isTransferred
          ? 'ОТПИСАН'
          : s.notEnrolledInCurriculum
          ? 'НЕ ИЗУЧАВА ПРЕДМЕТА'
          : isGradeless
          ? 'ОСВОБОДЕН'
          : ''
      };
    })
    .filter(
      (s) =>
        (data.teacherLesson.individualCurriculumPersonId == null ||
          s.personId === data.teacherLesson.individualCurriculumPersonId) &&
        (s.hasSelectedAbsence ||
          ((!bookTransferredHidden || !s.isTransferred) &&
            (!bookNotEnrolledHidden || !s.notEnrolledInCurriculum) &&
            (!bookGradelessHidden || !s.isGradeless)))
    );

  return {
    students
  };
}
