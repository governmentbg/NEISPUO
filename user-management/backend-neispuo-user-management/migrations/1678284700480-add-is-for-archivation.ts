import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddIsForArchivation1678284700480 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Enrollments ADD isForArchivation int DEFAULT 0 NOT NULL;
            `,
        );
        await queryRunner.query(
            `
        ALTER TABLE azure_temp.EnrollmentsArchived ADD isForArchivation int DEFAULT 0 NOT NULL;`,
            undefined,
        );
        await queryRunner.query(
            `
                DROP PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS
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
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.Enrollments DROP COLUMN isForArchivation', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.EnrollmentsArchived DROP COLUMN isForArchivation', undefined);
        await queryRunner.query(
            `
                DROP PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS
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
                                WHERE IsForArchivation IN (4,500);
                            DELETE FROM "azure_temp"."Enrollments"
                            WHERE  Status IN (4,500);
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
