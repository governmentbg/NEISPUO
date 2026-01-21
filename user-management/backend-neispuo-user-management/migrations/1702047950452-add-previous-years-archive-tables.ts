import { MigrationInterface, QueryRunner } from "typeorm";

export class AddPreviousYearsArchivedTables1702047950452 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `  
                CREATE TABLE azure_temp.ClassesArchivedPreviousYears (
                    RowID int NOT NULL,
                    WorkflowType int NOT NULL,
                    Title varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    ClassCode varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    OrgID varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    TermID int NULL,
                    TermName varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    TermStartDate datetime2 NULL,
                    TermEndDate datetime2 NULL,
                    InProcessing int DEFAULT 0 NULL,
                    ErrorMessage varchar(1000) COLLATE Cyrillic_General_CI_AS NULL,
                    CreatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                    UpdatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                    GUID varchar(255) COLLATE Cyrillic_General_CI_AS DEFAULT newid() NULL,
                    RetryAttempts int DEFAULT 0 NULL,
                    ClassID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    Status int DEFAULT 0 NOT NULL,
                    AzureID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    InProgressResultCount int DEFAULT 0 NULL,
                    isForArchivation int DEFAULT 0 NOT NULL
                );      
            `,
        );


        await queryRunner.query(
            `  
                ALTER TABLE azure_temp.ClassesArchivedPreviousYears ADD CONSTRAINT ClassesArchivedPreviousYears_FK FOREIGN KEY (WorkflowType) REFERENCES azure_temp.WorkflowTypes(RowID);
            `,
        );


        await queryRunner.query(
            `  
                ALTER TABLE azure_temp.ClassesArchivedPreviousYears ADD CONSTRAINT FK_ClassesArchivedPreviousYears_Status FOREIGN KEY (Status) REFERENCES azure_temp.EventStatus(RowID);
            `,
        );

        await queryRunner.query(
            ` 
                CREATE TABLE azure_temp.EnrollmentsArchivedPreviousYears (
                    RowID int NOT NULL,
                    WorkflowType int NOT NULL,
                    UserAzureID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    ClassAzureID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    OrganizationAzureID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    InProcessing int DEFAULT 0 NULL,
                    ErrorMessage varchar(1000) COLLATE Cyrillic_General_CI_AS NULL,
                    CreatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                    UpdatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                    GUID varchar(255) COLLATE Cyrillic_General_CI_AS DEFAULT newid() NULL,
                    RetryAttempts int DEFAULT 0 NULL,
                    Status int DEFAULT 0 NOT NULL,
                    InProgressResultCount int DEFAULT 0 NULL,
                    userPersonID int NULL,
                    organizationPersonID int NULL,
                    curriculumID int NULL,
                    UserRole varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    isForArchivation int DEFAULT 0 NOT NULL
                );
            `,
        );

        await queryRunner.query(
            ` 
                ALTER TABLE azure_temp.EnrollmentsArchivedPreviousYears ADD CONSTRAINT EnrollmentsArchivedPreviousYears_FK FOREIGN KEY (WorkflowType) REFERENCES azure_temp.WorkflowTypes(RowID);
            `,
        );
        await queryRunner.query(
            ` 
                ALTER TABLE azure_temp.EnrollmentsArchivedPreviousYears ADD CONSTRAINT Enrollments_Archived_Previous_Years_PersonID_FK FOREIGN KEY(userPersonID) REFERENCES core.Person(PersonID);
            `,
        );
        await queryRunner.query(
            ` 
                ALTER TABLE azure_temp.EnrollmentsArchivedPreviousYears ADD CONSTRAINT FK_EnrollmentsArchivedPreviousYears_Status FOREIGN KEY(Status) REFERENCES azure_temp.EventStatus(RowID);
            `,
        );

        await queryRunner.query(
            ` 
                CREATE TABLE azure_temp.OrganizationsArchivedPreviousYears(
                    RowID int NOT NULL,
                    WorkflowType int NOT NULL,
                    Name varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Description varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    PrincipalId varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    PrincipalName varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    PrincipalEmail varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    HighestGrade int NULL,
                    LowestGrade int NULL,
                    Phone varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    City varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Area varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Country varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    PostalCode varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Street varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    ErrorMessage varchar(1000) COLLATE Cyrillic_General_CI_AS NULL,
                    CreatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                    UpdatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                    GUID varchar(255) COLLATE Cyrillic_General_CI_AS DEFAULT newid() NULL,
                    RetryAttempts int DEFAULT 0 NULL,
                    OrganizationID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    Status int DEFAULT 0 NOT NULL,
                    InProcessing int DEFAULT 0 NULL,
                    Username varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    Password nvarchar(255) COLLATE Cyrillic_General_CI_AS DEFAULT 0 NULL,
                    AzureID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    InProgressResultCount int DEFAULT 0 NULL,
                    personID int NULL,
                    isForArchivation int DEFAULT 0 NOT NULL
                );
            `,
        );


        await queryRunner.query(
            ` 
                ALTER TABLE azure_temp.OrganizationsArchivedPreviousYears ADD CONSTRAINT FK_OrganizationsArchivedPreviousYears_Status FOREIGN KEY(Status) REFERENCES azure_temp.EventStatus(RowID);
        
            `,
        );

        await queryRunner.query(
            `
                ALTER TABLE azure_temp.OrganizationsArchivedPreviousYears ADD CONSTRAINT OrganizationsArchivedPreviousYears_FK FOREIGN KEY(WorkflowType) REFERENCES azure_temp.WorkflowTypes(RowID);
            `,
        );
        await queryRunner.query(
            ` 
                ALTER TABLE azure_temp.OrganizationsArchivedPreviousYears ADD CONSTRAINT OrganizationsArchivedPreviousYears_Person_FK FOREIGN KEY(personID) REFERENCES core.Person(PersonID);
            `,
        );

        await queryRunner.query(
            ` 
                CREATE TABLE azure_temp.UsersArchivedPreviousYears(
                    RowID int NOT NULL,
                    WorkflowType int NOT NULL,
                    Identifier varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    FirstName varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    MiddleName varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Surname varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Password varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Email varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Phone varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    Grade varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    SchoolId varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    BirthDate datetime2 NULL,
                    UserRole varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
                    AccountEnabled int NULL,
                    ErrorMessage varchar(1000) COLLATE Cyrillic_General_CI_AS NULL,
                    CreatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                    UpdatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                    GUID varchar(255) COLLATE Cyrillic_General_CI_AS DEFAULT newid() NULL,
                    RetryAttempts int DEFAULT 0 NULL,
                    UserID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    Status int DEFAULT 7 NOT NULL,
                    Username varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    InProcessing int DEFAULT 0 NULL,
                    DeletionType int DEFAULT NULL NULL,
                    AdditionalRole int DEFAULT NULL NULL,
                    HasNeispuoAccess int DEFAULT NULL NULL,
                    PersonID int NULL,
                    AssignedAccountantSchools nvarchar(255) COLLATE Cyrillic_General_CI_AS DEFAULT NULL NULL,
                    AzureID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                    InProgressResultCount int DEFAULT 0 NULL,
                    isForArchivation int DEFAULT 0 NOT NULL,
                    sisAccessSecondaryRole int DEFAULT NULL NULL
                );
            `,
        );


        await queryRunner.query(
            ` 
                ALTER TABLE azure_temp.UsersArchivedPreviousYears ADD CONSTRAINT UsersArchivedPreviousYears_PersonID_FK FOREIGN KEY(PersonID) REFERENCES core.Person(PersonID);
            `,
        );

        await queryRunner.query(
            ` 
                ALTER TABLE azure_temp.UsersArchivedPreviousYears ADD CONSTRAINT FK_UsersArchivedPreviousYears_Status FOREIGN KEY(Status) REFERENCES azure_temp.EventStatus(RowID);
            `,
        );

        await queryRunner.query(
            ` 
                ALTER TABLE azure_temp.UsersArchivedPreviousYears ADD CONSTRAINT UsersArchivedPreviousYears_FK FOREIGN KEY(WorkflowType) REFERENCES azure_temp.WorkflowTypes(RowID);
            `,
        );

        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'yearly-run-archive-users-previous-year', N'0 10 0 1 8 *', 0, 0, 0);
                `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'yearly-run-archive-enrollments-previous-year', N'0 10 0 1 8 *', 0, 0, 0);
                `,
        );

        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'yearly-run-archive-organizations-previous-year', N'0 10 0 1 8 *', 0, 0, 0);
                `,
        );


        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'yearly-run-archive-classes-previous-year', N'0 10 0 1 8 *', 0, 0, 0);
                `,
        );


        await queryRunner.query(
            ` 
                CREATE PROCEDURE  azure_temp.ARCHIVE_CLASSES_PREVIOUS_YEAR AS
                    BEGIN
                        BEGIN TRY
                            BEGIN TRANSACTION
                                DECLARE @UpperLimitInt INT;
                                SELECT @UpperLimitInt = CurrentYearID 
                                FROM inst_basic.CurrentYear cy WHERE cy.IsValid = 1
                                DECLARE @UpperLimitDate DATETIME = DATEFROMPARTS(@UpperLimitInt, 7, 30);
                                DECLARE @LowerLimitDate DATETIME = DATEADD(year,-1,@UpperLimitDate);
                                INSERT
                                    INTO
                                    "azure_temp"."ClassesArchivedPreviousYears"
                                SELECT
                                    *
                                FROM
                                    "azure_temp"."ClassesArchived"
                                    WHERE CreatedOn  > @LowerLimitDate AND CreatedOn < @UpperLimitDate;
                                DELETE FROM "azure_temp"."ClassesArchived"
                                WHERE CreatedOn  > @LowerLimitDate AND CreatedOn < @UpperLimitDate;
                            COMMIT;
                        END TRY
                        BEGIN CATCH
                            ROLLBACK;
                        END CATCH
                    END;
            `,
        );


        await queryRunner.query(
            `
                CREATE PROCEDURE  azure_temp.ARCHIVE_USERS_PREVIOUS_YEAR AS
                    BEGIN
                        BEGIN TRY
                            BEGIN TRANSACTION
                                DECLARE @UpperLimitInt INT;
                                SELECT @UpperLimitInt = CurrentYearID 
                                FROM inst_basic.CurrentYear cy WHERE cy.IsValid = 1
                                DECLARE @UpperLimitDate DATETIME = DATEFROMPARTS(@UpperLimitInt, 7, 30);
                                DECLARE @LowerLimitDate DATETIME = DATEADD(year,-1,@UpperLimitDate);
                                INSERT
                                    INTO
                                    "azure_temp"."UsersArchivedPreviousYears"
                                SELECT
                                    *
                                FROM
                                    "azure_temp"."UsersArchived"
                                    WHERE CreatedOn  > @LowerLimitDate AND CreatedOn < @UpperLimitDate;
                                DELETE FROM "azure_temp"."UsersArchived"
                                WHERE CreatedOn  > @LowerLimitDate AND CreatedOn < @UpperLimitDate;
                            COMMIT;
                        END TRY
                        BEGIN CATCH
                            ROLLBACK;
                        END CATCH
                    END;      
               
            `,
        );


        await queryRunner.query(
            `        
            CREATE PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS_PREVIOUS_YEAR AS
                BEGIN
                    BEGIN TRY
                        BEGIN TRANSACTION
                            DECLARE @UpperLimitInt INT;
                            SELECT @UpperLimitInt = CurrentYearID 
                            FROM inst_basic.CurrentYear cy WHERE cy.IsValid = 1
                            DECLARE @UpperLimitDate DATETIME = DATEFROMPARTS(@UpperLimitInt, 7, 30);
                            DECLARE @LowerLimitDate DATETIME = DATEADD(year,-1,@UpperLimitDate);
                            INSERT
                                INTO
                                "azure_temp"."EnrollmentsArchivedPreviousYears"
                            SELECT
                                *
                            FROM
                                "azure_temp"."EnrollmentsArchived"
                                WHERE CreatedOn  > @LowerLimitDate AND CreatedOn < @UpperLimitDate;
                            DELETE FROM "azure_temp"."EnrollmentsArchived"
                            WHERE CreatedOn  > @LowerLimitDate AND CreatedOn < @UpperLimitDate;
                        COMMIT;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK;
                    END CATCH
                END;   
            `,
        );


        await queryRunner.query(
            `        
                CREATE PROCEDURE  azure_temp.ARCHIVE_ORGANIZATIONS_PREVIOUS_YEAR AS
                    BEGIN
                        BEGIN TRY
                            BEGIN TRANSACTION
                                DECLARE @UpperLimitInt INT;
                                SELECT @UpperLimitInt = CurrentYearID 
                                FROM inst_basic.CurrentYear cy WHERE cy.IsValid = 1
                                DECLARE @UpperLimitDate DATETIME = DATEFROMPARTS(@UpperLimitInt, 7, 30);
                                DECLARE @LowerLimitDate DATETIME = DATEADD(year,-1,@UpperLimitDate);
                                INSERT
                                    INTO
                                    "azure_temp"."OrganizationsArchivedPreviousYears"
                                SELECT
                                    *
                                FROM
                                    "azure_temp"."OrganizationsArchived"
                                    WHERE CreatedOn  > @LowerLimitDate AND CreatedOn < @UpperLimitDate;
                                DELETE FROM "azure_temp"."OrganizationsArchived"
                                WHERE CreatedOn  > @LowerLimitDate AND CreatedOn < @UpperLimitDate;
                            COMMIT;
                        END TRY
                        BEGIN CATCH
                            ROLLBACK;
                        END CATCH
                    END; 
            `,
        );



    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        
        await queryRunner.query(
            ` 
                DROP TABLE azure_temp.UsersArchivedPreviousYears      
            `,
        );
        await queryRunner.query(
            ` 
                DROP TABLE azure_temp.OrganizationsArchivedPreviousYears      
            `,
        );
        await queryRunner.query(
            ` 
                DROP TABLE azure_temp.EnrollmentsArchivedPreviousYears     
            `,
        );
        await queryRunner.query(
            ` 
                DROP TABLE azure_temp.ClassesArchivedPreviousYears     
            `,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'yearly-run-archive-users-previous-year'
                    )
            `,
            undefined,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'yearly-run-archive-enrollments-previous-year'
                    )
            `,
            undefined,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'yearly-run-archive-organizations-previous-year'
                    )
            `,
            undefined,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'yearly-run-archive-classes-previous-year'
                    )
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.ARCHIVE_CLASSES_PREVIOUS_YEAR
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.ARCHIVE_USERS_PREVIOUS_YEAR
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.ARCHIVE_ENROLLMENTS_PREVIOUS_YEAR
            `,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE azure_temp.ARCHIVE_ORGANIZATIONS_PREVIOUS_YEAR
            `,
            undefined,
        );

    }

}
