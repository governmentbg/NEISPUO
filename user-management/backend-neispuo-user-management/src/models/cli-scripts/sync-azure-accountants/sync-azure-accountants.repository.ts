import { Injectable } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { SysUserMapper } from 'src/common/mappers/sys-user.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncAzureAccountantsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllAccountantsCount() {
        const result = await getManager().query(
            `
            SELECT
                COUNT( DISTINCT su.SysUserID ) as count
            FROM
                core.SysUser su
            join core.SysUserSysRole susr on
                susr.SysUserID = su.SysUserID
            AND 
                su.DeletedOn is NULL
            AND 
                susr.SysRoleID = @0
            `,
            [RoleEnum.ACCOUNTANT],
        );
        return result[0].count;
    }

    async getPaginatedAccountants(paging: Paging) {
        const result = await getManager().query(
            `
            SELECT DISTINCT 
                su.SysUserID as sysUserID
            FROM
                core.SysUser su
            join core.SysUserSysRole susr on
                susr.SysUserID = su.SysUserID
            WHERE
                su.DeletedOn is NULL
            AND 
                susr.SysRoleID = @0
            ORDER BY
                su.SysUserID ASC OFFSET @1 ROWS FETCH NEXT @2 ROWS ONLY
            `,
            [RoleEnum.ACCOUNTANT, paging.from, paging.numberOfElements],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult;
    }
}
