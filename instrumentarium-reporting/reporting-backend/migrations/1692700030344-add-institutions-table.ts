import { MigrationInterface, QueryRunner } from 'typeorm';

export class addInstitutionsTable1692700030344 implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `
    CREATE TABLE 
    reporting.R_Institutions_Table (
      InstitutionID int,
      InstitutionName nvarchar(1024),
      ShortInstitutionName nvarchar(255),
      Bulstat nvarchar(13),
      CountryName nvarchar(1024),
      RegionName nvarchar(1024),
      MunicipalityName nvarchar(1024),
      TownName nvarchar(1024),
      LocalAreaName nvarchar(1024),
      TownID int,
      MunicipalityID int,
      RegionID int,
      LocalAreaID int,
      BudgetingSchoolTypeID int,
      PostCode int,
      BaseSchoolTypeName nvarchar(255),
      DetailedSchoolTypeName nvarchar(255),
      FinancialSchoolTypeName nvarchar(255),
      BudgetingInstitutionName nvarchar(1024),
      IsDelegateBudget varchar(2),
      YearlyBudget decimal(12,2),
      Address nvarchar(255),
      PhoneNumber nvarchar(4000),
      Email nvarchar(255),
      Website nvarchar(255),
      EstablishedYear nvarchar(255),
      ConstitActFirst nvarchar(1024),
      ConstitActLast nvarchar(1024),
      SchoolShiftType nvarchar(255),
      IsCentral varchar(2),
      IsProtected varchar(2),
      IsInnovative varchar(2),
      IsNational varchar(2),
      IsProfSchool varchar(2),
      IsNonIndDormitory varchar(2),
      HasMunDecisionFor4 varchar(2),
      IsAppInnovSystem varchar(2),
      IsODZ varchar(2),
      IsProvideEduServ varchar(2),
      Director nvarchar(511),
      InstitutionPublicCouncil varchar(2),
      StaffCountAll float,
      PedagogStaffCount float,
      NonpedagogStaffCount float,
    );`,
    );

    await queryRunner.query(
      `
        CREATE NONCLUSTERED INDEX IX_RegionID ON reporting.R_Institutions_Table(RegionID)
        CREATE NONCLUSTERED INDEX IX_RegionName ON reporting.R_Institutions_Table(RegionName)
        CREATE NONCLUSTERED INDEX IX_MunicipalityID ON reporting.R_Institutions_Table(MunicipalityID)
        CREATE NONCLUSTERED INDEX IX_MunicipalityName ON reporting.R_Institutions_Table(MunicipalityName)
        CREATE NONCLUSTERED INDEX IX_TownID ON reporting.R_Institutions_Table(TownID)
        CREATE NONCLUSTERED INDEX IX_TownName ON reporting.R_Institutions_Table(TownName)
            `,
      undefined,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP TABLE reporting.R_Institutions_Table `);

    await queryRunner.query(
      `
            DROP INDEX IX_RegionID 
            DROP INDEX IX_RegionName
            DROP INDEX IX_MunicipalityID
            DROP INDEX IX_MunicipalityName
            DROP INDEX IX_TownID
            DROP INDEX IX_TownName
            `,
      undefined,
    );
  }
}
