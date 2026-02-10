import { EntityManager } from 'typeorm';
import { DataRefreshSyncCronJob } from '../../shared/classes/data-refresh-sync-cron-job';
import { Injectable } from '@nestjs/common';

@Injectable()
export class UpdateRInstitutionsService extends DataRefreshSyncCronJob {
  constructor(protected entityManager: EntityManager) {
    super(
      'inst_basic.R_Institutions',
      'reporting.R_Institutions_Table',
      `InstitutionID,
      InstitutionName,
      ShortInstitutionName,
      Bulstat,
      CountryName,
      RegionName,
      MunicipalityName,
      TownName,
      LocalAreaName,
      TownID,
      MunicipalityID,
      RegionID,
      LocalAreaID,
      BudgetingSchoolTypeID,
      PostCode,
      BaseSchoolTypeName,
      DetailedSchoolTypeName,
      FinancialSchoolTypeName,
      BudgetingInstitutionName,
      IsDelegateBudget,
      YearlyBudget,
      Address,
      PhoneNumber,
      Email,
      Website,
      EstablishedYear,
      ConstitActFirst,
      ConstitActLast,
      SchoolShiftType,
      IsCentral,
      IsProtected,
      IsInnovative,
      IsNational,
      IsProfSchool,
      IsNonIndDormitory,
      HasMunDecisionFor4,
      IsAppInnovSystem,
      IsODZ,
      IsProvideEduServ,
      Director,
      InstitutionPublicCouncil,
      StaffCountAll,
      PedagogStaffCount,
      NonpedagogStaffCount`,
      entityManager,
    );
  }
}
