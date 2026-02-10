import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddIsForArchivationToAll1678817663751 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE azure_temp.Classes ADD isForArchivation int DEFAULT 0 NOT NULL;`,);
        await queryRunner.query( `ALTER TABLE azure_temp.Organizations ADD isForArchivation int DEFAULT 0 NOT NULL;`,);
        await queryRunner.query(`ALTER TABLE azure_temp.Users ADD isForArchivation int DEFAULT 0 NOT NULL;`,);
        await queryRunner.query(`ALTER TABLE azure_temp.ClassesArchived ADD isForArchivation int DEFAULT 0 NOT NULL;`,);
        await queryRunner.query(`ALTER TABLE azure_temp.OrganizationsArchived ADD isForArchivation int DEFAULT 0 NOT NULL;`);
        await queryRunner.query(`ALTER TABLE azure_temp.UsersArchived ADD isForArchivation int DEFAULT 0 NOT NULL;`);
        await queryRunner.query(`DROP PROCEDURE  azure_temp.ARCHIVE_CLASSES`,);
        await queryRunner.query(`DROP PROCEDURE  azure_temp.ARCHIVE_ORGANIZATIONS`,);
        await queryRunner.query(`DROP PROCEDURE azure_temp.ARCHIVE_USERS`,);
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
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'archive-failed-workflows',
                        'archive-failed-workflows2'
                    )
            `,
            undefined,
        );
        
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.Classes DROP COLUMN isForArchivation', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.Organizations DROP COLUMN isForArchivation', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.Users DROP COLUMN isForArchivation', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.ClassesArchived DROP COLUMN isForArchivation', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.OrganizationsArchived DROP COLUMN isForArchivation', undefined);
        await queryRunner.query('ALTER TABLE azure_temp.UsersArchived DROP COLUMN isForArchivation', undefined);
        await queryRunner.query(
            `
                DROP PROCEDURE  azure_temp.ARCHIVE_CLASSES
            `,
        );await queryRunner.query(
            `
                DROP PROCEDURE  azure_temp.ARCHIVE_ORGANIZATIONS
            `,
        );await queryRunner.query(
            `
                DROP PROCEDURE  azure_temp.ARCHIVE_USERS
            `,
        );
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
                                WHERE IsForArchivation IN (4,500);
                            DELETE FROM "azure_temp"."Classes"
                            WHERE  Status IN (4,500);
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
                                WHERE IsForArchivation IN (4,500);
                            DELETE FROM "azure_temp"."Organizations"
                            WHERE  Status IN (4,500);
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
                                WHERE IsForArchivation IN (4,500);
                            DELETE FROM "azure_temp"."Users"
                            WHERE  Status IN (4,500);
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
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'archive-failed-workflows', N'0 * * * * *', 0, 1, 0);
            `,
        );
        await queryRunner.query(
            `  
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'archive-failed-workflows2', N'0 * * * * *', 0, 1, 0);
            `,
        );
    }
}
