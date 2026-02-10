import { Component, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faPencilAlt as fasPencilAlt } from '@fortawesome/pro-solid-svg-icons/faPencilAlt';
import { faPlusSquare as fasPlusSquare } from '@fortawesome/pro-solid-svg-icons/faPlusSquare';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetClassBookInfo,
  StudentInfoClassBook_GetGrade,
  StudentInfoClassBook_GetGrades
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import { GradeCategory } from 'projects/sb-api-client/src/model/gradeCategory';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { BreakpointService } from 'projects/shared/services/breakpoint.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { CLASS_BOOK_INFO } from '../student-book/student-book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class GradesSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<StudentInfoClassBook_GetClassBookInfo>,
    route: ActivatedRoute,
    studentInfoClassBookService: StudentInfoClassBookService
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');

    this.resolve(GradesComponent, {
      schoolYear,
      instId,
      classBookId,
      personId,
      studentClassBookId,
      grades: studentInfoClassBookService.getGrades({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId
      }),
      classBookInfo: from(classBookInfo)
    });
  }
}

@Component({
  selector: 'sb-grades',
  templateUrl: './grades.component.html',
  styleUrls: ['./grades.component.scss']
})
export class GradesComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    personId: number;
    studentClassBookId: number;
    grades: StudentInfoClassBook_GetGrades;
    classBookInfo: StudentInfoClassBook_GetClassBookInfo;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly fasPencilAlt = fasPencilAlt;
  readonly fasPlusSquare = fasPlusSquare;
  readonly GRADE_CATEGORY_DECIMAL = GradeCategory.Decimal;
  readonly GRADE_CATEGORY_QUALITATIVE = GradeCategory.Qualitative;
  readonly GRADE_CATEGORY_SPECIAL_NEEDS = GradeCategory.SpecialNeeds;

  selectedCurriculumId?: number;
  selectedGradeId?: number;
  selectedGrade?: StudentInfoClassBook_GetGrade;
  loadingGrade = false;
  showFinalGrades!: boolean;

  constructor(
    public breakpointService: BreakpointService,
    private studentInfoClassBookService: StudentInfoClassBookService
  ) {}

  selectStudentGrade(selectedCurriculumId: number, gradeId: number) {
    this.selectedGrade = undefined;

    if (this.selectedCurriculumId === selectedCurriculumId && this.selectedGradeId === gradeId) {
      this.selectedCurriculumId = undefined;
      this.selectedGradeId = undefined;

      return;
    }

    this.selectedCurriculumId = selectedCurriculumId;
    this.selectedGradeId = gradeId;
    this.loadingGrade = true;

    return this.studentInfoClassBookService
      .getGrade({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
        studentClassBookId: this.data.studentClassBookId,
        gradeId
      })
      .toPromise()
      .then((grade) => {
        this.selectedGrade = grade;
        this.selectedCurriculumId = selectedCurriculumId;
      })
      .catch((err) => {
        this.selectedCurriculumId = undefined;
        this.selectedGradeId = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingGrade = false;
      });
  }

  ngOnInit() {
    this.showFinalGrades = this.data.classBookInfo.basicClassId !== 1;
  }
}
