import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarCheck as farCalendarCheck } from '@fortawesome/pro-regular-svg-icons/faCalendarCheck';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetAbsencesDplr,
  StudentInfoClassBook_GetAbsencesForCurriculumAndType
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import { AbsenceType } from 'projects/sb-api-client/src/model/absenceType';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

export enum AbsencesDplrType {
  Absence = 'Absence',
  Attendance = 'Attendance'
}

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AbsencesDplrSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');
    const type = route.snapshot.data.type as AbsencesDplrType;
    const absenceType = type === AbsencesDplrType.Absence ? AbsenceType.DplrAbsence : AbsenceType.DplrAttendance;

    this.resolve(AbsencesDplrComponent, {
      schoolYear,
      instId,
      studentClassBookId,
      classBookId,
      personId,
      absenceType,
      absences: studentInfoClassBookService.getAbsencesDplr({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId,
        type: absenceType
      })
    });
  }
}

type Absence = ArrayElementType<StudentInfoClassBook_GetAbsencesForCurriculumAndType>;

@Component({
  selector: 'sb-absences-dplr',
  templateUrl: './absences-dplr.component.html'
})
export class AbsencesDplrComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    personId: number;
    studentClassBookId: number;
    absenceType: AbsenceType;
    absences: StudentInfoClassBook_GetAbsencesDplr;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly farCalendarTimes = farCalendarTimes;
  readonly farCalendarCheck = farCalendarCheck;

  studentAbsences!: ReturnType<typeof getStudentAbsences>;

  selectedCurriculumId?: number;
  selectedAbsences?: Absence[];
  loadingAbsences = false;

  isAbsence = false;

  constructor(private studentInfoClassBookService: StudentInfoClassBookService) {}

  ngOnInit() {
    this.studentAbsences = getStudentAbsences(this.data);
    this.isAbsence = this.data.absenceType === AbsenceType.DplrAbsence;
  }

  viewAbsences(curriculumId: number) {
    this.selectedAbsences = undefined;

    if (this.selectedCurriculumId === curriculumId) {
      this.selectedCurriculumId = undefined;

      return;
    }

    this.selectedCurriculumId = curriculumId;
    this.loadingAbsences = true;

    return this.studentInfoClassBookService
      .getAbsencesForCurriculumAndType({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
        studentClassBookId: this.data.studentClassBookId,
        curriculumId: this.selectedCurriculumId,
        type: this.data.absenceType
      })
      .toPromise()
      .then((absences) => {
        this.selectedAbsences = absences;
      })
      .catch((err) => {
        this.selectedCurriculumId = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingAbsences = false;
      });
  }
}

function getStudentAbsences(data: AbsencesDplrComponent['data']) {
  const total = data.absences.reduce((total, c) => total + c.count, 0);

  return {
    absences: data.absences,
    total
  };
}
