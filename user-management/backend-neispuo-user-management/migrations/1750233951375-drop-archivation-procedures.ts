import { MigrationInterface, QueryRunner } from 'typeorm';

export class DropArchivationProcedures1750233951375 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`DROP PROCEDURE  azure_temp.ARCHIVE_CLASSES`);
        await queryRunner.query(`DROP PROCEDURE  azure_temp.ARCHIVE_CLASSES_PREVIOUS_YEAR`);

        await queryRunner.query(`DROP PROCEDURE azure_temp.ARCHIVE_USERS`);
        await queryRunner.query(`DROP PROCEDURE azure_temp.ARCHIVE_USERS_PREVIOUS_YEAR`);

        await queryRunner.query(`DROP PROCEDURE  azure_temp.ARCHIVE_ORGANIZATIONS`);
        await queryRunner.query(`DROP PROCEDURE azure_temp.ARCHIVE_ORGANIZATIONS_PREVIOUS_YEAR`);

        await queryRunner.query(`DROP PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS`);
        await queryRunner.query(`DROP PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS_PREVIOUS_YEAR`);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
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
                                WHERE IsForArchivation = 1;
                            DELETE FROM "azure_temp"."Classes"
                            WHERE IsForArchivation = 1;
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
                                WHERE IsForArchivation = 1;
                            DELETE FROM "azure_temp"."Users"
                            WHERE IsForArchivation = 1;
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
                                WHERE IsForArchivation = 1;
                            DELETE FROM "azure_temp"."Organizations"
                            WHERE IsForArchivation = 1;
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
                                WHERE IsForArchivation = 1;
                            DELETE FROM "azure_temp"."Enrollments"
                            WHERE IsForArchivation = 1;
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
    }
}
