import { MigrationInterface, QueryRunner } from 'typeorm';

export class RoleAuditView1638533759548 implements MigrationInterface {
    name = 'RoleAuditView1638533759548';

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE VIEW "logs"."RoleAudit"
            AS
                SELECT AuditId, SysUserID, Username, SysRoleId, DateUtc, Action, InstId, ObjectName, ObjectId, AuditModuleId, JSON_VALUE(Data,'$.AssignedToSysUserID') as AssignedToSysUserID, JSON_VALUE(Data,'$.AssignedToSysUsername') as AssignedToSysUsername, JSON_VALUE(Data,'$.AssignedSysRoleID') as AssignedSysRoleID, JSON_VALUE(Data,'$.AssignedSysRoleName') as AssignedSysRoleName, JSON_VALUE(Data,'$.AssignedInstitutionID') as AssignedInstitutionID
                FROM [logs].[Audit];`,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`DROP VIEW "logs"."RoleAudit"`);
    }
}
