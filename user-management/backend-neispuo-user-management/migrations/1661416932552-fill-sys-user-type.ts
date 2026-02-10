import { MigrationInterface, QueryRunner } from 'typeorm';

export class FillSysUserType1661416932552 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            UPDATE
                P
            SET
                P.SysUserType = 1
            FROM (select p.SysUserType from core.Institution i JOIN core.SysUserSysRole susr on i.InstitutionID = susr.InstitutionID 
            JOIN core.SysUser su ON susr .SysUserID = su.SysUserID 
            JOIN core.Person p on p.PersonID = su.PersonID 
            WHERE 1=1
                AND p.SysUserType IS NULL
                AND susr.SysRoleID IN (0)) P
            `,
            undefined,
        );
        await queryRunner.query(
            `
            UPDATE
                P
            SET
                SysUserType = 0
            FROM (select p.SysUserType from core.SysUserSysRole susr
                JOIN core.SysUser su ON susr .SysUserID = su.SysUserID 
                JOIN core.Person p on p.PersonID = su.PersonID 
            WHERE 1=1
                AND p.SysUserType IS NULL
                AND susr.SysRoleID IN (1,2,3,4,9,10,11,12,13,14,15,16,17,18,19,21)) P
            `,
            undefined,
        );
        await queryRunner.query(
            `
            UPDATE
                P
            SET
                SysUserType = 2
            FROM (
            select p.SysUserType from core.Person p JOIN so.EducationalState es on p.PersonID = es.PersonID
            JOIN core.[Position] p2  on p2.PositionID = es.PositionID
            WHERE p2.SysRoleID = 5
			AND p.SysUserType IS NULL) P
            `,
            undefined,
        );
        await queryRunner.query(
            `
            UPDATE
                P
            SET
                SysUserType = 2
            FROM (select p.SysUserType from core.SysUserSysRole susr
                JOIN core.SysUser su ON susr .SysUserID = su.SysUserID 
                JOIN core.Person p on p.PersonID = su.PersonID 
            WHERE 1=1
                AND susr.SysRoleID IN (20)
                AND p.SysUserType IS NULL
                ) P
            `,
            undefined,
        );
        await queryRunner.query(
            `
            UPDATE
                P
            SET
                SysUserType = 2
            FROM (
            select p.SysUserType , es.PersonID, es.PositionID, es.SchoolYear   from core.Person p JOIN so.EducationalState es on p.PersonID = es.PersonID
            JOIN core.[Position] p2  on p2.PositionID = es.PositionID
            WHERE p2.SysRoleID = 6
			AND p.SysUserType IS NULL
            ) P
            `,
            undefined,
        );
        await queryRunner.query(
            `
            UPDATE
                P
            SET
                SysUserType = 3
            FROM
                (
                select
                    p.PersonID,
                    p.SysUserType
                from
                    azure_temp.Users u
                join core.Person p ON
                    u.PersonID = p.PersonID
                WHERE
                    u.WorkflowType = 'USER_CREATE'
                    and u.UserRole = 'PARENT'
                        ) P
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            UPDATE
                core.Person
            SET
                SysUserType = NULL
            WHERE 
                SysUserType IS NOT NULL
            `,
            undefined,
        );
    }
}
