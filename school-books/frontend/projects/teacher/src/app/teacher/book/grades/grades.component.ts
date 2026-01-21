import { Component, Inject, InjectionToken, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { ClassBooksService, ClassBooks_GetCurriculums } from 'projects/sb-api-client/src/api/classBooks.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { VerticalTabItem } from 'projects/shared/components/vertical-tabs/vertical-tab-item';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Observable, Subject } from 'rxjs';
import { filter, map, startWith, takeUntil } from 'rxjs/operators';
import { CLASS_BOOK_INFO } from '../book/book.component';

export const CLASS_BOOK_GRADES_CURRICULUMS = new InjectionToken<Promise<ClassBooks_GetCurriculums>>(
  'Class book grades curriculums'
);
const classBookGradesCurriculumsProviderFactory = (classBooksService: ClassBooksService, route: ActivatedRoute) =>
  classBooksService
    .getCurriculums({
      schoolYear: tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear'),
      instId: tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId'),
      classBookId: tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId'),
      excludeGradeless: true,
      includeInvalidWithGrades: true,
      includeInvalidWithTopicPlans: false
    })
    .toPromise();

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE,
  providers: [
    {
      provide: CLASS_BOOK_GRADES_CURRICULUMS,
      deps: [ClassBooksService, ActivatedRoute],
      useFactory: classBookGradesCurriculumsProviderFactory
    }
  ]
})
export class GradesSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    @Inject(CLASS_BOOK_GRADES_CURRICULUMS) curriculums: Promise<ClassBooks_GetCurriculums>
  ) {
    super();

    this.resolve(GradesComponent, {
      classBookInfo: from(classBookInfo),
      curriculums: from(curriculums)
    });
  }
}

type CurriculumVO = ArrayElementType<ClassBooks_GetCurriculums>;

@Component({
  selector: 'sb-grades',
  templateUrl: './grades.component.html'
})
export class GradesComponent implements OnInit, OnDestroy {
  @Input() data!: {
    classBookInfo: ClassBookInfoType;
    curriculums: ClassBooks_GetCurriculums;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fasPlus = fasPlus;

  tabs!: VerticalTabItem[];
  newGradeDisabled$?: Observable<boolean>;

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

    const curriculums = new Map(this.data.curriculums.map((c) => [c.curriculumId, c]));

    this.newGradeDisabled$ = this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),

      // when ngOnInit fires the navigation has already ended,
      // so we need this for the initial curriculum (the one that created this route)
      startWith(null),

      map(() => tryParseInt(this.route.snapshot.firstChild?.paramMap.get('curriculumId'))),
      map(
        (curriculumId) =>
          !curriculumId ||
          !this.data.classBookInfo.bookAllowsModifications ||
          !curriculums.has(curriculumId) ||
          !curriculums.get(curriculumId)?.hasCreateGradeAccess ||
          curriculums.get(curriculumId)?.withoutGrade ||
          !curriculums.get(curriculumId)?.isValid
      ),

      takeUntil(this.destroyed$)
    );
  }
}
