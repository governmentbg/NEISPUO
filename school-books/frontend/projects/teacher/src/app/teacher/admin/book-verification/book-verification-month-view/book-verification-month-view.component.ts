import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faTasksAlt as fadTasksAlt } from '@fortawesome/pro-duotone-svg-icons/faTasksAlt';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { format, getDay, getDaysInMonth } from 'date-fns';
import { bg } from 'date-fns/locale';
import {
  BookVerificationService,
  BookVerification_GetMonthView
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
export class BookVerificationMonthViewSkeletonComponent extends SkeletonComponentBase {
  constructor(bookVerificationService: BookVerificationService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const year = tryParseInt(route.snapshot.paramMap.get('year')) ?? throwParamError('year');
    const month = tryParseInt(route.snapshot.paramMap.get('month')) ?? throwParamError('month');
    const classBookId = tryParseInt(route.snapshot.queryParamMap.get('c'));
    const teacherPersonId = tryParseInt(route.snapshot.queryParamMap.get('t'));

    this.resolve(BookVerificationMonthViewComponent, {
      schoolYear,
      instId,
      year,
      month,
      monthView: bookVerificationService.getMonthView({
        schoolYear,
        instId,
        year,
        month,
        classBookId,
        teacherPersonId
      })
    });
  }
}

const DAYS_PER_WEEK = 7;

@Component({
  selector: 'sb-book-verification-month-view',
  templateUrl: './book-verification-month-view.component.html'
})
export class BookVerificationMonthViewComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    year: number;
    month: number;
    monthView: BookVerification_GetMonthView;
  };

  fadTasksAlt = fadTasksAlt;
  fasArrowLeft = fasArrowLeft;

  templateData!: ReturnType<typeof getTemplateData>;

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    this.templateData = getTemplateData(this.data);
  }

  goToDay(day: number) {
    this.router.navigate([day], { relativeTo: this.route, queryParamsHandling: 'preserve' });
  }
}

function getTemplateData(data: BookVerificationMonthViewComponent['data']) {
  const monthStart = new Date(data.year, data.month - 1, 1);
  const daysInMonth = getDaysInMonth(monthStart);
  const firstWeekOffset = (DAYS_PER_WEEK + getDay(monthStart) - 1) % DAYS_PER_WEEK;

  return {
    monthName: format(monthStart, 'MMMM', { locale: bg }),
    firstWeekOffsetTiles: range(1, firstWeekOffset),
    monthView: data.monthView.map((d) => ({
      ...d,
      dayName: format(new Date(data.year, data.month - 1, d.day), 'do MMMM', { locale: bg })
    })),
    emptyEndTiles: range(1, roundUpToMultipleOf(firstWeekOffset + daysInMonth, 7))
  };
}
