import { MigrationInterface, QueryRunner } from 'typeorm';

export class ChangeDefaultDates1644933217900 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'azure_temp'
                AND tables.name = 'Classes'
                AND all_columns.name = 'CreatedOn'


            EXEC('ALTER TABLE [azure_temp].[Classes] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE azure_temp.Classes ADD CONSTRAINT [DF_Classes_CreatedOn] DEFAULT getutcdate() FOR [CreatedOn];
        `);
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'azure_temp'
                AND tables.name = 'Classes'
                AND all_columns.name = 'UpdatedOn'


            EXEC('ALTER TABLE [azure_temp].[Classes] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE azure_temp.Classes ADD CONSTRAINT [DF_Classes_UpdatedOn] DEFAULT getutcdate() FOR [UpdatedOn];
        `);
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'azure_temp'
                AND tables.name = 'Organizations'
                AND all_columns.name = 'CreatedOn'


            EXEC('ALTER TABLE [azure_temp].[Organizations] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE azure_temp.Organizations ADD CONSTRAINT [DF_Organizations_CreatedOn] DEFAULT getutcdate() FOR [CreatedOn];
        `);
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'azure_temp'
                AND tables.name = 'Organizations'
                AND all_columns.name = 'UpdatedOn'


            EXEC('ALTER TABLE [azure_temp].[Organizations] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE azure_temp.Organizations ADD CONSTRAINT [DF_Organizations_UpdatedOn] DEFAULT getutcdate() FOR [UpdatedOn];
        `);
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'azure_temp'
                AND tables.name = 'Enrollments'
                AND all_columns.name = 'CreatedOn'


            EXEC('ALTER TABLE [azure_temp].[Enrollments] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE azure_temp.Enrollments ADD CONSTRAINT [DF_Enrollments_CreatedOn] DEFAULT getutcdate() FOR [CreatedOn];
        `);
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'azure_temp'
                AND tables.name = 'Enrollments'
                AND all_columns.name = 'UpdatedOn'


            EXEC('ALTER TABLE [azure_temp].[Enrollments] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE azure_temp.Enrollments ADD CONSTRAINT [DF_Enrollments_UpdatedOn] DEFAULT getutcdate() FOR [UpdatedOn];
        `);
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'azure_temp'
                AND tables.name = 'Users'
                AND all_columns.name = 'CreatedOn'


            EXEC('ALTER TABLE [azure_temp].[Users] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE azure_temp.Users ADD CONSTRAINT [DF_Users_CreatedOn] DEFAULT getutcdate() FOR [CreatedOn];
        `);
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'azure_temp'
                AND tables.name = 'Users'
                AND all_columns.name = 'UpdatedOn'


            EXEC('ALTER TABLE [azure_temp].[Users] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE azure_temp.Users ADD CONSTRAINT [DF_Users_UpdatedOn] DEFAULT getutcdate() FOR [UpdatedOn];
        `);
        await queryRunner.query(`
            DECLARE @DefaultConstraintName VARCHAR(100)= '';
            SELECT
                @DefaultConstraintName = default_constraints.name
            FROM
                sys.all_columns

                INNER JOIN
                sys.tables
                ON all_columns.object_id = tables.object_id

                INNER JOIN
                sys.schemas
                ON tables.schema_id = schemas.schema_id

                INNER JOIN
                sys.default_constraints
                ON all_columns.default_object_id = default_constraints.object_id
            WHERE
                    schemas.name = 'logs'
                AND tables.name = 'LoginAudit'
                AND all_columns.name = 'CreatedOn'


            EXEC('ALTER TABLE [logs].[LoginAudit] DROP CONSTRAINT ' + @DefaultConstraintName);
            ALTER TABLE logs.LoginAudit ADD CONSTRAINT [DF_LoginAudit_CreatedOn] DEFAULT getutcdate() FOR [CreatedOn];
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER TABLE azure_temp.Classes DROP CONSTRAINT [DF_Classes_CreatedOn];
            ALTER TABLE azure_temp.Classes ADD CONSTRAINT [DF__Classes__Created] DEFAULT getdate() FOR [CreatedOn];
            ALTER TABLE azure_temp.Classes DROP CONSTRAINT [DF_Classes_UpdatedOn];
            ALTER TABLE azure_temp.Classes ADD CONSTRAINT [DF__Classes__Updated] DEFAULT getdate() FOR [UpdatedOn];


            ALTER TABLE azure_temp.Organizations DROP CONSTRAINT [DF_Organizations_CreatedOn];
            ALTER TABLE azure_temp.Organizations ADD CONSTRAINT [DF__Organizat__Creat] DEFAULT getdate() FOR [CreatedOn];
            ALTER TABLE azure_temp.Organizations DROP CONSTRAINT [DF_Organizations_UpdatedOn];
            ALTER TABLE azure_temp.Organizations ADD CONSTRAINT [DF__Organizat__Updat] DEFAULT getdate() FOR [UpdatedOn];


            ALTER TABLE azure_temp.Enrollments DROP CONSTRAINT [DF_Enrollments_CreatedOn];
            ALTER TABLE azure_temp.Enrollments ADD CONSTRAINT [DF__Enrollmen__Creat] DEFAULT getdate() FOR [CreatedOn];
            ALTER TABLE azure_temp.Enrollments DROP CONSTRAINT [DF_Enrollments_UpdatedOn];
            ALTER TABLE azure_temp.Enrollments ADD CONSTRAINT [DF__Enrollmen__Updat] DEFAULT getdate() FOR [UpdatedOn];

            ALTER TABLE azure_temp.Users DROP CONSTRAINT [DF_Users_CreatedOn];
            ALTER TABLE azure_temp.Users ADD CONSTRAINT [DF__Users__CreatedOn] DEFAULT getdate() FOR [CreatedOn];
            ALTER TABLE azure_temp.Users DROP CONSTRAINT [DF_Users_UpdatedOn];
            ALTER TABLE azure_temp.Users ADD CONSTRAINT [DF__Users__UpdatedOn] DEFAULT getdate() FOR [UpdatedOn];


            ALTER TABLE logs.LoginAudit DROP CONSTRAINT [DF_LoginAudit_CreatedOn];
            ALTER TABLE logs.LoginAudit ADD CONSTRAINT [DF__LoginAudi__Creat] DEFAULT getdate() FOR [CreatedOn];

        `,
            undefined,
        );
    }
}
