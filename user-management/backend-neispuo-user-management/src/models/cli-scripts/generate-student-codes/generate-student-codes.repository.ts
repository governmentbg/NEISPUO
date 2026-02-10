import { Injectable } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { Filter } from 'src/common/dto/filter.dto';
import { Paging } from 'src/common/dto/paging.dto';
import { StudentResponseDTO } from 'src/common/dto/responses/student-response.dto';
import { StudentMapper } from 'src/common/mappers/student.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class GenerateStudentCodesRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllStudents(paging: Paging, filters: Filter[]) {
        const result = await this.entityManager.query(
            `
                SELECT
                    "user"."sysUserID" as sysUserID,
                    "user"."isAzureUser" as isAzureUser,
                    "user"."isAzureSynced" as isAzureSynced,
                    "user"."username" as username,
                    "person"."firstName" as firstName,
                    "person"."middleName" as middleName,
                    "person"."lastName" as lastName,
                    "person"."personID" as personID,
                    "ins"."InstitutionID" as institutionID,
                    "ins"."name" as institutionName,
                    "town"."name" as townName,
                    "mun"."name" as municipalityName,
                    "reg"."name" as regionName,
                    "pos"."name" as positionName,
                    "person"."personalID" as personalID
                FROM
                    "core"."SysUser" "user"
                LEFT JOIN "core"."Person" "person" ON
                    "person"."personID" = "user"."personID"
                LEFT JOIN "core"."EducationalState" "edu" ON
                    "edu"."personID" = "person"."personID"
                LEFT JOIN "core"."Institution" "ins" ON
                    "ins"."InstitutionID" = "edu"."institutionID"
                LEFT JOIN "location"."Town" "town" ON
                    "town"."townID" = "ins"."townID"
                LEFT JOIN "location"."Municipality" "mun" ON
                    "mun"."municipalityID" = "town"."municipalityID"
                LEFT JOIN "location"."Region" "reg" ON
                    "reg"."regionID" = "mun"."regionID"
                LEFT JOIN "core"."Position" "pos" ON
                    "pos"."positionID" = "edu"."positionID"
                LEFT JOIN "core"."sysRole" "rol" ON
                    "rol"."sysRoleID" = "pos"."sysRoleID"
                WHERE
                    1 = 1

                    ${filters['isAzureUser'] ? ' AND "user"."isAzureUser" = 1 ' : ' '}
                    ${filters['username'] ? ` AND "user"."username" LIKE '%${filters['username'].value}%'` : ' '}
                    ${
                        filters['institutionName']
                            ? ` AND "ins"."name" LIKE '%${filters['institutionName'].value}%'`
                            : ' '
                    }
                    ${filters['townName'] ? ` AND "town"."name"  LIKE '%${filters['townName'].value}%'` : ' '}
                    ${
                        filters['municipalityName']
                            ? ` AND "mun"."name" LIKE '%${filters['municipalityName'].value}%'`
                            : ' '
                    }
                    ${filters['regionName'] ? ` AND "reg"."name" LIKE '%${filters['regionName'].value}%'` : ' '}
                    ${
                        filters['institutionID']
                            ? ` AND CAST("ins"."InstitutionID" as CHAR)  LIKE '%${filters['institutionID'].value}%'`
                            : ' '
                    }
                    ${filters['positionName'] ? ` AND "pos"."name" LIKE '%${filters['positionName'].value}%'` : ' '}
                    ${
                        filters['personalID']
                            ? ` AND CAST("person"."personalID" as CHAR) LIKE '%${filters['personalID'].value}%'`
                            : ' '
                    }
                    ${
                        filters['threeNames']
                            ? ` AND ("person"."firstName" LIKE '%${filters['threeNames'].value}%'`
                            : ' '
                    }
                    ${filters['threeNames'] ? ` OR "person"."middleName" LIKE '%${filters['threeNames'].value}%'` : ' '}
                    ${filters['threeNames'] ? ` OR "person"."lastName" LIKE '%${filters['threeNames'].value}%')` : ' '}
                    
                    AND rol.sysRoleId = ${RoleEnum.STUDENT}
                    AND "user".DeletedOn is NULL
                ORDER BY
                "user"."sysUserID"  ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
                `,
        );
        const transformedResult: StudentResponseDTO[] = StudentMapper.transform(result);
        return transformedResult;
    }

    async getAllStudentsCount(filters: Filter[]) {
        const result = await this.entityManager.query(
            `
                SELECT
                    count("user"."SysUserID") as "count"
                FROM
                    "core"."SysUser" "user"
                LEFT JOIN "core"."Person" "person" ON
                    "person"."personID" = "user"."personID"
                LEFT JOIN "core"."EducationalState" "edu" ON
                    "edu"."personID" = "person"."personID"
                LEFT JOIN "core"."Institution" "ins" ON
                    "ins"."InstitutionID" = "edu"."institutionID"
                LEFT JOIN "location"."Town" "town" ON
                    "town"."townID" = "ins"."townID"
                LEFT JOIN "location"."Municipality" "mun" ON
                    "mun"."municipalityID" = "town"."municipalityID"
                LEFT JOIN "location"."Region" "reg" ON
                    "reg"."regionID" = "mun"."regionID"
                LEFT JOIN "core"."Position" "pos" ON
                    "pos"."positionID" = "edu"."positionID"
                LEFT JOIN "core"."sysRole" "rol" ON
                    "rol"."sysRoleID" = "pos"."sysRoleID"
                WHERE
                    1 = 1

                    ${filters['isAzureUser'] ? ' AND "user"."isAzureUser" = 1 ' : ' '}
                    ${filters['username'] ? ` AND "user"."username" LIKE '%${filters['username'].value}%'` : ' '}
                    ${
                        filters['institutionName']
                            ? ` AND "ins"."name" LIKE '%${filters['institutionName'].value}%'`
                            : ' '
                    }
                    ${filters['townName'] ? ` AND "town"."name"  LIKE '%${filters['townName'].value}%'` : ' '}
                    ${
                        filters['municipalityName']
                            ? ` AND "mun"."name" LIKE '%${filters['municipalityName'].value}%'`
                            : ' '
                    }
                    ${filters['regionName'] ? ` AND "reg"."name" LIKE '%${filters['regionName'].value}%'` : ' '}
                    ${
                        filters['institutionID']
                            ? ` AND CAST("ins"."InstitutionID" as CHAR)  LIKE '%${filters['institutionID'].value}%'`
                            : ' '
                    }
                    ${filters['positionName'] ? ` AND "pos"."name" LIKE '%${filters['positionName'].value}%'` : ' '}
                    ${
                        filters['personalID']
                            ? ` AND CAST("person"."personalID" as CHAR) LIKE '%${filters['personalID'].value}%'`
                            : ' '
                    }
                    ${
                        filters['threeNames']
                            ? ` AND ("person"."firstName" LIKE '%${filters['threeNames'].value}%'`
                            : ' '
                    }
                    ${filters['threeNames'] ? ` OR "person"."middleName" LIKE '%${filters['threeNames'].value}%'` : ' '}
                    ${filters['threeNames'] ? ` OR "person"."lastName" LIKE '%${filters['threeNames'].value}%')` : ' '}
                    
                    AND rol.sysRoleId = ${RoleEnum.STUDENT}
                    AND "user".DeletedOn is NULL
                `,
        );
        return result[0].count;
    }
}
