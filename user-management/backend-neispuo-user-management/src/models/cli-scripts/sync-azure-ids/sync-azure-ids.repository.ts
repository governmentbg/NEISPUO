import { Injectable } from '@nestjs/common';
import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { PersonMapper } from 'src/common/mappers/person.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncAzureIDsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllUsersWithoutAzureIDs(paging: Paging) {
        const result = await getManager().query(
            `
            SELECT
                DISTINCT p.PersonalID
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            WHERE
                pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and p.PersonalID is not NULL
                and p.AzureID is NULL
            ORDER BY
                PersonalID  ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
            `,
        );
        return result;
    }

    async getAllUsersWithoutAzureIDsCount() {
        const result = await getManager().query(
            `
            SELECT
                COUNT(DISTINCT p.PersonalID) as count
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            WHERE
                pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and p.PersonalID is not NULL
                and p.AzureID is NULL
            `,
        );
        return result;
    }

    async updateAzureID(azureID: string, personID: string) {
        const result = await getManager().query(
            `
            UPDATE core.Person SET
                    AzureID = @0
            OUTPUT INSERTED.PersonID as personID
            WHERE
                PersonalID = @1
            `,
            [azureID, personID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }
}
