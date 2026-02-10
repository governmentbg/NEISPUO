import { MigrationInterface, QueryRunner } from 'typeorm';

export class addRStudentsTable1692270542286 implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `CREATE TABLE reporting.R_Students_Table (
	ParentClassID int,
	ClassId int,
	InstitutionID int,
	InstitutionName nvarchar(1024),
	InstitutionAbbreviation nvarchar(255),
	RegionID int,
	RegionName nvarchar(1024),
	MunicipalityID int,
	MunicipalityName nvarchar(1024),
	TownID int,
	TownName nvarchar(1024),
	InstitutionDepartmentID int,
	InstitutionDepartmentName nvarchar(225),
	InstType int,
	BasicSchoolTypeName nvarchar(255),
	DetailedSchoolTypeName nvarchar(255),
	FinancialSchoolTypeName nvarchar(255),
	BudgetingSchoolTypeID int,
	IsInnovative nvarchar(2),
	IsCentral nvarchar(2),
	IsProtected nvarchar(2),
	IsStateFunded nvarchar(2),
	HasMunDecisionFor4 nvarchar(2),
	RomeClassName nvarchar(255),
	ClassName nvarchar(255),
	EduFormName nvarchar(255),
	ClassKind nvarchar(20),
	ClassType nvarchar(255),
	ProfName nvarchar(255),
	SpecialtyName nvarchar(255),
	PositionId int,
	StudentsCount int,
	StudentCSOPCount int,
	StudentSOPCount int,
	CountMale int,
	CountFemale int,
	RepeaterCount int,
	CommuterCount int,
	IsCSOP nvarchar(3),
	IsCDO nvarchar(3),
	IsHourlyOrganization int,
	IsNotPresentForm nvarchar(3),
	IsCombined nvarchar(3),
	SchoolYear int,
);`,
    );

    await queryRunner.query(
      `
            CREATE NONCLUSTERED INDEX IX_RegionID ON reporting.R_Students_Table(RegionID)
            CREATE NONCLUSTERED INDEX IX_RegionName ON reporting.R_Students_Table(RegionName)
            CREATE NONCLUSTERED INDEX IX_MunicipalityID ON reporting.R_Students_Table(MunicipalityID)
            CREATE NONCLUSTERED INDEX IX_MunicipalityName ON reporting.R_Students_Table(MunicipalityName)
            CREATE NONCLUSTERED INDEX IX_TownID ON reporting.R_Students_Table(TownID)
            CREATE NONCLUSTERED INDEX IX_TownName ON reporting.R_Students_Table(TownName)
			CREATE NONCLUSTERED INDEX IX_ClassID ON reporting.R_Students_Table(ClassId)
			CREATE NONCLUSTERED INDEX IX_ClassName ON reporting.R_Students_Table(ClassName)
            `,
      undefined,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP TABLE reporting.R_Students_Table `);

    await queryRunner.query(
      `
            DROP INDEX IX_RegionID 
            DROP INDEX IX_RegionName
            DROP INDEX IX_MunicipalityID
            DROP INDEX IX_MunicipalityName
            DROP INDEX IX_TownID
            DROP INDEX IX_TownName
			DROP INDEX IX_ClassID
			DROP INDEX IX_ClassName
            `,
      undefined,
    );
  }
}
