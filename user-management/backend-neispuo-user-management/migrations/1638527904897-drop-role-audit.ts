import { MigrationInterface, QueryRunner } from 'typeorm';

export class DropRoleAudit1638527904897 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`DROP TABLE audit.RoleAssignment`, undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE TABLE [audit].[RoleAssignment](
                [RoleAssignmentID] [int] IDENTITY(1,1) NOT NULL,
                [AssignedToSysUserID] [int] NOT NULL,
                [CreatedBySysUserID] [int] NULL,
                [SysRoleID] [int] NOT NULL,
                [CreatedOn] [datetime] NOT NULL,
                [Action] [nvarchar](256) NOT NULL,
                [InstitutionID] [int] NULL,
                [AssignedToSysUsername] [nvarchar](256) NULL,
                [CreatedBySysUsername] [nvarchar](256) NULL,
                [SysRoleName] [nvarchar](256) NULL
            ) ON [PRIMARY];
      `,
            undefined,
        );
    }
}
