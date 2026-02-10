import { MigrationInterface, QueryRunner } from 'typeorm';

export class addRPersonalTable1692567560224 implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(
      `CREATE TABLE [reporting].R_Personal_Table (
    FirstName nvarchar(255),
    MiddleName nvarchar(255),
    LastName nvarchar(255),
    PermanentAddress nvarchar(2048),
    PermanentTownName nvarchar(1024),
    CurrentAddress nvarchar(2048),
    CurrentTownName nvarchar(1024),
    PublicEduNumber nvarchar(4000),
    NationalityName nvarchar(1024),
    BirthDate date,
    BirthPlaceTownName nvarchar(1024),
    BirthPlaceCountryName nvarchar(1024),
    GenderName nvarchar(255),
    PersonDetailTitle nvarchar(100),
    InstitutionID int,
    InstitutionName nvarchar(1024),
    InstitutionAbbreviation nvarchar(255),
    RegionName nvarchar(1024),
    MunicipalityName nvarchar(1024),
    TownName nvarchar(1024),
    LocalAreaName nvarchar(1024),
    TownID int,
    MunicipalityID int,
    RegionID int,
    LocalAreaID int,
    BudgetingSchoolTypeID int,
    BaseSchoolTypeName nvarchar(255),
    DetailedSchoolTypeName nvarchar(255),
    FinancialSchoolTypeName nvarchar(255),
    BudgetingInstitutionName nvarchar(1024),
    WorkStartYear int,
    WorkExpTotalYears int,
    WorkExpSpecYears int,
    WorkExpTeachYears int,
    StaffOrd int,
    StaffPositionNo int,
    PositionKindName nvarchar(255),
    StaffTypeName nvarchar(255),
    CategoryStaffTypeName varchar(255),
    PositionCount real,
    isNotMeetReq varchar(2),
    ContractWithName nvarchar(255),
    NKPDPositionName nvarchar(266),
    SubjectGroupName nvarchar(255),
    CurrentlyValid varchar(2),
    PositionNotes nvarchar(1024),
    ContractTypeName nvarchar(255),
    ContractReasonName nvarchar(255),
    ContractNo nvarchar(50),
    ContractYear int,
    ContractNotes nvarchar(1024),
    isAccountablePerson varchar(2),
    isTravel varchar(2),
    isExtendStudent varchar(2),
    isPensioneer varchar(2),
    isMentor varchar(2),
    isTrainee varchar(2),
    isHospital varchar(2),
    Norma float,
    NormaT float,
    ReductionHours float,
    LectYear real,
    SchoolYear smallint,
    PhoneNumber nvarchar(100),
    Email nvarchar(100),
    AcquiredPK nvarchar(1024),
    EducationGradeType nvarchar(255),
    SpecialityOKS nvarchar(1024),
    PKSType nvarchar(255),
    QCourseDurationCredits int
);
`,
    );
    await queryRunner.query(
      `
            CREATE NONCLUSTERED INDEX IX_RegionID ON reporting.R_Personal_Table(RegionID)
            CREATE NONCLUSTERED INDEX IX_RegionName ON reporting.R_Personal_Table(RegionName)
            CREATE NONCLUSTERED INDEX IX_MunicipalityID ON reporting.R_Personal_Table(MunicipalityID)
            CREATE NONCLUSTERED INDEX IX_MunicipalityName ON reporting.R_Personal_Table(MunicipalityName)
            CREATE NONCLUSTERED INDEX IX_TownID ON reporting.R_Personal_Table(TownID)
            CREATE NONCLUSTERED INDEX IX_TownName ON reporting.R_Personal_Table(TownName)
            `,
      undefined,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP TABLE reporting.R_Personal_Table `);

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
