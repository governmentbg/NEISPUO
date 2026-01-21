import { Injectable } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { Filter } from 'src/common/dto/filter.dto';
import { Paging } from 'src/common/dto/paging.dto';
import { AzureOrganizationsResponseDTO } from 'src/common/dto/responses/azure-organizations-response.dto';
import { InstitutionResponseDTO } from 'src/common/dto/responses/institution-response.dto';
import { AzureOrganizationsMapper } from 'src/common/mappers/azure-organizations.mapper';
import { InstitutionMapper } from 'src/common/mappers/institution.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncAzureOrganizationsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllInstitutionsAccountsCount(filters: Filter[]) {
        const result = await this.entityManager.query(
            `
            SELECT
                count("user"."sysUserID") AS "count"
            FROM
                "core"."SysUser" "user"
            LEFT JOIN "core"."SysUserSysRole" "susr" ON
                "susr"."sysUserID" = "user"."sysUserID"
            LEFT JOIN "core"."Person" "person" ON
                "person"."personID" = "user"."personID"
            LEFT JOIN "core"."Institution" "ins" ON
                "ins"."InstitutionID" = "susr"."institutionID"
            LEFT JOIN "noms"."FinancialSchoolType" "finSchoolType" ON
                "finSchoolType"."financialSchoolTypeID" = "ins"."financialSchoolTypeID"
            LEFT JOIN "noms"."BaseSchoolType" "baseSchoolType" ON
                "baseSchoolType"."baseSchoolTypeID" = "ins"."baseSchoolTypeID"
            LEFT JOIN "noms"."DetailedSchoolType" "detSchoolType" ON
                "detSchoolType"."detailedSchoolTypeID" = "ins"."detailedSchoolTypeID"
            LEFT JOIN "location"."Town" "town" ON
                "town"."townID" = "ins"."townID"
            LEFT JOIN "location"."Municipality" "mun" ON
                "mun"."municipalityID" = "town"."municipalityID"
            LEFT JOIN "location"."Region" "reg" ON
                "reg"."regionID" = "mun"."regionID"
            LEFT JOIN "core"."sysRole" "rol" ON
                "rol"."sysRoleID" = "susr"."sysRoleID"
            WHERE
                1 = 1

                ${filters['isAzureUser'] ? ' AND "user"."isAzureUser" = 1 ' : ' '}
                ${filters['username'] ? ` AND "user"."username" LIKE '%${filters['username'].value}%'` : ' '}
                ${filters['institutionName'] ? ` AND "ins"."name" LIKE '%${filters['institutionName'].value}%'` : ' '}
                ${filters['townName'] ? ` AND "town"."name"  LIKE '%${filters['townName'].value}%'` : ' '}
                ${filters['municipalityName'] ? ` AND "mun"."name" LIKE '%${filters['municipalityName'].value}%'` : ' '}
                ${filters['regionName'] ? ` AND "reg"."name" LIKE '%${filters['regionName'].value}%'` : ' '}
                ${
                    filters['institutionID']
                        ? ` AND CAST("ins"."InstitutionID" as CHAR)  LIKE '%${filters['institutionID'].value}%'`
                        : ' '
                }
                ${
                    filters['financialSchoolTypeName']
                        ? ` AND "finSchoolType"."name" LIKE '%${filters['financialSchoolTypeName'].value}%'`
                        : ' '
                }
                ${
                    filters['baseSchoolTypeName']
                        ? ` AND "baseSchoolType"."name" LIKE '%${filters['baseSchoolTypeName'].value}%'`
                        : ' '
                }
                ${
                    filters['detailedSchoolTypeName']
                        ? ` AND "detSchoolType"."name" LIKE '%${filters['detailedSchoolTypeName'].value}%'`
                        : ' '
                }
                
                AND rol.sysRoleId = ${RoleEnum.INSTITUTION}
                AND "user".DeletedOn is NULL
                
            `,
        );
        return result[0].count;
    }

    async getAllInstitutionsAccounts(paging: Paging, filters: Filter[]) {
        const result = await this.entityManager.query(
            `
            SELECT
                "user"."sysUserID" AS "sysUserID",
                "user"."username" AS "username",
                "user"."isAzureUser" AS "isAzureUser",
                "finSchoolType"."name" AS "financialSchoolTypeName",
                "baseSchoolType"."name" AS "baseSchoolTypeName",
                "detSchoolType"."name" AS "detailedSchoolTypeName",
                "town"."name" AS "townName",
                "mun"."name" AS "municipalityName",
                "reg"."name" AS "regionName",
                "ins"."InstitutionID" AS "institutionID",
                "ins"."name" AS "institutionName"
            FROM
                "core"."SysUser" "user"
            LEFT JOIN "core"."SysUserSysRole" "susr" ON
                "susr"."sysUserID" = "user"."sysUserID"
            LEFT JOIN "core"."Person" "person" ON
                "person"."personID" = "user"."personID"
            LEFT JOIN "core"."Institution" "ins" ON
                "ins"."InstitutionID" = "susr"."institutionID"
            LEFT JOIN "noms"."FinancialSchoolType" "finSchoolType" ON
                "finSchoolType"."financialSchoolTypeID" = "ins"."financialSchoolTypeID"
            LEFT JOIN "noms"."BaseSchoolType" "baseSchoolType" ON
                "baseSchoolType"."baseSchoolTypeID" = "ins"."baseSchoolTypeID"
            LEFT JOIN "noms"."DetailedSchoolType" "detSchoolType" ON
                "detSchoolType"."detailedSchoolTypeID" = "ins"."detailedSchoolTypeID"
            LEFT JOIN "location"."Town" "town" ON
                "town"."townID" = "ins"."townID"
            LEFT JOIN "location"."Municipality" "mun" ON
                "mun"."municipalityID" = "town"."municipalityID"
            LEFT JOIN "location"."Region" "reg" ON
                "reg"."regionID" = "mun"."regionID"
            LEFT JOIN "core"."sysRole" "rol" ON
                "rol"."sysRoleID" = "susr"."sysRoleID"
            WHERE
                1 = 1

                ${filters['isAzureUser'] ? ' AND "user"."isAzureUser" = 1 ' : ' '}
                ${filters['username'] ? ` AND "user"."username" LIKE '%${filters['username'].value}%'` : ' '}
                ${filters['institutionName'] ? ` AND "ins"."name" LIKE '%${filters['institutionName'].value}%'` : ' '}
                ${filters['townName'] ? ` AND "town"."name"  LIKE '%${filters['townName'].value}%'` : ' '}
                ${filters['municipalityName'] ? ` AND "mun"."name" LIKE '%${filters['municipalityName'].value}%'` : ' '}
                ${filters['regionName'] ? ` AND "reg"."name" LIKE '%${filters['regionName'].value}%'` : ' '}
                ${
                    filters['institutionID']
                        ? ` AND CAST("ins"."InstitutionID" as CHAR)  LIKE '%${filters['institutionID'].value}%'`
                        : ' '
                }
                ${
                    filters['financialSchoolTypeName']
                        ? ` AND "finSchoolType"."name" LIKE '%${filters['financialSchoolTypeName'].value}%'`
                        : ' '
                }
                ${
                    filters['baseSchoolTypeName']
                        ? ` AND "baseSchoolType"."name" LIKE '%${filters['baseSchoolTypeName'].value}%'`
                        : ' '
                }
                ${
                    filters['detailedSchoolTypeName']
                        ? ` AND "detSchoolType"."name" LIKE '%${filters['detailedSchoolTypeName'].value}%'`
                        : ' '
                }
                
                AND rol.sysRoleId = ${RoleEnum.INSTITUTION}
                AND "user".DeletedOn is NULL
                
            ORDER BY
            "user"."sysUserID"  ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
            `,
        );
        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult;
    }

    async getAllAzureOrganizationsByOrganizationID(dto: AzureOrganizationsResponseDTO) {
        const { organizationID } = dto;
        const result = await this.entityManager.query(
            `
                SELECT
                    "RowID"
                FROM
                    azure_temp.Organizations
                WHERE
                    OrganizationID = @0
            `,
            [organizationID],
        );
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult;
    }

    async syncAzureOrganization(dto: AzureOrganizationsResponseDTO) {
        const {
            name,
            description,
            principalName,
            principalEmail,
            highestGrade,
            lowestGrade,
            phone,
            city,
            area,
            country,
            postalCode,
            street,
            organizationID,
            status,
            username,
        } = dto;
        const result = await this.entityManager.query(
            `           
                INSERT
                INTO
                azure_temp.Organizations (
                    WorkflowType,
                    Name,
                    Description,
                    PrincipalName,
                    PrincipalEmail,
                    HighestGrade,
                    LowestGrade,
                    Phone,
                    City,
                    Area,
                    Country,
                    PostalCode,
                    Street,
                    OrganizationID,
                    Status,
                    Username
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15);
            `,
            [
                WorkflowType.SCHOOL_CREATE,
                name,
                description,
                principalName,
                principalEmail,
                highestGrade,
                lowestGrade,
                phone,
                city,
                area,
                country,
                postalCode,
                street,
                organizationID,
                status,
                username,
            ],
        );
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult[0];
    }
}
