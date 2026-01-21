import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetAbsences,
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

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AbsencesSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');

    this.resolve(AbsencesComponent, {
      schoolYear,
      instId,
      classBookId,
      personId,
      studentClassBookId,
      absences: studentInfoClassBookService.getAbsences({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId
      })
    });
  }
}

type Absence = ArrayElementType<StudentInfoClassBook_GetAbsencesForCurriculumAndType>;

@Component({
  selector: 'sb-absences',
  templateUrl: './absences.component.html'
})
export class AbsencesComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    personId: number;
    studentClassBookId: number;
    absences: StudentInfoClassBook_GetAbsences;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly farCalendarTimes = farCalendarTimes;

  readonly ABSENCE_TYPE_LATE = AbsenceType.Late;
  readonly ABSENCE_TYPE_UNEXCUSED = AbsenceType.Unexcused;
  readonly ABSENCE_TYPE_EXCUSED = AbsenceType.Excused;

  studentAbsences!: ReturnType<typeof getStudentAbsences>;

  selectedCurriculumId?: number;
  selectedType?: AbsenceType;
  selectedAbsences?: Absence[];
  loadingAbsences = false;

  constructor(private studentInfoClassBookService: StudentInfoClassBookService) {}

  ngOnInit() {
    this.studentAbsences = getStudentAbsences(this.data);
  }

  viewAbsences(curriculumId: number, type: AbsenceType) {
    this.selectedAbsences = undefined;

    if (this.selectedCurriculumId === curriculumId && this.selectedType === type) {
      this.selectedCurriculumId = undefined;
      this.selectedType = undefined;

      return;
    }

    this.selectedCurriculumId = curriculumId;
    this.selectedType = type;
    this.loadingAbsences = true;

    return this.studentInfoClassBookService
      .getAbsencesForCurriculumAndType({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
        studentClassBookId: this.data.studentClassBookId,
        curriculumId: this.selectedCurriculumId,
        type: this.selectedType
      })
      .toPromise()
      .then((absences) => {
        this.selectedAbsences = absences;
      })
      .catch((err) => {
        this.selectedCurriculumId = undefined;
        this.selectedType = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingAbsences = false;
      });
  }
}

function getStudentAbsences(data: AbsencesComponent['data']) {
  const totals = data.absences.reduce(
    (totals, s) => {
      totals.lateAbsencesCountTotal += s.lateAbsencesCount;
      totals.unexcusedAbsencesCountTotal += s.unexcusedAbsencesCount;
      totals.excusedAbsencesCountTotal += s.excusedAbsencesCount;

      return totals;
    },
    {
      lateAbsencesCountTotal: 0,
      unexcusedAbsencesCountTotal: 0,
      excusedAbsencesCountTotal: 0
    }
  );

  return {
    absences: data.absences,
    totals
  };
}
