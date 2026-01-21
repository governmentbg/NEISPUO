import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class azureTempOrganizations1633596213245 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE TABLE azure_temp.Organizations (
                RowID int IDENTITY(1,1) NOT NULL,
                WorkflowType varchar(255) NOT NULL,
                Name varchar(255) NULL,
                Description varchar(255) NULL,
                PrincipalId varchar(255) NULL,
                PrincipalName varchar(255) NULL,
                PrincipalEmail varchar(255) NULL,
                HighestGrade int NULL,
                LowestGrade int NULL,
                Phone varchar(255) NULL,
                City varchar(255) NULL,
                Area varchar(255) NULL,
                Country varchar(255) NULL,
                PostalCode varchar(255) NULL,
                Street varchar(255) NULL,
                ErrorMessage varchar(255) NULL,
                CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
                UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
                GUID varchar(255) NULL,
                RetryAttempts int DEFAULT 0 NULL,
                OrganizationID varchar(100) NULL,
                Status int DEFAULT 0 NOT NULL,
                InProcessing int DEFAULT 0 NULL,
                Username varchar(100) NULL,
                CONSTRAINT PK__Organiza__CADB0B724B8EB191 PRIMARY KEY (RowID)
            );`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP TABLE azure_temp.Organizations;', undefined);
    }
}
