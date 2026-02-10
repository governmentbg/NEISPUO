import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarStar as fadCalendarStar } from '@fortawesome/pro-duotone-svg-icons/faCalendarStar';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  SchoolYearSettingsService,
  SchoolYearSettings_GetAll
} from 'projects/sb-api-client/src/api/schoolYearSettings.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  selector: 'sb-school-year-settings',
  templateUrl: './school-year-settings.component.html'
})
export class SchoolYearSettingsComponent {
  dataSource: TableDataSource<SchoolYearSettings_GetAll>;
  schoolYearName: string;
  fadCalendarStar = fadCalendarStar;
  fasPlus = fasPlus;
  fadChevronCircleRight = fadChevronCircleRight;
  canCreate = false;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    schoolYearSettingsService: SchoolYearSettingsService,
    route: ActivatedRoute
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      schoolYearSettingsService.getAll({ schoolYear, instId, offset, limit })
    );

    this.schoolYearName = `${schoolYear} - ${schoolYear + 1}`;

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate =
        institutionInfo.schoolYearAllowsModifications && institutionInfo.hasSchoolYearSettingsCreateAccess;
    });
  }
}
