import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class azureTempEnrollmentsHistory1633596062903 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE TABLE azure_temp.Enrollments_History (
                RowID int NOT NULL,
                WorkflowType varchar(255) NOT NULL,
                UserID varchar(255) NOT NULL,
                ClassID varchar(255) NOT NULL,
                OrganizationID varchar(255) NOT NULL,
                InProcessing int DEFAULT 0 NULL,
                ErrorMessage varchar(255) NULL,
                CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
                UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
                GUID varchar(255) NULL,
                RetryAttempts int DEFAULT 0 NULL,
                Status int DEFAULT 0 NOT NULL,
                CONSTRAINT PK__Enrollme__FD27BBEAD599348E PRIMARY KEY (RowID)
            );`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP TABLE azure_temp.Enrollments_History;', undefined);
    }
}
