import { Injectable } from '@nestjs/common';
import { EntityManager } from 'typeorm';
import { DataRefreshSyncCronJob } from '../../shared/classes/data-refresh-sync-cron-job';

@Injectable()
export class UpdateRStudentsDetailsService extends DataRefreshSyncCronJob {
  constructor(protected entityManager: EntityManager) {
    super(
      'reporting.R_Students_Details',
      'reporting.R_Students_Details_Table',
      `	RegionID,
    RegionName,
    MunicipalityID,
    MunicipalityName,
    TownID,
    TownName,
	  LocalAreaID,
    LAreaName,
    InstitutionID,
    BudgetingSchoolTypeID,
    InstitutionName,
    InstitutionKind,
    FirstName,
    MiddleName,
    LastName,
    PublicEduNumber,
    IdType,
    PersonalID,
    BirthDate,
    PersonTown, 
    PersonMunicipality,
    PersonRegion,
	  Nationality,
    RomeName,
    ClassName,
    EduFormName,
    ClassType,
    ProfName,
    SpecName,
    IsIndividualCurriculum,
    IsTravel,
    IsRepeatClass,
    IsSOP,
    IsRP,
    Gender`,
      entityManager,
    );
  }
}
