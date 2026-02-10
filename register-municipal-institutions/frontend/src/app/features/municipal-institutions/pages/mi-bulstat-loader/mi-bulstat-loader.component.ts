import { Component, OnInit } from '@angular/core';
import { BaseSchoolType } from '@municipal-institutions/models/base-school-type';
import { BulstatService } from '@municipal-institutions/services/bulstat.service';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { filter, map } from 'rxjs/operators';
import { GuideName } from '@shared/modules/user-tour-guide/constants/user-tour-guide.constants';
import { UserTourGuideService } from '@shared/modules/user-tour-guide/user-tour-guide.service';

@Component({
  selector: 'app-mi-bulstat-loader',
  templateUrl: './mi-bulstat-loader.component.html',
  styleUrls: ['./mi-bulstat-loader.component.scss'],
})
export class MiBulstatLoaderComponent implements OnInit {
  selectedBaseSchoolType: BaseSchoolType;

  bulstat: string = '';

  populatedMI: MunicipalInstitution;

  displayWarnMessage = false;

  displaySuccessMessage = false;

  warnMessage = "Институция с въведения ЕИК не беше намерена. Натиснете бутон 'Напред', за да въведете ръчно информацията.";

  successMessage = "Институция с въведения ЕИК беше намерена. Натиснете бутон 'Напред', за да заредите получените данни.";

  guideName = GuideName.BULSTAT_CHECK;

  BaseSchoolTypes$ = this.nmQuery.BaseSchoolTypes$
    .pipe(
      filter((baseSchoolTypes) => !!baseSchoolTypes),
      map(
        (baseSchoolTypes) => baseSchoolTypes
          .filter((bst) => bst.BaseSchoolTypeID === EnumBaseSchoolType.KINDERGARTEN || bst.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE),

      ),

    );

  constructor(
    private bulstatService: BulstatService,
    private nmQuery: NomenclatureQuery,
    private userTourService: UserTourGuideService,
  ) { }

  ngOnInit(): void {
  }

  async verifyEIK() {
    this.displaySuccessMessage = false;
    this.displayWarnMessage = false;
    this.bulstatService.verify(this.bulstat)
      .subscribe(
        (institutionData: MunicipalInstitution) => {
          this.displaySuccessMessage = true;
          this.bulstatService.municipalInstitution = institutionData;
        },
        (error) => {
          this.bulstatService.municipalInstitution = null;
          this.displayWarnMessage = true;
        },
      );
  }

  ngOnDestroy(): void {
    this.userTourService.stopGuide();
  }
}
