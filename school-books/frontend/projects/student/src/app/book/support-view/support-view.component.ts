import { Component, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  StudentClassBookService,
  StudentClassBook_GetSupport,
  StudentClassBook_GetSupportActivities
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class SupportViewSkeletonComponent extends SkeletonComponentBase {
  constructor(studentClassBookService: StudentClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const supportId = tryParseInt(route.snapshot.paramMap.get('supportId')) ?? throwParamError('personId');

    this.resolve(SupportViewComponent, {
      schoolYear,
      classBookId,
      personId,
      support: studentClassBookService.getSupport({
        schoolYear,
        classBookId,
        personId,
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
    classBookId: number;
    personId: number;
    support: StudentClassBook_GetSupport;
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

  dataSource!: TableDataSource<StudentClassBook_GetSupportActivities>;

  constructor(private fb: UntypedFormBuilder, private studentClassBookService: StudentClassBookService) {
    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      this.studentClassBookService.getSupportActivities({
        schoolYear: this.data.schoolYear,
        classBookId: this.data.classBookId,
        personId: this.data.personId,
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
