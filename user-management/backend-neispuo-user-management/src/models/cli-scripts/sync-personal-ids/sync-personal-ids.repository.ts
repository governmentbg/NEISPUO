import { Injectable } from '@nestjs/common';
import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncPersonalIDsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllUsersWithAzureIDs(paging: Paging) {
        const result = await getManager().query(
            `
            SELECT
                DISTINCT p.PersonID as personID,
                p.AzureID as azureID,
                pos.SysRoleID as sysRoleID
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            left join core.SysUser su on
                su.PersonID = p.PersonID
            WHERE
                su.SysUserID is NOT NULL
                and pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and TRIM(p.PersonalID) LIKE '0%'
                and p.AzureID is NOT NULL
                and su.DeletedOn is NULL
            ORDER BY
                PersonID  ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
            `,
        );
        return result;
    }

    async getAllUsersWithAzureIDsCount() {
        const result = await getManager().query(
            `
            SELECT
                COUNT(DISTINCT p.PersonID) as count
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            left join core.SysUser su on
                su.PersonID = p.PersonID
            WHERE
                su.SysUserID is NOT NULL
                and pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and TRIM(p.PersonalID) LIKE '0%'
                and p.AzureID is NOT NULL
                and su.DeletedOn is NULL
            `,
        );
        return result;
    }
}
