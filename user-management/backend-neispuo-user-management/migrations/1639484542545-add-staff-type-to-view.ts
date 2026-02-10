import { MigrationInterface, QueryRunner } from 'typeorm';

export class EmptyMigration1639484542545 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
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
                    "person"."PublicEduNumber" as publicEduNumber,
                    "sp"."StaffTypeID" as staffTypeID,
                    "st"."Name" as staffTypeName,
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
                    LEFT JOIN "inst_basic"."StaffPosition" "sp" on
                        "person"."PersonID" = "sp"."PersonID"
                    LEFT JOIN "inst_nom"."StaffType" "st" on
                        "st"."StaffTypeID" = "sp"."StaffTypeID"
                    WHERE
                        "user"."DeletedOn" is NULL;
                  `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
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
}
