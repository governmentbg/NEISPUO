import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarDay as fadCalendarDay } from '@fortawesome/pro-duotone-svg-icons/faCalendarDay';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { OffDaysService, OffDays_GetAll } from 'projects/sb-api-client/src/api/offDays.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  selector: 'sb-off-days',
  templateUrl: './off-days.component.html'
})
export class OffDaysComponent {
  dataSource: TableDataSource<OffDays_GetAll>;

  fadCalendarDay = fadCalendarDay;
  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;
  canCreate = false;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    offDaysService: OffDaysService,
    route: ActivatedRoute
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      offDaysService.getAll({ schoolYear, instId, offset, limit })
    );

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate = institutionInfo.schoolYearAllowsModifications && institutionInfo.hasOffDayCreateAccess;
    });
  }
}
