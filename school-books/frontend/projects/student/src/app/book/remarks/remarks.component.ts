import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarTimes as farCalendarTimes } from '@fortawesome/pro-regular-svg-icons/faCalendarTimes';
import { faBookOpen as fasBookOpen } from '@fortawesome/pro-solid-svg-icons/faBookOpen';
import { faCalendarAlt as fasCalendarAlt } from '@fortawesome/pro-solid-svg-icons/faCalendarAlt';
import { faPencil as fasPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import {
  StudentClassBookService,
  StudentClassBook_GetRemarks,
  StudentClassBook_GetRemarksForCurriculumAndType
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import { RemarkType } from 'projects/sb-api-client/src/model/remarkType';
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
export class RemarksSkeletonComponent extends SkeletonComponentBase {
  constructor(studentClassBookService: StudentClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.resolve(RemarksComponent, {
      schoolYear,
      classBookId,
      personId,
      remarks: studentClassBookService.getRemarks({
        schoolYear,
        classBookId,
        personId
      })
    });
  }
}

type Remark = ArrayElementType<StudentClassBook_GetRemarksForCurriculumAndType>;

@Component({
  selector: 'sb-remarks',
  templateUrl: './remarks.component.html'
})
export class RemarksComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    classBookId: number;
    personId: number;
    remarks: StudentClassBook_GetRemarks;
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

  constructor(private studentClassBookService: StudentClassBookService) {}

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

    return this.studentClassBookService
      .getRemarksForCurriculumAndType({
        schoolYear: this.data.schoolYear,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
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
