import { Injectable } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { InstitutionResponseDTO } from 'src/common/dto/responses/institution-response.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { InstitutionMapper } from 'src/common/mappers/institution.mapper';
import { PersonMapper } from 'src/common/mappers/person.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncInstitutionsAzureIDsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllInstitutionsAccountsWithoutAzureIDsCount() {
        const result = await this.entityManager.query(
            `
                  
            SELECT
                count("ins"."InstitutionID") AS "count"
            FROM
                "core"."SysUser" "user"
            LEFT JOIN "core"."SysUserSysRole" "susr" ON
                "susr"."sysUserID" = "user"."sysUserID"
            LEFT JOIN "core"."Person" "person" ON
                "person"."personID" = "user"."personID"
            LEFT JOIN "core"."Institution" "ins" ON
                "ins"."InstitutionID" = "susr"."institutionID"
            LEFT JOIN "core"."sysRole" "rol" ON
                "rol"."sysRoleID" = "susr"."sysRoleID"
            LEFT JOIN "core"."EducationalState" "es" on "es"."PersonID" = "user"."PersonID"
                WHERE
                    1=1
                    AND es.EducationalStateID IS NULL
                    AND susr.SysRoleID = ${RoleEnum.INSTITUTION}
                    AND "user".DeletedOn IS NULL
                    AND "person".AzureID IS NULL
        `,
        );
        return result[0]?.count;
    }

    async getAllInstitutionsAccountsWithoutAzureIDs(paging: Paging) {
        const result = await this.entityManager.query(
            `  
            SELECT
                "ins".InstitutionID as institutionID,
                "person".PersonID as personID
            FROM
                "core"."SysUser" "user"
            LEFT JOIN "core"."SysUserSysRole" "susr" ON
                "susr"."sysUserID" = "user"."sysUserID"
            LEFT JOIN "core"."Person" "person" ON
                "person"."personID" = "user"."personID"
            LEFT JOIN "core"."Institution" "ins" ON
                "ins"."InstitutionID" = "susr"."institutionID"
            LEFT JOIN "core"."sysRole" "rol" ON
                "rol"."sysRoleID" = "susr"."sysRoleID"
            LEFT JOIN "core"."EducationalState" "es" on "es"."PersonID" = "user"."PersonID"
            WHERE
                1=1
                AND "es".EducationalStateID IS NULL
                AND "susr".sysRoleId = 0
                AND "user".DeletedOn is NULL
                AND "person".AzureID IS NULL
            ORDER BY
            "user"."sysUserID"  ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
        `,
        );
        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult;
    }

    async updateAzureID(azureID: string, personID: number) {
        const result = await getManager().query(
            `
            UPDATE core.Person SET
                    AzureID = @0
            OUTPUT INSERTED.PersonID as personID
            WHERE
                PersonID = @1
            `,
            [azureID, personID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }
}
