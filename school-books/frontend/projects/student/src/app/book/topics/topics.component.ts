import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  StudentClassBookService,
  StudentClassBook_GetCurriculumForTopics
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TabItem } from 'projects/shared/components/tabs/tab-item';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class TopicsSkeletonComponent extends SkeletonComponentBase {
  constructor(studentClassBookService: StudentClassBookService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.resolve(TopicsComponent, {
      curriculum: studentClassBookService.getCurriculumForTopics({
        schoolYear,
        classBookId,
        personId
      })
    });
  }
}

@Component({
  selector: 'sb-topics',
  templateUrl: './topics.component.html'
})
export class TopicsComponent implements OnInit {
  @Input() data!: {
    curriculum: StudentClassBook_GetCurriculumForTopics;
  };

  readonly fasPlus = fasPlus;

  tabs!: TabItem[];

  constructor(private router: Router, public route: ActivatedRoute) {}

  ngOnInit() {
    this.tabs = this.data.curriculum.map((c) => ({
      text: !c.curriculumGroupName
        ? `${c.subjectName} / ${c.subjectTypeName}`
        : `${c.subjectName} / ${c.subjectTypeName} - ${c.curriculumGroupName}`,
      routeCommands: ['./', c.curriculumId],
      routeExtras: { relativeTo: this.route }
    }));

    // navigate to the first curriculum item if none is selected
    if (this.data.curriculum.length && !this.route.firstChild) {
      this.router.navigate(['./', this.data.curriculum[0].curriculumId], { relativeTo: this.route });
    }
  }
}
