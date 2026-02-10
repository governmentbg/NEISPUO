import { Component, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faPencilAlt as fasPencilAlt } from '@fortawesome/pro-solid-svg-icons/faPencilAlt';
import { faPlusSquare as fasPlusSquare } from '@fortawesome/pro-solid-svg-icons/faPlusSquare';
import { isBefore } from 'date-fns';
import {
  StudentClassBookService,
  StudentClassBook_GetClassBookInfo,
  StudentClassBook_GetGrade,
  StudentClassBook_GetGrades
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import { GradeCategory } from 'projects/sb-api-client/src/model/gradeCategory';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { AuthService, SysRole } from 'projects/shared/services/auth.service';
import { BreakpointService } from 'projects/shared/services/breakpoint.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class GradesSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<StudentClassBook_GetClassBookInfo>,
    route: ActivatedRoute,
    studentClassBookService: StudentClassBookService
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.resolve(GradesComponent, {
      schoolYear,
      classBookId,
      personId,
      grades: studentClassBookService.getGrades({
        schoolYear,
        classBookId,
        personId
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
    classBookId: number;
    personId: number;
    grades: StudentClassBook_GetGrades;
    classBookInfo: StudentClassBook_GetClassBookInfo;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly fasPencilAlt = fasPencilAlt;
  readonly fasPlusSquare = fasPlusSquare;
  readonly GRADE_CATEGORY_DECIMAL = GradeCategory.Decimal;
  readonly GRADE_CATEGORY_QUALITATIVE = GradeCategory.Qualitative;
  readonly GRADE_CATEGORY_SPECIAL_NEEDS = GradeCategory.SpecialNeeds;

  firstTermExpanded!: boolean;
  secondTermExpanded!: boolean;

  selectedCurriculumId?: number;
  selectedGradeId?: number;
  selectedGrade?: StudentClassBook_GetGrade;
  loadingGrade = false;
  showFinalGrades!: boolean;

  constructor(
    public breakpointService: BreakpointService,
    private studentClassBookService: StudentClassBookService,
    private authService: AuthService,
    private actionService: ActionService
  ) {}

  expandFirstTerm() {
    this.firstTermExpanded = true;
    this.secondTermExpanded = false;
    this.selectedCurriculumId = undefined;
    this.selectedGradeId = undefined;
    this.selectedGrade = undefined;
  }

  expandSecondTerm() {
    this.firstTermExpanded = false;
    this.secondTermExpanded = true;
    this.selectedCurriculumId = undefined;
    this.selectedGradeId = undefined;
    this.selectedGrade = undefined;
  }

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

    return this.studentClassBookService
      .getGrade({
        schoolYear: this.data.schoolYear,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
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

    const now = new Date();
    const isFirstTerm = isBefore(now, this.data.classBookInfo.secondTermStartDate);

    this.firstTermExpanded = isFirstTerm;
    this.secondTermExpanded = !isFirstTerm;

    if (this.authService.tokenPayload.selected_role.SysRoleID === SysRole.Parent) {
      const gradeIds = this.data.grades
        .flatMap((g) => [
          ...g.firstTermRegularGrades,
          ...g.firstTermTermGrades,
          ...g.secondTermRegularGrades,
          ...g.secondTermTermGrades,
          ...g.finalGrades
        ])
        .filter((grade) => !grade.isReadFromParent)
        .map((grade) => grade.gradeId);

      const schoolYear = this.data.schoolYear;
      const classBookId = this.data.classBookId;
      const personId = this.data.personId;

      if (gradeIds) {
        setTimeout(() => {
          this.studentClassBookService.setGradesAsRead({ schoolYear, classBookId, personId, gradeIds }).toPromise();
        }, 2000);
      }
    }
  }
}
