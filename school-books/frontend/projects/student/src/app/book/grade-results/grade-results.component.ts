import { Component, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentClassBookService,
  StudentClassBook_GetGradeResults
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class GradeResultsSkeletonComponent extends SkeletonComponentBase {
  constructor(studentClassBookService: StudentClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.resolve(GradeResultsComponent, {
      gradeResults: studentClassBookService.getGradeResults({
        schoolYear,
        classBookId,
        personId
      })
    });
  }
}

@Component({
  selector: 'sb-grade-results',
  templateUrl: './grade-results.component.html'
})
export class GradeResultsComponent {
  @Input() data!: {
    gradeResults: StudentClassBook_GetGradeResults;
  };

  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  noShowText = 'Неявил се';
}
