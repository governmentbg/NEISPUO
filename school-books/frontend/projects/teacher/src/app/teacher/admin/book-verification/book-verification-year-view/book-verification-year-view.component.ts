import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faTasksAlt as fadTasksAlt } from '@fortawesome/pro-duotone-svg-icons/faTasksAlt';
import { format } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  BookVerificationService,
  BookVerification_GetYearView
} from 'projects/sb-api-client/src/api/bookVerification.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { range } from 'projects/shared/utils/array';
import { roundUpToMultipleOf, throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookVerificationYearViewSkeletonComponent extends SkeletonComponentBase {
  constructor(bookVerificationService: BookVerificationService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.queryParamMap.get('c'));
    const teacherPersonId = tryParseInt(route.snapshot.queryParamMap.get('t'));

    this.resolve(BookVerificationYearViewComponent, {
      schoolYear,
      instId,
      yearView: bookVerificationService.getYearView({
        schoolYear,
        instId,
        classBookId,
        teacherPersonId
      })
    });
  }
}

@Component({
  selector: 'sb-book-verification-year-view',
  templateUrl: './book-verification-year-view.component.html'
})
export class BookVerificationYearViewComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    yearView: BookVerification_GetYearView;
  };

  fadTasksAlt = fadTasksAlt;

  templateData!: ReturnType<typeof getTemplateData>;

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    this.templateData = getTemplateData(this.data);
  }

  goToMonth(year: number, month: number) {
    this.router.navigate([year, month], { relativeTo: this.route, queryParamsHandling: 'preserve' });
  }
}

function getTemplateData(data: BookVerificationYearViewComponent['data']) {
  const repeatingMonths: boolean[] = [];

  const yearView = data.yearView.map((m) => {
    const result = {
      ...m,
      monthName: format(
        new Date(m.year, m.month - 1, 1),
        !repeatingMonths[m.month] ? 'MMMM' : 'MMMM (yyyy)', // September repeats, so add year
        { locale: bg }
      )
    };

    repeatingMonths[m.month] = true;

    return result;
  });

  return {
    yearView,
    emptyEndTiles: range(1, roundUpToMultipleOf(yearView.length, 3))
  };
}
