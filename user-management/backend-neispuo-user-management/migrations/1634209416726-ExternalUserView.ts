import { MigrationInterface, QueryRunner } from 'typeorm';

export class ExternalUserView1634209416726 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE VIEW azure_temp.ExternalUserView AS 
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
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP VIEW azure_temp.ExternalUserView; ', undefined);
    }
}
