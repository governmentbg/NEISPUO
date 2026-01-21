import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class azureTempClasses1633595425155 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE TABLE azure_temp.Classes (
                RowID int IDENTITY(1,1) NOT NULL,
                WorkflowType varchar(255) NOT NULL,
                Title varchar(255) NULL,
                ClassCode varchar(255) NULL,
                OrgID varchar(255) NULL,
                TermID int NULL,
                TermName varchar(255) NULL,
                TermStartDate datetime2(7) NULL,
                TermEndDate datetime2(7) NULL,
                InProcessing int DEFAULT 0 NULL,
                ErrorMessage varchar(255) NULL,
                CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
                UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
                GUID varchar(255) NULL,
                RetryAttempts int DEFAULT 0 NULL,
                ClassID varchar(100) NULL,
                Status int DEFAULT 0 NOT NULL,
                CONSTRAINT PK__Classes__CB1927A07908B1CE PRIMARY KEY (RowID)
            );`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP TABLE azure_temp.Classes;', undefined);
    }
}
