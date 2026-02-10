import { MigrationInterface, QueryRunner } from 'typeorm';

export class ChangeEnrollmentTable1669222390149 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments ADD userPersonID INT NULL`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments ADD organizationPersonID INT NULL`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments ADD curriculumID INT NULL`, undefined);
        await queryRunner.query(
            `
            UPDATE
                e
            SET
                e.userPersonID = u.PersonID
            FROM
                azure_temp.Enrollments e
            JOIN azure_temp.Users u on
                e.UserRowID = u.RowID
            WHERE
                e.userPersonID IS NULL
           `,
            undefined,
        );
        await queryRunner.query(
            `
            UPDATE
                e
            SET
                e.curriculumID = c.ClassID
            FROM
                azure_temp.Enrollments e
            JOIN azure_temp.Classes c on
                e.ClassRowID  = c.RowID
            WHERE
                e.curriculumID IS NULL
           `,
            undefined,
        );

        await queryRunner.query(`ALTER TABLE azure_temp.Organizations ADD personID INT NULL`, undefined);
        await queryRunner.query(
            `
            UPDATE
                o
            SET
                o.personID = p.PersonID
            FROM
                azure_temp.Organizations o
            JOIN core.Institution i on
                o.OrganizationID  = i.InstitutionID
            JOIN 
                core.SysUserSysRole susr ON
                susr.InstitutionID = i.InstitutionID 
            JOIN core.SysUser su
            ON 
                susr.SysUserID = su.SysUserID 
                JOIN core.Person p 
                ON p.PersonID = su.PersonID
                WHERE
                1=1
                AND o.personID IS NULL
                AND susr.SysRoleID = 0
                AND su.Username = CONCAT(i.InstitutionID, '@edu.mon.bg') 
           `,
            undefined,
        );
        await queryRunner.query(
            ` 
            UPDATE
                e
            SET
                e.organizationPersonID  = o.personID
            FROM
                azure_temp.Enrollments e
            JOIN azure_temp.Organizations o on	
                e.OrganizationRowID  = o.RowID
            WHERE
                e.organizationPersonID IS NULL
           `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Enrollments ALTER COLUMN UserAzureID VARCHAR(100) NULL
            `,
            undefined,
        );
        await queryRunner.query(
            ` 
            DROP TABLE azure_temp.ClassesArchived;
           `,
            undefined,
        );

        await queryRunner.query(
            ` 
            CREATE TABLE azure_temp.ClassesArchived (
                RowID int  NOT NULL,
                WorkflowType varchar(255)  NOT NULL,
                Title varchar(255)  NULL,
                ClassCode varchar(255)  NULL,
                OrgID varchar(255)  NULL,
                TermID int NULL,
                TermName varchar(255)  NULL,
                TermStartDate datetime2 NULL,
                TermEndDate datetime2 NULL,
                InProcessing int DEFAULT 0 NULL,
                ErrorMessage varchar(1000)  NULL,
                CreatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                UpdatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                GUID varchar(255)  DEFAULT newid() NULL,
                RetryAttempts int DEFAULT 0 NULL,
                ClassID varchar(100)  NULL,
                Status int DEFAULT 0 NOT NULL,
                AzureID varchar(100)  NULL,
                InProgressResultCount int DEFAULT 0 NULL
            );
           `,
            undefined,
        );

        await queryRunner.query(
            ` 
            DROP TABLE azure_temp.EnrollmentsArchived;
           `,
            undefined,
        );

        await queryRunner.query(
            `   
            CREATE TABLE azure_temp.EnrollmentsArchived (
                RowID int NOT NULL,
                WorkflowType varchar(255)  NOT NULL,
                UserAzureID varchar(100)  NOT NULL,
                ClassAzureID varchar(100)  NULL,
                OrganizationAzureID varchar(100)  NULL,
                InProcessing int DEFAULT 0 NULL,
                ErrorMessage varchar(1000)  NULL,
                CreatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                UpdatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                GUID varchar(255)  DEFAULT newid() NULL,
                RetryAttempts int DEFAULT 0 NULL,
                Status int DEFAULT 0 NOT NULL,
                OrganizationRowID int NULL,
                UserRowID int NULL,
                ClassRowID int NULL,
                InProgressResultCount int DEFAULT 0 NULL,
                userPersonID int NULL,
                organizationPersonID int NULL,
                curriculumID int NULL
            );
           `,
            undefined,
        );

        await queryRunner.query(
            ` 
            DROP TABLE azure_temp.OrganizationsArchived;
           `,
            undefined,
        );

        await queryRunner.query(
            ` 
                        
            CREATE TABLE  azure_temp.OrganizationsArchived (
                RowID int NOT NULL,
                WorkflowType varchar(255)  NOT NULL,
                Name varchar(255)  NULL,
                Description varchar(255)  NULL,
                PrincipalId varchar(255)  NULL,
                PrincipalName varchar(255)  NULL,
                PrincipalEmail varchar(255)  NULL,
                HighestGrade int NULL,
                LowestGrade int NULL,
                Phone varchar(255)  NULL,
                City varchar(255)  NULL,
                Area varchar(255)  NULL,
                Country varchar(255)  NULL,
                PostalCode varchar(255)  NULL,
                Street varchar(255)  NULL,
                ErrorMessage varchar(1000)  NULL,
                CreatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                UpdatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                GUID varchar(255)  DEFAULT newid() NULL,
                RetryAttempts int DEFAULT 0 NULL,
                OrganizationID varchar(100)  NULL,
                Status int DEFAULT 0 NOT NULL,
                InProcessing int DEFAULT 0 NULL,
                Username varchar(100)  NULL,
                Password nvarchar(255)  DEFAULT 0 NULL,
                AzureID varchar(100)  NULL,
                InProgressResultCount int DEFAULT 0 NULL,
                personID int NULL
            );
           `,
            undefined,
        );

        await queryRunner.query(
            ` 
            DROP TABLE azure_temp.UsersArchived;
           `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE TABLE azure_temp.UsersArchived (
                RowID int NOT NULL,
                WorkflowType varchar(255)  NOT NULL,
                Identifier varchar(255)  NULL,
                FirstName varchar(255)  NULL,
                MiddleName varchar(255)  NULL,
                Surname varchar(255)  NULL,
                Password varchar(255)  NULL,
                Email varchar(255)  NULL,
                Phone varchar(255)  NULL,
                Grade varchar(255)  NULL,
                SchoolId varchar(255)  NULL,
                BirthDate datetime2 NULL,
                UserRole varchar(255)  NULL,
                AccountEnabled int NULL,
                ErrorMessage varchar(1000)  NULL,
                CreatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                UpdatedOn datetime2 DEFAULT getutcdate() NOT NULL,
                GUID varchar(255)  DEFAULT newid() NULL,
                RetryAttempts int DEFAULT 0 NULL,
                UserID varchar(100)  NULL,
                Status int DEFAULT 7 NOT NULL,
                Username varchar(100)  NULL,
                InProcessing int DEFAULT 0 NULL,
                DeletionType int DEFAULT NULL NULL,
                AdditionalRole int DEFAULT NULL NULL,
                HasNeispuoAccess int DEFAULT NULL NULL,
                PersonID int NULL,
                AssignedAccountantSchools nvarchar(255)  DEFAULT NULL NULL,
                AzureID varchar(100)  NULL,
                InProgressResultCount int DEFAULT 0 NULL
            );
           `,
            undefined,
        );

        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-users-archive', N'0 0 0 * * *', 0, 0, 0);
            `,
            undefined,
        );
        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-organizations-archive', N'0 0 0 * * *', 0, 0, 0);
            `,
            undefined,
        );
        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-enrollments-archive', N'0 0 0 * * *', 0, 0, 0);
            `,
            undefined,
        );
        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'azure-classes-archive', N'0 0 0 * * *', 0, 0, 0);
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments DROP COLUMN userPersonID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments DROP COLUMN organizationPersonID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments DROP COLUMN curriculumID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Organizations DROP COLUMN personID;`, undefined);
        await queryRunner.query(
            `
            DELETE 
            FROM 
                azure_temp.CronJobConfig 
            WHERE 
                Name IN (
                    'azure-users-archive',
                    'azure-organizations-archive',
                    'azure-enrollments-archive',
                    'azure-classes-archive'
                );
        `,
            undefined,
        );
    }
}
