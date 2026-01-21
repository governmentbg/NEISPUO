import { Injectable } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { PersonMapper } from 'src/common/mappers/person.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncAzureIDsForExternalUsersRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getExternalUsersWithoutAzureIDs() {
        const result = await getManager().query(
            `
            Select * from core.sysUser su
JOIN core.Person p ON su.PersonID = p.PersonID
JOIN core.SysUserSysRole susr  ON su.SysUserID = susr.SysUserID 
Where azureID IS NULL and susr.SysRoleID IN(${RoleEnum.MON_ADMIN},${RoleEnum.MON_EXPERT},${RoleEnum.RUO},${RoleEnum.RUO_EXPERT},${RoleEnum.BUDGETING_INSTITUTION},${RoleEnum.CIOO},${RoleEnum.MON_USER_ADMIN},${RoleEnum.MON_OBGUM_FINANCES},${RoleEnum.MON_OBGUM},${RoleEnum.MON_CHRAO},${RoleEnum.NIO} )            `,
        );
        return result;
    }

    async getExternalUsersWithoutAzureIDsCount() {
        const result = await getManager().query(
            `
            Select count(*) as count from core.sysUser su
JOIN core.Person p ON su.PersonID = p.PersonID
JOIN core.SysUserSysRole susr  ON su.SysUserID = susr.SysUserID 
Where azureID IS NULL and susr.SysRoleID IN (${RoleEnum.MON_ADMIN},${RoleEnum.MON_EXPERT},${RoleEnum.RUO},${RoleEnum.RUO_EXPERT},${RoleEnum.BUDGETING_INSTITUTION},${RoleEnum.CIOO},${RoleEnum.MON_USER_ADMIN},${RoleEnum.MON_OBGUM_FINANCES},${RoleEnum.MON_OBGUM},${RoleEnum.MON_CHRAO},${RoleEnum.NIO} )
            `,
        );
        return result[0].count;
    }

    async updateAzureID(azureID: string, mail: string) {
        console.log(mail);
        const result = await getManager().query(
            `UPDATE core.Person
             SET
                    AzureID = @0
            OUTPUT INSERTED.PersonID as personID
            FROM core.Person p
            JOIN core.SysUser su ON p.PersonID=su.PersonID 
            WHERE
                su.username = @1`,
            [azureID, mail],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }
}
