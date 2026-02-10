import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class fixHistoryProcedures1638525886582 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments_History ALTER COLUMN ClassID VARCHAR(100) NULL`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments_History ALTER COLUMN OrganizationID VARCHAR(100) NULL`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.Classes_History DROP CONSTRAINT PK__Classes___CB1927A058D07992;`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments_History DROP CONSTRAINT PK__Enrollme__FD27BBEAD599348E;`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.Users_History DROP CONSTRAINT PK__Users_Hi__1788CCACFDD4FDAD;`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Organizations_History DROP CONSTRAINT PK__Organiza__CADB0B72B04082B8;`,
            undefined,
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

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `WITH CTE1 AS(
                SELECT RowID,
                    RN = ROW_NUMBER()OVER(PARTITION BY RowID ORDER BY RowID)
                FROM azure_temp.Classes_History
             )
             DELETE FROM CTE1 WHERE RN > 1;`,
            undefined,
        );
        await queryRunner.query(
            `WITH CTE2 AS(
                SELECT RowID,
                    RN = ROW_NUMBER()OVER(PARTITION BY RowID ORDER BY RowID)
                FROM azure_temp.Enrollments_History
             )
             DELETE FROM CTE2 WHERE RN > 1;`,
            undefined,
        );
        await queryRunner.query(
            `WITH CTE3 AS(
                SELECT RowID,
                    RN = ROW_NUMBER()OVER(PARTITION BY RowID ORDER BY RowID)
                FROM azure_temp.Organizations_History
             )
             DELETE FROM CTE3 WHERE RN > 1;`,
            undefined,
        );
        await queryRunner.query(
            `WITH CTE4 AS(
                SELECT RowID,
                    RN = ROW_NUMBER()OVER(PARTITION BY RowID ORDER BY RowID)
                FROM azure_temp.Users_History
             )
             DELETE FROM CTE4 WHERE RN > 1;`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.Organizations_History ADD CONSTRAINT PK__Organiza__CADB0B72B04082B8 PRIMARY KEY (RowID);`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.Users_History ADD CONSTRAINT PK__Users_Hi__1788CCACFDD4FDAD PRIMARY KEY (RowID);`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments_History ADD CONSTRAINT PK__Enrollme__FD27BBEAD599348E PRIMARY KEY (RowID);`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.Classes_History ADD CONSTRAINT PK__Classes___CB1927A058D07992 PRIMARY KEY (RowID);`,
            undefined,
        );

        await queryRunner.query(`DELETE FROM azure_temp.Enrollments_History WHERE ClassID IS NULL;`, undefined);
        await queryRunner.query(`DELETE FROM azure_temp.Enrollments_History WHERE OrganizationID IS NULL;`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments_History ALTER COLUMN ClassID VARCHAR(100) NOT NULL;`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments_History ALTER COLUMN OrganizationID VARCHAR(100) NOT NULL;`,
            undefined,
        );

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_CLASSES;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_ENROLLMENTS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_ORGANIZATIONS;', undefined);

        await queryRunner.query('DROP PROCEDURE azure_temp.ARCHIVE_USERS;', undefined);

        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_CLASSES AS
                BEGIN
                    INSERT
                        INTO
                        "azure_temp"."Classes_History"
                    SELECT
                        *
                    FROM
                        "azure_temp"."Classes";
                    TRUNCATE TABLE "azure_temp"."Classes"
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_ENROLLMENTS AS
                BEGIN
                    INSERT
                        INTO
                        "azure_temp"."Enrollments_History"
                    SELECT
                        *
                    FROM
                        "azure_temp"."Enrollments";
                    TRUNCATE TABLE "azure_temp"."Enrollments"
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_ORGANIZATIONS AS
                BEGIN
                    INSERT
                        INTO
                        "azure_temp"."Organizations_History"
                    SELECT
                        *
                    FROM
                        "azure_temp"."Organizations";
                    TRUNCATE TABLE "azure_temp"."Organizations"
                END;`,
            undefined,
        );

        await queryRunner.query(
            `CREATE PROCEDURE  azure_temp.ARCHIVE_USERS AS
                BEGIN
                    INSERT
                        INTO
                        "azure_temp"."Users_History"
                    SELECT
                        *
                    FROM
                        "azure_temp"."Users";
                    TRUNCATE TABLE "azure_temp"."Users"
                END;`,
            undefined,
        );
    }
}
