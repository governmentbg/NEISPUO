import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  ScheduleAndAbsencesByTermAllClassesReportsService,
  ScheduleAndAbsencesByTermAllClassesReports_GetAll
} from 'projects/sb-api-client/src/api/scheduleAndAbsencesByTermAllClassesReports.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  selector: 'sb-schedule-and-absences-by-term-all-classes-reports',
  templateUrl: './schedule-and-absences-by-term-all-classes-reports.component.html'
})
export class ScheduleAndAbsencesByTermAllClassesReportsComponent {
  dataSource: TableDataSource<ScheduleAndAbsencesByTermAllClassesReports_GetAll>;

  fadCalendarMinus = fadCalendarMinus;
  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;

  constructor(
    scheduleAndAbsencesByTermAllClassesReportsService: ScheduleAndAbsencesByTermAllClassesReportsService,
    route: ActivatedRoute
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      scheduleAndAbsencesByTermAllClassesReportsService.getAll({ schoolYear, instId, offset, limit })
    );
  }
}
