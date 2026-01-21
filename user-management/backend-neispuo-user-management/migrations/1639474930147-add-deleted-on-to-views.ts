import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddDeletedOnToViews1639474930147 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER VIEW azure_temp.ExternalUserView
            AS
                SELECT
                    "user"."sysUserID" AS "sysUserID",
                    "user"."username" AS "username",
                    "user"."isAzureUser" AS "isAzureUser",
                    person.firstName as firstName,
                    person.middleName as middleName,
                    person.lastName as lastName,
                    CONCAT(person.firstName, ' ', person.middleName,  ' ', person.lastName) as "threeNames",
                    "finSchoolType"."name" AS "financialSchoolTypeName",
                    "finSchoolType"."financialSchoolTypeID" AS "financialSchoolTypeID",
                    "baseSchoolType"."name" AS "baseSchoolTypeName",
                    "baseSchoolType"."baseSchoolTypeID" AS "baseSchoolTypeID",
                    "detSchoolType"."name" AS "detailedSchoolTypeName",
                    "detSchoolType"."detailedSchoolTypeID" AS "detailedSchoolTypeID",
                    "town"."name" AS "townName",
                    "mun"."municipalityID" AS "municipalityID",
                    "mun"."name" AS "municipalityName",
                    "reg"."regionID" AS "regionID",
                    "reg"."name" AS "regionName",
                    "ins"."InstitutionID" AS "institutionID",
                    "ins"."name" AS "institutionName",
                    "rol"."SysRoleID" AS "SysRoleID",
                    "rol"."Name" AS "RoleName",
                    "bi"."BudgetingInstitutionID" as "budgetingInstitutionID",
                    "bi"."name" AS "budgetingInstitutionName"
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
                    "mun"."municipalityID" = "susr"."municipalityID"
                LEFT JOIN "location"."Region" "reg" ON
                    "reg"."regionID" = "susr"."regionID"
                LEFT JOIN "noms"."BudgetingInstitution" "bi" ON
                    "bi"."BudgetingInstitutionID" = "susr"."BudgetingInstitutionID"
                LEFT JOIN "core"."sysRole" "rol" ON
                    "rol"."sysRoleID" = "susr"."sysRoleID"
                WHERE
                    "user"."DeletedOn" is NULL;
                  `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER VIEW azure_temp.InstitutionsTable
            AS
                SELECT
                    "user"."sysUserID" AS "sysUserID",
                    "user"."username" AS "username",
                    "user"."isAzureUser" AS "isAzureUser",
                    "finSchoolType"."name" AS "financialSchoolTypeName",
                    "baseSchoolType"."name" AS "baseSchoolTypeName",
                    "detSchoolType"."name" AS "detailedSchoolTypeName",
                    "town"."name" AS "townName",
                    "mun"."name" AS "municipalityName",
                    "mun"."municipalityID" AS "municipalityID",
                    "reg"."name" AS "regionName",
                    "reg"."regionID" AS "regionID",
                    "ins"."InstitutionID" AS "institutionID",
                    "ins"."name" AS "institutionName",
                    "rol"."sysRoleID" as "sysRoleID"
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
                    "user"."DeletedOn" is NULL;
                  `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER VIEW azure_temp.StudentTeacherUsers
            AS
                SELECT
                    "user"."sysUserID" as sysUserID,
                    "user"."isAzureUser" as isAzureUser,
                    "user"."isAzureSynced" as isAzureSynced,
                    "user"."username" as username,
                    RTRIM(LTRIM(
                    CONCAT(
                        COALESCE("person"."firstName", ''),
                        COALESCE( ' ' + "person"."middleName", ''),
                        COALESCE( ' ' + "person"."lastName", '')
                    )
                    )) AS threeNames,
                    "person"."personID" as personID,
                    "person"."firstName" as firstName,
                    "person"."middleName" as middleName,
                    "person"."lastName" as lastName,
                    "ins"."InstitutionID" as institutionID,
                    "ins"."name" as institutionName,
                    "town"."townID" as townID,
                    "town"."name" as townName,
                    "mun"."municipalityID" as municipalityID,
                    "mun"."name" as municipalityName,
                    "reg"."regionid" as regionID,
                    "reg"."name" as regionName,
                    "pos"."name" as positionName,
                    "person"."personalID" as personalID,
                    "person"."PublicEduNumber" as PublicEduNumber,
                    "rol"."sysRoleID" as sysRoleID
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
                        "user"."DeletedOn" is NULL;
                  `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER VIEW azure_temp.ExternalUserView
            AS
                SELECT
                    "user"."sysUserID" AS "sysUserID",
                    "user"."username" AS "username",
                    "user"."isAzureUser" AS "isAzureUser",
                    person.firstName as firstName,
                    person.middleName as middleName,
                    person.lastName as lastName,
                    CONCAT(person.firstName, ' ', person.middleName,  ' ', person.lastName) as "threeNames",
                    "finSchoolType"."name" AS "financialSchoolTypeName",
                    "finSchoolType"."financialSchoolTypeID" AS "financialSchoolTypeID",
                    "baseSchoolType"."name" AS "baseSchoolTypeName",
                    "baseSchoolType"."baseSchoolTypeID" AS "baseSchoolTypeID",
                    "detSchoolType"."name" AS "detailedSchoolTypeName",
                    "detSchoolType"."detailedSchoolTypeID" AS "detailedSchoolTypeID",
                    "town"."name" AS "townName",
                    "mun"."municipalityID" AS "municipalityID",
                    "mun"."name" AS "municipalityName",
                    "reg"."regionID" AS "regionID",
                    "reg"."name" AS "regionName",
                    "ins"."InstitutionID" AS "institutionID",
                    "ins"."name" AS "institutionName",
                    "rol"."SysRoleID" AS "SysRoleID",
                    "rol"."Name" AS "RoleName",
                    "bi"."BudgetingInstitutionID" as "budgetingInstitutionID",
                    "bi"."name" AS "budgetingInstitutionName"
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
                    "mun"."municipalityID" = "susr"."municipalityID"
                LEFT JOIN "location"."Region" "reg" ON
                    "reg"."regionID" = "susr"."regionID"
                LEFT JOIN "noms"."BudgetingInstitution" "bi" ON
                    "bi"."BudgetingInstitutionID" = "susr"."BudgetingInstitutionID"
                LEFT JOIN "core"."sysRole" "rol" ON
                    "rol"."sysRoleID" = "susr"."sysRoleID";
                  `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER VIEW azure_temp.InstitutionsTable
            AS
                SELECT
                    "user"."sysUserID" AS "sysUserID",
                    "user"."username" AS "username",
                    "user"."isAzureUser" AS "isAzureUser",
                    "finSchoolType"."name" AS "financialSchoolTypeName",
                    "baseSchoolType"."name" AS "baseSchoolTypeName",
                    "detSchoolType"."name" AS "detailedSchoolTypeName",
                    "town"."name" AS "townName",
                    "mun"."name" AS "municipalityName",
                    "mun"."municipalityID" AS "municipalityID",
                    "reg"."name" AS "regionName",
                    "reg"."regionID" AS "regionID",
                    "ins"."InstitutionID" AS "institutionID",
                    "ins"."name" AS "institutionName",
                    "rol"."sysRoleID" as "sysRoleID"
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
                    "rol"."sysRoleID" = "susr"."sysRoleID";
                  `,
            undefined,
        );
        await queryRunner.query(
            `
            ALTER VIEW azure_temp.StudentTeacherUsers
            AS
                SELECT
                    "user"."sysUserID" as sysUserID,
                    "user"."isAzureUser" as isAzureUser,
                    "user"."isAzureSynced" as isAzureSynced,
                    "user"."username" as username,
                    RTRIM(LTRIM(
                    CONCAT(
                        COALESCE("person"."firstName", ''),
                        COALESCE( ' ' + "person"."middleName", ''),
                        COALESCE( ' ' + "person"."lastName", '')
                    )
                    )) AS threeNames,
                    "person"."personID" as personID,
                    "person"."firstName" as firstName,
                    "person"."middleName" as middleName,
                    "person"."lastName" as lastName,
                    "ins"."InstitutionID" as institutionID,
                    "ins"."name" as institutionName,
                    "town"."townID" as townID,
                    "town"."name" as townName,
                    "mun"."municipalityID" as municipalityID,
                    "mun"."name" as municipalityName,
                    "reg"."regionid" as regionID,
                    "reg"."name" as regionName,
                    "pos"."name" as positionName,
                    "person"."personalID" as personalID,
                    "person"."PublicEduNumber" as PublicEduNumber,
                    "rol"."sysRoleID" as sysRoleID
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
                                "rol"."sysRoleID" = "pos"."sysRoleID";
                  `,
            undefined,
        );
    }
}
