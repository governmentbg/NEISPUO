import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateLoginAuditTable1636362617936 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE TABLE audit.LoginAudit (
                LoginAuditID int IDENTITY(1,1) NOT NULL,
                SysUserID int NOT NULL,
                Username VARCHAR(255) NULL,
                SysRoleID int NOT NULL,
                SysRoleName VARCHAR(255) NULL,
                InstitutionID int NULL,
                InstitutionName VARCHAR(255) NULL,
                RegionID int NULL,
                RegionName VARCHAR(255) NULL,
                MunicipalityID int NULL,
                MunicipalityName VARCHAR(255) NULL,
                BudgetingInstitutionID int NULL,
                BudgetingInstitutionName VARCHAR(255) NULL,
                PositionID int NULL,
                IPSource VARCHAR(255) NOT NULL,
                CreatedOn datetime2(7) DEFAULT getdate() NOT NULL
            );`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP TABLE audit.LoginAudit;', undefined);
    }
}
