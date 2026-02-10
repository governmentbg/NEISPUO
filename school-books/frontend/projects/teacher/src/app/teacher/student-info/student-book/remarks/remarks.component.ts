import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetRemarks,
  StudentInfoClassBook_GetRemarksForCurriculumAndType
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import { RemarkType } from 'projects/sb-api-client/src/model/remarkType';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class RemarksSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');

    this.resolve(RemarksComponent, {
      schoolYear,
      instId,
      classBookId,
      personId,
      studentClassBookId,
      remarks: studentInfoClassBookService.getRemarks({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId
      })
    });
  }
}

type Remark = ArrayElementType<StudentInfoClassBook_GetRemarksForCurriculumAndType>;

@Component({
  selector: 'sb-remarks',
  templateUrl: './remarks.component.html'
})
export class RemarksComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    personId: number;
    studentClassBookId: number;
    remarks: StudentInfoClassBook_GetRemarks;
  };

  readonly fasBookOpen = fasBookOpen;
  readonly fasCalendarAlt = fasCalendarAlt;
  readonly farCalendarTimes = farCalendarTimes;
  readonly fasPencil = fasPencil;

  readonly REMARK_TYPE_BAD = RemarkType.Bad;
  readonly REMARK_TYPE_GOOD = RemarkType.Good;

  studentRemarks!: ReturnType<typeof getStudentRemarks>;

  selectedCurriculumId?: number;
  selectedType?: RemarkType;
  selectedRemarks?: Remark[];
  loadingRemarks = false;

  constructor(private studentInfoClassBookService: StudentInfoClassBookService) {}

  ngOnInit() {
    this.studentRemarks = getStudentRemarks(this.data);
  }

  viewRemarks(curriculumId: number, type: RemarkType) {
    this.selectedRemarks = undefined;

    if (this.selectedCurriculumId === curriculumId && this.selectedType === type) {
      this.selectedCurriculumId = undefined;
      this.selectedType = undefined;

      return;
    }

    this.selectedCurriculumId = curriculumId;
    this.selectedType = type;
    this.loadingRemarks = true;

    return this.studentInfoClassBookService
      .getRemarksForCurriculumAndType({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
        studentClassBookId: this.data.studentClassBookId,
        curriculumId: this.selectedCurriculumId,
        type: this.selectedType
      })
      .toPromise()
      .then((remarks) => {
        this.selectedRemarks = remarks;
      })
      .catch((err) => {
        this.selectedCurriculumId = undefined;
        this.selectedType = undefined;

        GlobalErrorHandler.instance.handleError(err);
      })
      .finally(() => {
        this.loadingRemarks = false;
      });
  }
}

function getStudentRemarks(data: RemarksComponent['data']) {
  const totals = data.remarks.reduce(
    (totals, s) => {
      totals.badRemarksCountTotal += s.badRemarksCount;
      totals.goodRemarksCountTotal += s.goodRemarksCount;

      return totals;
    },
    {
      badRemarksCountTotal: 0,
      goodRemarksCountTotal: 0
    }
  );

  return {
    remarks: data.remarks,
    totals
  };
}
