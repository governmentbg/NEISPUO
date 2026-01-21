import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarWeek as fadCalendarWeek } from '@fortawesome/pro-duotone-svg-icons/faCalendarWeek';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  MissingTopicsReportsService,
  MissingTopicsReports_GetAll
} from 'projects/sb-api-client/src/api/missingTopicsReports.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  selector: 'sb-missing-topics-reports',
  templateUrl: './missing-topics-reports.component.html'
})
export class MissingTopicsReportsComponent {
  dataSource: TableDataSource<MissingTopicsReports_GetAll>;

  fadCalendarWeek = fadCalendarWeek;
  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;

  constructor(missingTopicsReportsService: MissingTopicsReportsService, route: ActivatedRoute) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      missingTopicsReportsService.getAll({ schoolYear, instId, offset, limit })
    );
  }
}
