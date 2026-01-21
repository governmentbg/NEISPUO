import { MigrationInterface, QueryRunner } from 'typeorm';

// eslint-disable-next-line @typescript-eslint/naming-convention
export class azureTempUsers1633596654147 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE TABLE azure_temp.Users (
                RowID int IDENTITY(1,1) NOT NULL,
                WorkflowType varchar(255) NOT NULL,
                Identifier varchar(255) NULL,
                FirstName varchar(255) NULL,
                MiddleName varchar(255) NULL,
                Surname varchar(255) NULL,
                Password varchar(255) NULL,
                Email varchar(255) NULL,
                Phone varchar(255) NULL,
                Grade varchar(255) NULL,
                SchoolId varchar(255) NULL,
                BirthDate datetime2(7) NULL,
                UserRole varchar(255) NULL,
                AccountEnabled int NULL,
                ErrorMessage varchar(255) NULL,
                CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
                UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
                GUID varchar(255) NULL,
                RetryAttempts int DEFAULT 0 NULL,
                UserID varchar(100) NULL,
                Status int DEFAULT 0 NOT NULL,
                Username varchar(100) NULL,
                InProcessing int DEFAULT 0 NULL,
                CONSTRAINT PK__Users__1788CCAC998A9D4E PRIMARY KEY (RowID)
            );`,
            undefined,
        );

        await queryRunner.query('ALTER TABLE azure_temp.Users ADD DeletionType INT DEFAULT NULL;', undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP TABLE azure_temp.Users;', undefined);
    }
}
