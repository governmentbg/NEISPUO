import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarStar as fadCalendarStar } from '@fortawesome/pro-duotone-svg-icons/faCalendarStar';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  LectureSchedulesReportsService,
  LectureSchedulesReports_GetAll
} from 'projects/sb-api-client/src/api/lectureSchedulesReports.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  selector: 'sb-lecture-schedules-reports',
  templateUrl: './lecture-schedules-reports.component.html'
})
export class LectureSchedulesReportsComponent {
  dataSource: TableDataSource<LectureSchedulesReports_GetAll>;

  fadCalendarStar = fadCalendarStar;
  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;

  constructor(lectureSchedulesReportsService: LectureSchedulesReportsService, route: ActivatedRoute) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      lectureSchedulesReportsService.getAll({ schoolYear, instId, offset, limit })
    );
  }
}
