import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateInstitutionView1634205639393 implements MigrationInterface {
    name = 'CreateInstitutionView1634205639393';

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
      CREATE VIEW azure_temp.InstitutionsTable
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
                  `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP VIEW azure_temp.InstitutionsTable;', undefined);
    }
}
