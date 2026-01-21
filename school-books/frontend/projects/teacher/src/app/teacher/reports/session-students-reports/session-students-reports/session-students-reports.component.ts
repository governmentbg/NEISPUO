import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  SessionStudentsReportsService,
  SessionStudentsReports_GetAll
} from 'projects/sb-api-client/src/api/sessionStudentsReports.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  selector: 'sb-session-students-reports',
  templateUrl: './session-students-reports.component.html'
})
export class SessionStudentsReportsComponent {
  dataSource: TableDataSource<SessionStudentsReports_GetAll>;

  fadCalendarMinus = fadCalendarMinus;
  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;
  fasSpinnerThird = fasSpinnerThird;
  creating = false;
  schoolYear: number;
  instId: number;

  constructor(
    private sessionStudentsReportsService: SessionStudentsReportsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {
    this.schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    this.instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      sessionStudentsReportsService.getAll({ schoolYear: this.schoolYear, instId: this.instId, offset, limit })
    );
  }

  create() {
    this.creating = true;
    this.actionService
      .execute({
        httpAction: () => {
          return this.sessionStudentsReportsService
            .create({
              schoolYear: this.schoolYear,
              instId: this.instId
            })
            .toPromise()
            .then((newSessionStudentsReportId) => {
              this.router.navigate(['./', newSessionStudentsReportId], { relativeTo: this.route });
            });
        }
      })
      .finally(() => (this.creating = false));
  }
}
