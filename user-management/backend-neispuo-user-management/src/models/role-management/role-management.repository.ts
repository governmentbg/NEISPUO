import { Injectable } from '@nestjs/common';
import { RoleAssignmentRequestDTO } from 'src/common/dto/requests/role-assignment-create-request.dts';
import { RoleAssignmentResponseDTO } from 'src/common/dto/responses/role-assignment-response.dto';
import { RoleResponseDTO } from 'src/common/dto/responses/role-response.dto';
import { RoleAssignmentMapper } from 'src/common/mappers/role-assignment.mapper';
import { RoleMapper } from 'src/common/mappers/role.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class RoleManagementRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async insertRole(roleAssignmentCreateRequestDTO: RoleAssignmentRequestDTO) {
        const { sysUserID, sysRoleID, institutionID } = roleAssignmentCreateRequestDTO;
        const result = await this.entityManager.query(
            `
            INSERT INTO
                core.SysUserSysRole (
                    SysUserID,
                    SysRoleID,
                    InstitutionID
                )
            OUTPUT
                    INSERTED."SysUserID" AS "sysUserID",
                    INSERTED."SysRoleID" AS "sysRoleID",
                    INSERTED."InstitutionID" AS "institutionID"
     
            VALUES (
                @0,
                @1,
                @2
            );
            `,
            [sysUserID, sysRoleID, institutionID],
        );
        const transformedResult: RoleAssignmentResponseDTO[] = RoleAssignmentMapper.transform(result);
        return transformedResult[0];
    }

    async deleteRole(roleAssignmentCreateRequestDTO: RoleAssignmentRequestDTO) {
        const { sysUserID, sysRoleID, institutionID } = roleAssignmentCreateRequestDTO;
        const result = await this.entityManager.query(
            `
            DELETE FROM
                core.SysUserSysRole
            OUTPUT
                    DELETED."SysUserID" AS "sysUserID",
                    DELETED."SysRoleID" AS "sysRoleID",
                    DELETED."InstitutionID" AS "institutionID"
            WHERE 
                SysUserID = @0 AND
                SysRoleID = @1 AND
                InstitutionID = @2
            `,
            [sysUserID, sysRoleID, institutionID],
        );
        const transformedResult: RoleAssignmentResponseDTO[] = RoleAssignmentMapper.transform(result);
        return transformedResult[0];
    }

    async getRoleAssignmentsByUserID(roleAssignmentCreateRequestDTO: RoleAssignmentRequestDTO) {
        const { sysUserID } = roleAssignmentCreateRequestDTO;
        const result = await this.entityManager.query(
            `
            SELECT
                sysUserID,
                sysRoleID,
                institutionID
            FROM
                core.SysUserSysRole
            WHERE
                SysUserID = @0
            `,
            [sysUserID],
        );
        const transformedResult: RoleAssignmentResponseDTO[] = RoleAssignmentMapper.transform(result);
        return transformedResult;
    }

    async getSysRoleBySysRoleID(sysRoleID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                sr.Name AS sysRoleName
            FROM
                core.SysRole sr
            WHERE
                sr.SysRoleID = @0
            `,
            [sysRoleID],
        );
        const transformedResult: RoleResponseDTO[] = RoleMapper.transform(result);
        return transformedResult[0];
    }

    async getCountOfRolesByUserAndRole(roleAssignmentRequestDTO: RoleAssignmentRequestDTO): Promise<number> {
        const result = await this.entityManager.query(
            `
            SELECT COUNT(susr.InstitutionID) as count
            FROM
                core.SysUserSysRole susr
            WHERE
                susr.SysUserID = @0
            AND
                susr.SysRoleID = @1
            `,
            [roleAssignmentRequestDTO.sysUserID, roleAssignmentRequestDTO.sysRoleID],
        );

        return result[0].count;
    }
}
