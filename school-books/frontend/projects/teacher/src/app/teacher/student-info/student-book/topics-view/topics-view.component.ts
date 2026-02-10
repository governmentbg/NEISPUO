import { Component, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetTopics
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class TopicsViewSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const curriculumId = tryParseInt(route.snapshot.paramMap.get('curriculumId')) ?? throwParamError('curriculumId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');

    this.resolve(TopicsViewComponent, {
      schoolYear,
      instId,
      classBookId,
      curriculumId,
      personId,
      topics: studentInfoClassBookService.getTopics({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId,
        curriculumId
      })
    });
  }
}

@Component({
  selector: 'sb-topics-view',
  templateUrl: './topics-view.component.html'
})
export class TopicsViewComponent {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    curriculumId: number;
    personId: number;
    topics: StudentInfoClassBook_GetTopics;
  };

  readonly fasPlus = fasPlus;
}
