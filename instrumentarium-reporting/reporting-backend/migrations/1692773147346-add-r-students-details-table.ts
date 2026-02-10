import { MigrationInterface, QueryRunner } from 'typeorm';

export class addRStudentsDetailsTable1692773147346
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `CREATE TABLE reporting.R_Students_Details_Table (
	RegionID int,
    RegionName nvarchar(1024),
    MunicipalityID int,
    MunicipalityName nvarchar(1024),
    TownID int,
    TownName nvarchar(1024),
    InstitutionID int,
    BudgetingSchoolTypeID int,
    InstitutionName nvarchar(1024),
    InstitutionKind nvarchar(255),
    FirstName nvarchar(255),
    MiddleName nvarchar(255),
    LastName nvarchar(255),
    IdType nvarchar(255),
    PersonalID nvarchar(255),
    BirthDate date,
    PersonTown nvarchar(255), 
    PersonMunicipality nvarchar(1024),
    PersonRegion nvarchar(1024),
    RomeName nvarchar(255),
    ClassName nvarchar(255),
    EduFormName nvarchar(255),
    ClassType nvarchar(255),
    ProfName nvarchar(255),
    SpecName nvarchar(255),
    IsIndividualCurriculum varchar(2),
    IsTravel nvarchar(255),
    IsRepeatClass nvarchar(255),
    IsSOP varchar(2),
    IsRP varchar(2)
);`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
