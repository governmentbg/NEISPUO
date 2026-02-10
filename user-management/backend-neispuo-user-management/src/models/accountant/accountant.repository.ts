import { Injectable } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { AccountantResponseDTO } from 'src/common/dto/responses/accountant-response.dto';
import { AccountantMapper } from 'src/common/mappers/accountant.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class AccountantRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async getAccountantsByInstitutionID(institutionID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                susr.InstitutionID as "institutionID",
                u.SysUserID as "sysUserID",
                u.PersonID as "personID"
            FROM
                "core"."SysUserSysRole" "susr"
            JOIN 
                "core"."SysUser" "u" ON susr.SysUserID = u.SysUserID
            WHERE
            1 = 1
            AND "susr"."InstitutionID" = @0
            AND "susr"."SysRoleID" = @1
            AND "u"."DeletedOn" IS NULL
        `,
            [institutionID, RoleEnum.ACCOUNTANT],
        );

        const transformedResult: AccountantResponseDTO[] = AccountantMapper.transform(result);
        return transformedResult;
    }
}
