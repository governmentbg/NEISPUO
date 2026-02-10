import { Component, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { GradeResultsService, GradeResults_GetSessionAll } from 'projects/sb-api-client/src/api/gradeResults.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class GradeResultSessionsSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    gradeResultsService: GradeResultsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.resolve(GradeResultSessionsComponent, {
      classBookId,
      classBookInfo: from(classBookInfo),
      students: gradeResultsService.getSessionAll({
        schoolYear,
        instId,
        classBookId
      })
    });
  }
}

@Component({
  selector: 'sb-grade-result-sessions',
  templateUrl: './grade-result-sessions.component.html'
})
export class GradeResultSessionsComponent implements OnInit {
  @Input() data!: {
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    students: GradeResults_GetSessionAll;
  };

  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly noShowText = 'Неявил се';

  students!: ReturnType<typeof mapStudents>;

  canEdit = false;

  ngOnInit() {
    this.students = mapStudents(this.data);

    this.canEdit =
      this.data.classBookInfo.bookAllowsModifications && this.data.classBookInfo.hasEditGradeResultSessionsAccess;
  }
}

function mapStudents(data: GradeResultSessionsComponent['data']) {
  return data.students.map((s) => ({
    ...s,
    showInfoLink: !s.isTransferred && !s.isRemoved,
    abnormalStatus: s.isRemoved ? 'ИЗТРИТ' : s.isTransferred ? 'ОТПИСАН' : ''
  }));
}
