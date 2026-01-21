import { MigrationInterface, QueryRunner } from 'typeorm';

export class DropEnrollmentColumns1670416059907 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                DELETE FROM azure_temp.CronJobConfig WHERE NAME = 'azure-enrollments-to-class-create';
            `,
        );
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments ADD UserRole VARCHAR(100) NULL`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.EnrollmentsArchived ADD UserRole VARCHAR(100) NULL`, undefined);
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-student-to-class-create', N'*/5 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-teacher-to-class-create', N'*/30 * * * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `
            DROP INDEX [azure_temp].[Enrollments].[IX_Enrollments_OrganizationRowID];
            DROP INDEX [azure_temp].[Enrollments].[IX_Enrollments_UserRowID];
            DROP INDEX [azure_temp].[Enrollments].[IX_Enrollments_ClassRowID];
        `,
            undefined,
        );

        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Enrollments DROP COLUMN OrganizationRowID;
            `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Enrollments DROP COLUMN UserRowID;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Enrollments DROP COLUMN ClassRowID;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            ALTER TABLE azure_temp.EnrollmentsArchived DROP COLUMN OrganizationRowID;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            ALTER TABLE azure_temp.EnrollmentsArchived DROP COLUMN UserRowID;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            ALTER TABLE azure_temp.EnrollmentsArchived DROP COLUMN ClassRowID;
            `,
            undefined,
        );

        await queryRunner.query(
            `        
            UPDATE
                azure_temp.Enrollments 
            SET
                UserRole = 'TEACHER'
            WHERE
                RowID IN (
                    Select RowID  from azure_temp.Enrollments e JOIN core.EducationalState es ON e.userPersonID = es.PersonID WHERE es.PositionID = 2
                ) AND UserRole IS NULL

            `,
        );

        await queryRunner.query(
            `
            UPDATE
                azure_temp.Enrollments 
            SET
                UserRole = 'STUDENT'
            WHERE
                UserRole IS NULL
            `,
            undefined,
        );
    }
    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments ADD OrganizationRowID INT NULL', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments ADD UserRowID INT NULL', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments ADD ClassRowID INT NULL', undefined);

        await queryRunner.query(
            `
            CREATE NONCLUSTERED INDEX IX_Enrollments_OrganizationRowID ON azure_temp.Enrollments(OrganizationRowID)
            CREATE NONCLUSTERED INDEX IX_Enrollments_UserRowID ON azure_temp.Enrollments(UserRowID)
            CREATE NONCLUSTERED INDEX IX_Enrollments_ClassRowID ON azure_temp.Enrollments(ClassRowID)
        `,
            undefined,
        );

        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived ADD OrganizationRowID INT NULL', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived ADD UserRowID INT NULL', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived ADD ClassRowID INT NULL', undefined);

        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments DROP COLUMN UserRole;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.EnrollmentsArchived DROP COLUMN UserRole;`, undefined);
        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES(N'azure-enrollments-to-class-create', N'*/1 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `        
                DELETE FROM azure_temp.CronJobConfig WHERE NAME = 'azure-enrollments-student-to-class-create';
            `,
        );
        await queryRunner.query(
            `        
                DELETE FROM azure_temp.CronJobConfig WHERE NAME = 'azure-enrollments-teacher-to-class-create';
            `,
        );
    }
}
