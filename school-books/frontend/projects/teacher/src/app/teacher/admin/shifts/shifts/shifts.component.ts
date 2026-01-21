import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faBellSchool as fadBellSchool } from '@fortawesome/pro-duotone-svg-icons/faBellSchool';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { ShiftsService, Shifts_GetAll } from 'projects/sb-api-client/src/api/shifts.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  selector: 'sb-shifts',
  templateUrl: './shifts.component.html'
})
export class ShiftsComponent {
  dataSource: TableDataSource<Shifts_GetAll>;

  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;
  fadBellSchool = fadBellSchool;
  canCreate = false;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    shiftsService: ShiftsService,
    route: ActivatedRoute
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      shiftsService.getAll({ schoolYear, instId, offset, limit })
    );

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate = institutionInfo.schoolYearAllowsModifications && institutionInfo.hasShiftCreateAccess;
    });
  }
}
