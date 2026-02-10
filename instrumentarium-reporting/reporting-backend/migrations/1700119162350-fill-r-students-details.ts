import { MigrationInterface, QueryRunner } from 'typeorm';

export class fillRStudentsDetails1700119162350 implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `TRUNCATE TABLE [reporting].R_Students_Details_Table`,
    );
    await queryRunner.query(
      `INSERT INTO [reporting].R_Students_Details_Table WITH (TABLOCK)
    SELECT RegionID,
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
    IsRP,Gender from [reporting].R_Students_Details`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
