import { Component, Inject, InjectionToken, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { ClassBooksService, ClassBooks_GetCurriculums } from 'projects/sb-api-client/src/api/classBooks.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { VerticalTabItem } from 'projects/shared/components/vertical-tabs/vertical-tab-item';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';

export const CLASS_BOOK_TOPIC_PLANS_CURRICULUMS = new InjectionToken<Promise<ClassBooks_GetCurriculums>>(
  'Class book topic plan curriculums'
);
const classBookTopicPlansCurriculumsProviderFactory = (classBooksService: ClassBooksService, route: ActivatedRoute) =>
  classBooksService
    .getCurriculums({
      schoolYear: tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear'),
      instId: tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId'),
      classBookId: tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId'),
      excludeGradeless: false,
      includeInvalidWithGrades: false,
      includeInvalidWithTopicPlans: true
    })
    .toPromise();

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE,
  providers: [
    {
      provide: CLASS_BOOK_TOPIC_PLANS_CURRICULUMS,
      deps: [ClassBooksService, ActivatedRoute],
      useFactory: classBookTopicPlansCurriculumsProviderFactory
    }
  ]
})
export class TopicPlansSkeletonComponent extends SkeletonComponentBase {
  constructor(@Inject(CLASS_BOOK_TOPIC_PLANS_CURRICULUMS) curriculums: Promise<ClassBooks_GetCurriculums>) {
    super();

    this.resolve(TopicPlansComponent, {
      curriculums: from(curriculums)
    });
  }
}

type CurriculumVO = ArrayElementType<ClassBooks_GetCurriculums>;

@Component({
  selector: 'sb-topic-plans',
  templateUrl: './topic-plans.component.html'
})
export class TopicPlansComponent implements OnInit, OnDestroy {
  @Input() data!: {
    curriculums: ClassBooks_GetCurriculums;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fasPlus = fasPlus;

  tabs!: VerticalTabItem[];

  constructor(private router: Router, public route: ActivatedRoute) {}

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit() {
    this.tabs = [];

    const mapTab = (c: CurriculumVO): VerticalTabItem => ({
      text:
        (!c.curriculumGroupName
          ? `${c.subjectName} / ${c.subjectTypeName}`
          : `${c.subjectName} / ${c.subjectTypeName} - ${c.curriculumGroupName}`) +
        `${c.isIndividualCurriculum ? ' (ИУП)' : ''}` +
        `${c.isIndividualLesson ? ' (ИЧ)' : ''}`,
      badge: !c.isValid ? 'АРХИВИРАН' : c.withoutGrade ? 'БЕЗ ОЦЕНКА' : null,
      routeCommands: ['./', c.curriculumId],
      routeExtras: { relativeTo: this.route }
    });

    const parents = this.data.curriculums.filter((c) => c.parentCurriculumId == null);

    for (const parent of parents) {
      const children = this.data.curriculums.filter((cg) => cg.parentCurriculumId === parent.curriculumId);

      if (!parent.isValid && parent.subjectTypeIsProfilingSubject && children.length === 0) {
        continue;
      }

      this.tabs.push({
        ...mapTab(parent),
        tabItems: children.map(mapTab)
      });
    }
  }
}
