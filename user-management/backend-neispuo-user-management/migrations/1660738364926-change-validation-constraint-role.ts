import { MigrationInterface, QueryRunner } from 'typeorm';

export class ChangeSysUserSysRoleValidationConstraint1660738364926 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER TABLE core.SysUserSysRole 
            DROP CONSTRAINT ValidateConditionalNotNulls`,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE core.SysUserSysRole WITH NOCHECK ADD CONSTRAINT ValidateConditionalNotNulls CHECK (NOT ([SysRoleId]=(8) OR [SysRoleId]=(6) OR [SysRoleId]=(5)) AND (((((case when ([SysRoleID]=(14) OR [SysRoleID]=(0) OR [SysRoleID]=(20)) AND [InstitutionID] IS NOT NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end+case when ([SysRoleID]=(22) OR [SysRoleID]=(21) OR [SysRoleID]=(19) OR [SysRoleID]=(18) OR [SysRoleID]=(17) OR [SysRoleID]=(16) OR [SysRoleID]=(15) OR [SysRoleID]=(14) OR [SysRoleID]=(13) OR [SysRoleID]=(12) OR [SysRoleID]=(11) OR [SysRoleID]=(10) OR [SysRoleID]=(1)) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)+case when ([SysRoleID]=(9) OR [SysRoleID]=(2)) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NOT NULL then (1) else (0) end)+case when [SysRoleID]=(3) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NOT NULL AND [RegionID] IS NULL then (1) else (0) end)+case when [SysRoleID]=(4) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NOT NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)+case when [SysRoleID]=(7) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)=(1));
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER TABLE core.SysUserSysRole 
            DROP CONSTRAINT ValidateConditionalNotNulls`,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE core.SysUserSysRole WITH NOCHECK ADD CONSTRAINT ValidateConditionalNotNulls CHECK (NOT ([SysRoleId]=(8) OR [SysRoleId]=(6) OR [SysRoleId]=(5)) AND (((((case when ([SysRoleID]=(14) OR [SysRoleID]=(0) OR [SysRoleID]=(20)) AND [InstitutionID] IS NOT NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end+case when ([SysRoleID]=(21) OR [SysRoleID]=(19) OR [SysRoleID]=(18) OR [SysRoleID]=(17) OR [SysRoleID]=(16) OR [SysRoleID]=(15) OR [SysRoleID]=(14) OR [SysRoleID]=(13) OR [SysRoleID]=(12) OR [SysRoleID]=(11) OR [SysRoleID]=(10) OR [SysRoleID]=(1)) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)+case when ([SysRoleID]=(9) OR [SysRoleID]=(2)) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NOT NULL then (1) else (0) end)+case when [SysRoleID]=(3) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NOT NULL AND [RegionID] IS NULL then (1) else (0) end)+case when [SysRoleID]=(4) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NOT NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)+case when [SysRoleID]=(7) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)=(1));
            `,
            undefined,
        );
    }
}
