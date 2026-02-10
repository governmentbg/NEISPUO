import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { InstitutionResponseDTO } from 'src/common/dto/responses/institution-response.dto';
import { SysUserEntity } from 'src/common/entities/sys-user.entity';
import { InstitutionMapper } from 'src/common/mappers/institution.mapper';
import { EntityRepository, Repository, getManager } from 'typeorm';

@EntityRepository(SysUserEntity)
export class InstitutionRepository extends Repository<InstitutionRepository> {
    entityManager = getManager();

    async getInstitutionByInstitutionID(institutionID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                "ins"."name" AS "institutionName",
                "ins"."abbreviation" AS "description",
                null AS "highestGrade",
                null AS "lowestGrade",
                "ins"."InstitutionID" AS "institutionID",
                null AS "street",
                null AS "postalCode",
                su.sysUserID as sysUserID,
                p.PersonID as personID,
                p.AzureID as azureID

            FROM
                "core"."Institution" "ins"
                LEFT JOIN core.SysUserSysRole susr on "ins".InstitutionID = susr.InstitutionID
                LEFT JOIN core.SysUser su on su.SysUserID = susr.SysUserID
                LEFT JOIN core.Person p ON
                    p.personID = su.personID
            WHERE
                1 = 1
                AND ins.institutionID = @0
                AND su.DeletedOn IS NULL 
                AND  (su.Username = CONCAT(ins.institutionID, '@edu.mon.bg') OR su.Username IS NULL)
            `,
            [institutionID],
        );
        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult[0];
    }

    async getDeletedInstitutionByInstitutionID(institutionID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                "ins"."name" AS "institutionName",
                "ins"."abbreviation" AS "description",
                null AS "highestGrade",
                null AS "lowestGrade",
                "ins"."InstitutionID" AS "institutionID",
                null AS "street",
                null AS "postalCode",
                su.sysUserID as sysUserID,
                p.AzureID as azureID

            FROM
                "core"."Institution" "ins"
                LEFT JOIN core.SysUserSysRole susr on "ins".InstitutionID = susr.InstitutionID
                LEFT JOIN core.SysUser su on su.SysUserID = susr.SysUserID
                LEFT JOIN core.Person p ON
                    p.personID = su.personID
            WHERE
                1 = 1
                AND ins.institutionID = @0
                AND su.DeletedOn IS NOT NULL
                AND su.Username = CONCAT(ins.institutionID, '@edu.mon.bg') 
            `,
            [institutionID],
        );
        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult[0];
    }

    async getInstitutionByUserName(username: string) {
        const result = await this.entityManager.query(
            `
            SELECT
                "ins"."name" AS "institutionName",
                "ins"."abbreviation" AS "description",
                "usr"."sysUserID" AS "principalId",
                "person"."FirstName" AS "principalName",
                "usr"."Username" AS "principalEmail",
                null AS "highestGrade",
                null AS "lowestGrade",
                "ins"."InstitutionID" AS "institutionID",
                null AS "street",
                null AS "postalCode"

            FROM
                "core"."Institution" "ins"
            LEFT JOIN "core"."SysUserSysRole" "susr" ON
                "susr"."InstitutionID" = "ins"."InstitutionID"
            LEFT JOIN "core"."SysUser" "usr" ON
                "susr"."SysUserID" = "usr"."sysUserID"
            LEFT JOIN "core"."Person" "person" ON
                "person"."personID" = "usr"."personID"
            LEFT JOIN "location"."Town" "town" ON
                "town"."townID" = "ins"."townID"
            LEFT JOIN "core"."sysRole" "rol" ON
                "rol"."sysRoleID" = "susr"."sysRoleID"
            WHERE
                1 = 1
                AND usr.Username = @0
                AND usr.DeletedOn is NULL
            `,
            [username],
        );
        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult;
    }

    async getAccountantInstitutionsByUserID(sysUserID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                susr.InstitutionID as "institutionID"
            FROM
                "core"."SysUserSysRole" "susr"

                    WHERE
            1 = 1
            AND "susr"."SysUserID" = @0
            AND "susr"."SysRoleID" = @1
        `,
            [sysUserID, RoleEnum.ACCOUNTANT],
        );

        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult;
    }

    async getAccountantSysUserIDByPersonID(personID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                su.SysUserID as "sysUserID"
            FROM
                "core"."SysUser" "su"
            JOIN "core"."SysUserSysRole" "susr" ON "susr"."SysUserID" = "su"."SysUserID"
                    WHERE
            1 = 1
            AND "su"."PersonID" = @0
            AND "susr"."SysRoleID" = @1
            AND "su"."DeletedOn" IS NULL
        `,
            [personID, RoleEnum.ACCOUNTANT],
        );

        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult[0]?.sysUserID;
    }

    async getVUSysUserIDByPersonID(personID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                su.SysUserID as "sysUserID"
            FROM
                "core"."SysUser" "su"
            JOIN "core"."SysUserSysRole" "susr" ON "susr"."SysUserID" = "su"."SysUserID"
                    WHERE
            1 = 1
            AND "su"."PersonID" = @0
            AND "susr"."SysRoleID" = @1
            AND "su"."DeletedOn" IS NULL
        `,
            [personID, RoleEnum.VU_TEACHER],
        );

        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult[0]?.sysUserID;
    }

    getInstitutionWithoutSyncedTeachers(validFromDate: Date) {
        const formattedDate = validFromDate.toISOString().slice(0, 19).replace('T', ' ');
        return this.manager.query(
            `
                SELECT DISTINCT inst.InstitutionID from core.Person  per
                    LEFT JOIN core.EducationalState es on es.PersonID = per.PersonID
                    LEFT JOIN core.Institution inst on inst.InstitutionID = es.InstitutionID
                    LEFT JOIN SysUser su on su.PersonID = per.PersonID 
                    WHERE per.ValidFrom >= '${formattedDate}'
                        AND su.SysUserID is null AND  es.PositionID = ${PositionEnum.EMPLOYEE}
            `,
        );
    }

    getInstitutionWithoutSyncedStudents(validFromDate: Date) {
        const formattedDate = validFromDate.toISOString().slice(0, 19).replace('T', ' ');
        return this.manager.query(
            `
            SELECT DISTINCT inst.InstitutionID FROM core.Person  per
                LEFT JOIN core.EducationalState es ON es.PersonID = per.PersonID
                LEFT JOIN core.Institution inst ON inst.InstitutionID = es.InstitutionID
                LEFT JOIN SysUser su ON su.PersonID = per.PersonID 
                WHERE per.ValidFrom >= '${formattedDate}'
                    AND su.SysUserID IS NULL AND es.PositionID IN (${PositionEnum.STUDENT_INSTITUTION}, \
                        ${PositionEnum.STUDENT_OTHER_INSTITUTION}, \
                        ${PositionEnum.STUDENT_PLR}, \
                        ${PositionEnum.UNATTENTDING}, \
                        ${PositionEnum.SPECIAL_STUDENT}) 
                    AND per.PersonalID IS NOT NULL
                    AND su.DeletedOn IS NULL
            `,
        );
    }

    async getUnsyncedInstitutions() {
        const result = await this.entityManager.query(
            `
                EXEC [azure_temp].[DAILY_PROCEDURE_ORGANIZATIONS_CREATE]
            `,
            [],
        );
        const transformedResult: InstitutionResponseDTO[] = InstitutionMapper.transform(result);
        return transformedResult;
    }
}
