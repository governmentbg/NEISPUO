import { Component, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentClassBookService,
  StudentClassBook_GetTopics
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class TopicsViewSkeletonComponent extends SkeletonComponentBase {
  constructor(studentClassBookService: StudentClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const curriculumId = tryParseInt(route.snapshot.paramMap.get('curriculumId')) ?? throwParamError('curriculumId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.resolve(TopicsViewComponent, {
      schoolYear,
      classBookId,
      curriculumId,
      personId,
      topics: studentClassBookService.getTopics({ schoolYear, classBookId, personId, curriculumId })
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
    classBookId: number;
    curriculumId: number;
    personId: number;
    topics: StudentClassBook_GetTopics;
  };

  readonly fasPlus = fasPlus;
}
