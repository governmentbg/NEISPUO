import { MigrationInterface, QueryRunner } from 'typeorm';

export class ChangeUserView1670312952086 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.StudentTeacherUsers
            `,
        );
        await queryRunner.query(
            `
            CREATE VIEW [azure_temp].[StudentTeacherUsers]
            AS
                    SELECT 
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
                    "person"."schoolBooksCodesID" as schoolBooksCodesID,
                    CASE WHEN person.azureID IS NOT NULL THEN 1 ELSE 0 END as hasAzureID,
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
                    "person"."AzureID" as azureID,
                    "rol"."sysRoleID" as sysRoleID
                FROM 
                    "core"."Person" "person" 
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
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                DROP VIEW azure_temp.StudentTeacherUsers
            `,
        );

        await queryRunner.query(
            `
            CREATE VIEW [azure_temp].[StudentTeacherUsers]
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
                                "person"."schoolBooksCodesID" as schoolBooksCodesID,
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
                                "person"."AzureID" as azureID,
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
        );
    }
}
