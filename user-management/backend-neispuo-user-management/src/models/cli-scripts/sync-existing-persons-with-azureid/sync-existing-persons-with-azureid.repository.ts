import { Injectable } from '@nestjs/common';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { SysUserMapper } from 'src/common/mappers/sys-user.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncExistingPersonsWithAzureIDRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllUsersWithoutSysUserCount() {
        const result = await this.entityManager.query(
            `     
         SELECT COUNT(*) as count from core.Person
             INNER JOIN core.EducationalState as eduState ON person.PersonID=eduState.PersonID
             LEFT JOIN core.SysUser ON SysUser.PersonID = person.PersonID
        WHERE person.azureID is NOT NULL AND person.PublicEduNumber is NOT NULL and sysUser.sysUserID is NULL;
        `,
        );
        return result[0]?.count;
    }

    async getAllUsersWithoutSysUser() {
        const result = await this.entityManager.query(
            `  
            SELECT person.PersonID as personID, person.PublicEduNumber as PublicEduNumber, person.PersonalID as personalID, person.AzureID as azureID from core.Person
            INNER JOIN core.EducationalState as eduState ON Person.PersonID=eduState.PersonID
            LEFT JOIN core.SysUser ON SysUser.PersonID = Person.PersonID
            WHERE person.azureID is NOT NULL AND person.PublicEduNumber is NOT NULL and sysUser.sysUserID is NULL;
        `,
        );
        return result;
    }

    async addSysUser(username: string, personID: number) {
        const result = await getManager().query(
            `
        INSERT INTO core.SysUser
        (Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
        OUTPUT INSERTED.SysUserID as sysUserID, INSERTED.PersonID as personID
        VALUES (@0, NULL, 1, @1, 1, NULL, NULL);    `,
            [username, personID],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult[0];
    }
}
