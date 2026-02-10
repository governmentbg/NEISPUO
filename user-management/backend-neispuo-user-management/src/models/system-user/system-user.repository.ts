import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { SysUserMapper } from 'src/common/mappers/sys-user.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SystemUserRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async getSystemUserFromDatabase() {
        const result = await this.entityManager.query(
            `
            SELECT 
                PersonID as personID,
                Username as username
            FROM
                core.SysUser
            WHERE
                Username = '${CONSTANTS.SYS_USER_USERNAME}'
            `,
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult[0];
    }
}
