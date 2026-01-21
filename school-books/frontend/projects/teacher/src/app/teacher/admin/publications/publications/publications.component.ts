import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faInfoSquare as fadInfoSquare } from '@fortawesome/pro-solid-svg-icons/faInfoSquare';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { PublicationsService, Publications_GetAll } from 'projects/sb-api-client/src/api/publications.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  selector: 'sb-publications',
  templateUrl: './publications.component.html'
})
export class PublicationsComponent {
  readonly fadInfoSquare = fadInfoSquare;
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  canCreate = false;

  dataSource: TableDataSource<Publications_GetAll>;

  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    publicationsService: PublicationsService,
    route: ActivatedRoute
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      publicationsService.getAll({ schoolYear, instId, offset, limit })
    );

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    institutionInfo.then((institutionInfo) => {
      this.canCreate = institutionInfo.schoolYearAllowsModifications && institutionInfo.hasPublicationCreateAccess;
    });
  }
}
