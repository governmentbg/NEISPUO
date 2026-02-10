import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddAccountantRole1641892192128 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`DBCC CHECKIDENT ('core.SysRole', RESEED, 19);`, undefined);
        await queryRunner.query(
            `
            INSERT
                INTO
                core.SysRole (Name,
                Description)
            VALUES ('Счетоводител',
            'Счетоводител');
            `,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Users ADD AdditionalRole INT CONSTRAINT AdditionalRoleUsersDefault DEFAULT NULL`,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE azure_temp.UsersArchived ADD AdditionalRole INT CONSTRAINT AdditionalRoleUsersHistoryDefault  DEFAULT NULL`,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER TABLE core.SysUserSysRole 
            DROP CONSTRAINT ValidateConditionalNotNulls`,
            undefined,
        );
        await queryRunner.query(
            `
        ALTER TABLE core.SysUserSysRole 
            ADD CONSTRAINT ValidateConditionalNotNulls CHECK
                
            (NOT ([SysRoleId] =(8)
            OR [SysRoleId] =(6)
            OR [SysRoleId] =(5))
            AND ((((
            (case
                when ([SysRoleID] =(14)
                OR [SysRoleID] =(0)
                OR [SysRoleID] =(20))
                AND [InstitutionID] IS NOT NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end
            +
            case
                when ([SysRoleID] =(19)
                OR [SysRoleID] =(18)
                OR [SysRoleID] =(17)
                OR [SysRoleID] =(16)
                OR [SysRoleID] =(15)
                OR [SysRoleID] =(14)
                OR [SysRoleID] =(13)
                OR [SysRoleID] =(12)
                OR [SysRoleID] =(11)
                OR [SysRoleID] =(10)
                OR [SysRoleID] =(1))
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end)
            +
            case
                when ([SysRoleID] =(9)
                OR [SysRoleID] =(2))
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NOT NULL then (1)
                else (0)
            end)
            +
            case
                when [SysRoleID] =(3)
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NOT NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end)
            +
            case
                when [SysRoleID] =(4)
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NOT NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end)
            +
            case
                when [SysRoleID] =(7)
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end)=(1))
        `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DELETE
            FROM
                core.SysRole
            WHERE
                Name = 'Счетоводител'
                AND Description = 'Счетоводител';
            `,
        );
        await queryRunner.query(`DBCC CHECKIDENT ('core.SysRole', RESEED, 19);`, undefined);

        await queryRunner.query(`ALTER TABLE azure_temp.Users DROP CONSTRAINT AdditionalRoleUsersDefault`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Users DROP COLUMN AdditionalRole`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.UsersArchived DROP CONSTRAINT AdditionalRoleUsersHistoryDefault`,
            undefined,
        );
        await queryRunner.query(`ALTER TABLE azure_temp.UsersArchived DROP COLUMN AdditionalRole`, undefined);
        await queryRunner.query(`DELETE FROM core.SysUserSysRole WHERE SysRoleID IN ( @0, @1, @2, @3, @4, @5)`, [
            RoleEnum.MON_OBGUM,
            RoleEnum.MON_OBGUM_FINANCES,
            RoleEnum.MON_CHRAO,
            RoleEnum.CONSORTIUM_HELPDESK,
            RoleEnum.NIO,
            RoleEnum.ACCOUNTANT,
        ]);
        await queryRunner.query(
            `
            ALTER TABLE core.SysUserSysRole 
            DROP CONSTRAINT ValidateConditionalNotNulls`,
            undefined,
        );
        await queryRunner.query(
            `
        ALTER TABLE core.SysUserSysRole 
            ADD CONSTRAINT ValidateConditionalNotNulls CHECK
                
                (NOT ([SysRoleId] =(8)
            OR [SysRoleId] =(6)
            OR [SysRoleId] =(5))
            AND ((((
            (case
                when ([SysRoleID] =(14)
                OR [SysRoleID] =(0))
                AND [InstitutionID] IS NOT NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end
            +
            case
                when ([SysRoleID] =(13)
                OR [SysRoleID] =(12)
                OR [SysRoleID] =(11)
                OR [SysRoleID] =(10)
                OR [SysRoleID] =(1))
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end)
            +
            case
                when ([SysRoleID] =(9)
                OR [SysRoleID] =(2))
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NOT NULL then (1)
                else (0)
            end)
            +
            case
                when [SysRoleID] =(3)
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NOT NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end)
            +
            case
                when [SysRoleID] =(4)
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NOT NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end)
            +
            case
                when [SysRoleID] =(7)
                AND [InstitutionID] IS NULL
                AND [BudgetingInstitutionID] IS NULL
                AND [MunicipalityID] IS NULL
                AND [RegionID] IS NULL then (1)
                else (0)
            end)=(1))
        `,
            undefined,
        );
    }
}
