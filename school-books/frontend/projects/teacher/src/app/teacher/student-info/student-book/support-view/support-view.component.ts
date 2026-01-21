import { Component, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetSupport,
  StudentInfoClassBook_GetSupportActivities
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class SupportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');
    const supportId = tryParseInt(route.snapshot.paramMap.get('supportId')) ?? throwParamError('personId');

    this.resolve(SupportViewComponent, {
      schoolYear,
      instId,
      classBookId,
      personId,
      studentClassBookId,
      support: studentInfoClassBookService.getSupport({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId,
        supportId: supportId
      })
    });
  }
}

@Component({
  selector: 'sb-support-view',
  templateUrl: './support-view.component.html'
})
export class SupportViewComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    personId: number;
    studentClassBookId: number;
    support: StudentInfoClassBook_GetSupport;
  };

  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasArrowLeft = fasArrowLeft;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    difficultyTypes: [null],
    description: [null],
    expectedResult: [null],
    endDate: [null],
    teacherNames: [null]
  });

  dataSource!: TableDataSource<StudentInfoClassBook_GetSupportActivities>;

  constructor(private fb: UntypedFormBuilder, private studentInfoClassBookService: StudentInfoClassBookService) {
    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.studentInfoClassBookService.getSupportActivities({
        schoolYear: this.data.schoolYear,
        instId: this.data.instId,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
        studentClassBookId: this.data.studentClassBookId,
        supportId: this.data.support.supportId,
        offset,
        limit
      })
    );
  }

  ngOnInit() {
    this.form.setValue({
      difficultyTypes: this.data.support.difficulties,
      description: this.data.support.description,
      expectedResult: this.data.support.expectedResult,
      endDate: this.data.support.endDate,
      teacherNames: this.data.support.teacherNames
    });
  }
}
