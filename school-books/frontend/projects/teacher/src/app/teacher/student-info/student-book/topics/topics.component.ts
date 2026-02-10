import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentInfoClassBookService,
  StudentInfoClassBook_GetCurriculumForTopics
} from 'projects/sb-api-client/src/api/studentInfoClassBook.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { VerticalTabItem } from 'projects/shared/components/vertical-tabs/vertical-tab-item';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class TopicsSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoClassBookService: StudentInfoClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');
    const studentClassBookId =
      tryParseInt(route.snapshot.paramMap.get('studentClassBookId')) ?? throwParamError('studentClassBookId');

    this.resolve(TopicsComponent, {
      curriculum: studentInfoClassBookService.getCurriculumForTopics({
        schoolYear,
        instId,
        classBookId,
        personId,
        studentClassBookId
      })
    });
  }
}

type CurriculumVO = ArrayElementType<StudentInfoClassBook_GetCurriculumForTopics>;

@Component({
  selector: 'sb-topics',
  templateUrl: './topics.component.html'
})
export class TopicsComponent implements OnInit {
  @Input() data!: {
    curriculum: StudentInfoClassBook_GetCurriculumForTopics;
  };

  readonly fasPlus = fasPlus;

  tabs!: VerticalTabItem[];

  constructor(private router: Router, public route: ActivatedRoute) {}

  ngOnInit() {
    this.tabs = [];

    const mapTab = (c: CurriculumVO): VerticalTabItem => ({
      text: !c.curriculumGroupName
        ? `${c.subjectName} / ${c.subjectTypeName}`
        : `${c.subjectName} / ${c.subjectTypeName} - ${c.curriculumGroupName}`,
      routeCommands: ['./', c.curriculumId],
      routeExtras: { relativeTo: this.route }
    });

    const parents = this.data.curriculum.filter((c) => c.parentCurriculumId == null);

    for (const parent of parents) {
      const children = this.data.curriculum.filter((cg) => cg.parentCurriculumId === parent.curriculumId);

      this.tabs.push({
        ...mapTab(parent),
        tabItems: children.map(mapTab)
      });
    }

    // navigate to the first curriculum item if none is selected
    if (this.data.curriculum.length && !this.route.firstChild) {
      this.router.navigate(['./', this.data.curriculum[0].curriculumId], { relativeTo: this.route });
    }
  }
}
