import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class TransformToSysVerTables1639059252126 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DELETE FROM [azure_temp].[Classes];
            `,
        );
        await queryRunner.query(
            `
            DELETE FROM [azure_temp].[Enrollments];
            `,
        );
        await queryRunner.query(
            `
            DELETE FROM [azure_temp].[Organizations];
            `,
        );
        await queryRunner.query(
            `
            DELETE FROM [azure_temp].[Users];
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Classes]
            ADD 
                ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL CONSTRAINT defaultValidFromClasses DEFAULT GETDATE(),
                ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL CONSTRAINT defaultValidToClasses DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
                PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Classes] SET
                (SYSTEM_VERSIONING = ON
                (HISTORY_TABLE = [azure_temp].[ClassesHistory]));
            `,
        );
        await queryRunner.query(
            `
            EXEC sp_rename N'azure_temp.Classes_History',
                N'ClassesArchived',
                'OBJECT';
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Enrollments]
            ADD 
                ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL CONSTRAINT defaultValidFromEnrollments DEFAULT GETDATE(),
                ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL CONSTRAINT defaultValidToEnrollments DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
                PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Enrollments] SET
                (SYSTEM_VERSIONING = ON
                (HISTORY_TABLE = [azure_temp].[EnrollmentsHistory]));
            `,
        );
        await queryRunner.query(
            `
            EXEC sp_rename N'azure_temp.Enrollments_History',
                N'EnrollmentsArchived',
                'OBJECT';
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Organizations]
            ADD 
                ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL CONSTRAINT defaultValidFromOrganizations DEFAULT GETDATE(),
                ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL CONSTRAINT defaultValidToOrganizations DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
                PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Organizations] SET
                (SYSTEM_VERSIONING = ON
                (HISTORY_TABLE = [azure_temp].[OrganizationsHistory]));
            `,
        );
        await queryRunner.query(
            `
            EXEC sp_rename N'azure_temp.Organizations_History',
                N'OrganizationsArchived',
                'OBJECT';
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Users]
            ADD 
                ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL CONSTRAINT defaultValidFromUsers DEFAULT GETDATE(),
                ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL CONSTRAINT defaultValidToUsers DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
                PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Users] SET
                (SYSTEM_VERSIONING = ON
                (HISTORY_TABLE = [azure_temp].[UsersHistory]));
            `,
        );
        await queryRunner.query(
            `
            EXEC sp_rename N'azure_temp.Users_History',
                N'UsersArchived',
                'OBJECT';
            `,
        );

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_CLASSES;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_ENROLLMENTS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_ORGANIZATIONS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_USERS;', undefined);

        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.ARCHIVE_CLASSES AS
                BEGIN
                    BEGIN TRY
                        BEGIN TRANSACTION
                            INSERT
                                INTO
                                "azure_temp"."ClassesArchived"
                            SELECT
                                *
                            FROM
                                "azure_temp"."Classes"
                            WHERE RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} OR Status = ${EventStatus.SYNCHRONIZED};
                            DELETE FROM "azure_temp"."Classes"
                            WHERE RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} OR Status = ${EventStatus.SYNCHRONIZED};
                        COMMIT;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK;
                    END CATCH
                END;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS AS
                BEGIN
                    BEGIN TRY
				        BEGIN TRANSACTION
				        	INSERT
		                        INTO
		                        "azure_temp"."EnrollmentsArchived"
		                    SELECT
		                        *
		                    FROM
		                        "azure_temp"."Enrollments"
                                WHERE RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} OR Status = ${EventStatus.SYNCHRONIZED};
                            DELETE FROM "azure_temp"."Enrollments"
                            WHERE RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} OR Status = ${EventStatus.SYNCHRONIZED};
				        COMMIT;
				    END TRY
				    BEGIN CATCH
				       ROLLBACK;
				    END CATCH
                END;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.ARCHIVE_ORGANIZATIONS AS
                BEGIN
                    BEGIN TRY
				        BEGIN TRANSACTION
				        	INSERT
		                        INTO
		                        "azure_temp"."OrganizationsArchived"
		                    SELECT
		                        *
		                    FROM
		                        "azure_temp"."Organizations"
                            WHERE RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} OR Status = ${EventStatus.SYNCHRONIZED};
                            DELETE FROM "azure_temp"."Organizations"
                            WHERE RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} OR Status = ${EventStatus.SYNCHRONIZED};
				        COMMIT;
				    END TRY
				    BEGIN CATCH
				       ROLLBACK;
				    END CATCH
                END;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.ARCHIVE_USERS AS
                BEGIN
                    BEGIN TRY
                        BEGIN TRANSACTION
                            INSERT
                                INTO
                                "azure_temp"."UsersArchived"
                            SELECT
                                *
                            FROM
                                "azure_temp"."Users"
                                WHERE RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} OR Status = ${EventStatus.SYNCHRONIZED};
                            DELETE FROM "azure_temp"."Users"
                            WHERE RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} OR Status = ${EventStatus.SYNCHRONIZED};
                        COMMIT;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK;
                    END CATCH
                END;
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            EXEC sp_rename N'azure_temp.ClassesArchived',
                N'Classes_History',
                'OBJECT';
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Classes] SET (SYSTEM_VERSIONING = OFF);
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Classes] DROP CONSTRAINT defaultValidFromClasses, defaultValidToClasses;
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Classes] DROP PERIOD FOR SYSTEM_TIME;
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Classes] DROP COLUMN ValidFrom, ValidTo;
            `,
        );
        await queryRunner.query(
            `
            DROP TABLE [azure_temp].[ClassesHistory]
            `,
        );

        await queryRunner.query(
            `
            EXEC sp_rename N'azure_temp.EnrollmentsArchived',
                N'Enrollments_History',
                'OBJECT';
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Enrollments] SET (SYSTEM_VERSIONING = OFF);
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Enrollments] DROP CONSTRAINT defaultValidFromEnrollments, defaultValidToEnrollments;
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Enrollments] DROP PERIOD FOR SYSTEM_TIME;
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Enrollments] DROP COLUMN ValidFrom, ValidTo;
            `,
        );
        await queryRunner.query(
            `
            DROP TABLE [azure_temp].[EnrollmentsHistory]
            `,
        );
        await queryRunner.query(
            `
            EXEC sp_rename N'azure_temp.OrganizationsArchived',
                N'Organizations_History',
                'OBJECT';
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Organizations] SET (SYSTEM_VERSIONING = OFF);
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Organizations] DROP CONSTRAINT defaultValidFromOrganizations, defaultValidToOrganizations;
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Organizations] DROP PERIOD FOR SYSTEM_TIME;
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Organizations] DROP COLUMN ValidFrom, ValidTo;
            `,
        );
        await queryRunner.query(
            `
            DROP TABLE [azure_temp].[OrganizationsHistory]
            `,
        );
        await queryRunner.query(
            `
            EXEC sp_rename N'azure_temp.UsersArchived',
                N'Users_History',
                'OBJECT';
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Users] SET (SYSTEM_VERSIONING = OFF);
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Users] DROP CONSTRAINT defaultValidFromUsers, defaultValidToUsers;
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Users] DROP PERIOD FOR SYSTEM_TIME;
            `,
        );
        await queryRunner.query(
            `
            ALTER TABLE [azure_temp].[Users] DROP COLUMN ValidFrom, ValidTo;
            `,
        );
        await queryRunner.query(
            `
            DROP TABLE [azure_temp].[UsersHistory]
            `,
        );

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_CLASSES;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_ENROLLMENTS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_ORGANIZATIONS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_USERS;', undefined);

        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_CLASSES AS
                BEGIN
                    BEGIN TRY
                        BEGIN TRANSACTION
                            INSERT
                                INTO
                                "azure_temp"."Classes_History"
                            SELECT
                                *
                            FROM
                                "azure_temp"."Classes";
                            DELETE FROM "azure_temp"."Classes"
                        COMMIT;
                    END TRY
                    BEGIN CATCH
                    ROLLBACK;
                    END CATCH
                END;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS AS
                BEGIN
                    BEGIN TRY
				        BEGIN TRANSACTION
				        	INSERT
		                        INTO
		                        "azure_temp"."Enrollments_History"
		                    SELECT
		                        *
		                    FROM
		                        "azure_temp"."Enrollments";
                            DELETE FROM "azure_temp"."Enrollments"
				        COMMIT;
				    END TRY
				    BEGIN CATCH
				       ROLLBACK;
				    END CATCH
                END;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.ARCHIVE_ORGANIZATIONS AS
                BEGIN
                    BEGIN TRY
				        BEGIN TRANSACTION
				        	INSERT
		                        INTO
		                        "azure_temp"."Organizations_History"
		                    SELECT
		                        *
		                    FROM
		                        "azure_temp"."Organizations";
                            DELETE FROM "azure_temp"."Organizations"
				        COMMIT;
				    END TRY
				    BEGIN CATCH
				       ROLLBACK;
				    END CATCH
                END;
            `,
            undefined,
        );

        await queryRunner.query(
            `
            CREATE PROCEDURE  azure_temp.ARCHIVE_USERS AS
                BEGIN
                    BEGIN TRY
                        BEGIN TRANSACTION
                            INSERT
                                INTO
                                "azure_temp"."Users_History"
                            SELECT
                                *
                            FROM
                                "azure_temp"."Users";
                            DELETE FROM "azure_temp"."Users"
                        COMMIT;
                    END TRY
                    BEGIN CATCH
                    ROLLBACK;
                    END CATCH
                END;
            `,
            undefined,
        );
    }
}
