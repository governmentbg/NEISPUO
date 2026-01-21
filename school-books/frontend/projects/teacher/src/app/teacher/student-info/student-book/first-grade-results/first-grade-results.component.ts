import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetFirstGradeResults
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import { GradeCategory } from 'projects/sb-api-client/src/model/gradeCategory';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class FirstGradeResultsSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');

    this.resolve(FirstGradeResultsComponent, {
      firstGradeResults: studentInfoClassBookService.getFirstGradeResults({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId
      })
    });
  }
}

@Component({
  selector: 'sb-first-grade-results',
  templateUrl: './first-grade-results.component.html',
  styleUrls: ['./first-grade-results.component.scss']
})
export class FirstGradeResultsComponent implements OnInit {
  @Input() data!: {
    firstGradeResults: StudentInfoClassBook_GetFirstGradeResults | null;
  };

  firstGradeResults!: ReturnType<typeof getFirstGradeResults>;

  ngOnInit() {
    this.firstGradeResults = getFirstGradeResults(this.data);
  }
}

function getFirstGradeResults(data: FirstGradeResultsComponent['data']) {
  if (!data.firstGradeResults) {
    return null;
  }

  return {
    grade: {
      category:
        data.firstGradeResults?.qualitativeGrade != null ? GradeCategory.Qualitative : GradeCategory.SpecialNeeds,
      qualitativeGrade: data.firstGradeResults.qualitativeGrade,
      specialGrade: data.firstGradeResults.specialGrade
    }
  };
}
