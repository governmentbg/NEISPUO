import { Injectable } from '@nestjs/common';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { IsAzureSynced } from 'src/common/constants/enum/is-azure-synced.enum';
import { IsAzureUser } from 'src/common/constants/enum/is-azure-user.enum';
import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { SysUserCreateDTO } from 'src/common/dto/sys-user-create.dto';
import { PersonMapper } from 'src/common/mappers/person.mapper';
import { SysUserMapper } from 'src/common/mappers/sys-user.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncAzureUsersRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getNonSyncedUsersPersonalIDs(paging: Paging) {
        const result = await this.entityManager.query(
            `
            SELECT
                DISTINCT PersonalID
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            left join core.SysUser su on
                su.PersonID = p.PersonID
            WHERE
                pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and p.PersonalID is not NULL
                and p.PublicEduNumber is NULL
                and su.DeletedOn is NULL
            ORDER BY
                PersonalID  ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
            `,
        );
        return result;
    }

    async getNonSyncedUsersPersonalIDsCount() {
        const result = await this.entityManager.query(
            `
            SELECT
                COUNT( DISTINCT PersonalID ) as count
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            left join core.SysUser su on
                su.PersonID = p.PersonID
            WHERE
                pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and p.PersonalID is not NULL
                and p.PublicEduNumber is NULL
                and su.DeletedOn is NULL
            `,
        );
        return result[0].count;
    }

    async updateUserPublicEduNumber(publicEduNumber: string, personalID: string) {
        const result = await this.entityManager.query(
            `
            UPDATE core.Person SET
                    PublicEduNumber = @0
            OUTPUT INSERTED.PersonID as personID
            WHERE
                PersonalID = @1
            `,
            [publicEduNumber, personalID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }

    async updateUserAzureID(azureID: string, personalID: string) {
        const result = await this.entityManager.query(
            `
            UPDATE core.Person SET
                    AzureID = @0
            OUTPUT INSERTED.PersonID as personID
            WHERE
                PersonalID = @1
            `,
            [azureID, personalID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }

    async getSysUserByPersonID(personID: number) {
        const result = await this.entityManager.query(
            `            
                SELECT
                    su.SysUserID as sysUserID,
                    su.IsAzureUser as isAzureUser,
                    su.Username as username
                FROM
                    core.SysUser su
                WHERE
                    su.PersonID = @0 AND
                    su.DeletedOn is NULL
            `,
            [personID],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult[0];
    }

    async createSysUser(createUserDTO: SysUserCreateDTO) {
        const { username, personID } = createUserDTO;

        const result = await this.entityManager.query(
            `            
            INSERT INTO 
            core.SysUser (
                    Username,
                    PersonID,
                    IsAzureUser,
                    IsAzureSynced
                )
            OUTPUT 
                INSERTED.SysUserID as sysUserID,
                INSERTED.Username as username,
                INSERTED.SysUserID as sysUserID
            VALUES (
                @0,
                @1,
                @2,
                @3
            );
            `,
            [username, personID, IsAzureUser.YES, IsAzureSynced.YES],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult[0];
    }

    async getNonSyncedTeacherUserByInstitutionID(institutionID: number) {
        const query = `
        SELECT
            DISTINCT p.personID, p.personalID, p.azureID, p.validFrom
        FROM
            core.Person p
        join core.EducationalState es on
            p.PersonID = es.PersonID
        join core.Position pos on
            es.PositionID = pos.PositionID
        left join core.SysUser su on
            su.PersonID = p.PersonID
        WHERE
            1=1
            and su.SysUserID is NULL
            and pos.PositionID = ${PositionEnum.EMPLOYEE}
            and pos.SysRoleID = ${RoleEnum.TEACHER}
            and p.PersonalID is not NULL
            and su.DeletedOn is NULL
            and es.InstitutionID = ${institutionID}
        `;
        return this.entityManager.query(query);
    }

    async getNonSyncedStudentUserByInstitutionID(institutionID: number) {
        const query = `
        SELECT
        DISTINCT p.personID, p.personalID, p.validFrom
        FROM
            core.Person p
            LEFT JOIN core.EducationalState es ON
            p.PersonID = es.PersonID
            LEFT JOIN core.Position pos ON
            es.PositionID = pos.PositionID
            LEFT JOIN core.SysUser su ON
            su.PersonID = p.PersonID
            LEFT JOIN azure_temp.Users atu ON
            atu.Identifier = p.PersonalID
            WHERE
            1=1
            AND su.SysUserID IS NULL
            AND su.DeletedOn IS NULL
            AND pos.PositionID IN (${PositionEnum.STUDENT_INSTITUTION}, \
                ${PositionEnum.STUDENT_OTHER_INSTITUTION}, \
                ${PositionEnum.STUDENT_PLR}, \
                ${PositionEnum.UNATTENTDING}, \
                ${PositionEnum.SPECIAL_STUDENT})
                AND (pos.SysRoleID = ${RoleEnum.STUDENT}
                OR (pos.SysRoleID IS NULL
            AND pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
            AND p.PersonalID IS not NULL
            AND su.DeletedOn IS NULL
            AND es.InstitutionID = ${institutionID}
            `;
        return this.entityManager.query(query);
    }

    async azureUserRecordExists(personalID: string) {
        const result = await this.entityManager.query(
            `
            SELECT TOP 1 u.rowID FROM azure_temp.Users u
            WHERE u.Identifier = @0 AND u.WorkflowType = @1 AND u.Status IN (
                ${EventStatus.SUCCESSFUL},
                ${EventStatus.IN_CREATION},
                ${EventStatus.AWAITING_CREATION},
                ${EventStatus.SYNCHRONIZED}
                )
        `,
            [personalID, WorkflowType.USER_CREATE],
        );
        return result;
    }

    async getInstitutionWithoutSyncedTeachers(validFromDate: string) {
        const result = await this.entityManager.query(` 
        SELECT DISTINCT inst.InstitutionID from core.Person  per 
            LEFT JOIN core.EducationalState es on es.PersonID = per.PersonID 
            LEFT JOIN core.Institution inst on inst.InstitutionID = es.InstitutionID 
            LEFT JOIN core.SysUser su on su.PersonID = per.PersonID  
            WHERE per.ValidFrom >= '${validFromDate}' 
            AND su.SysUserID is null AND per.PersonalID is not null AND  es.PositionID = ${PositionEnum.EMPLOYEE}
        `);
        return result;
    }

    async getInstitutionWithoutSyncedStudents(validFromDate: string) {
        const result = await this.entityManager.query(`
        SELECT DISTINCT inst.InstitutionID FROM core.Person  per 
            LEFT JOIN core.EducationalState es ON es.PersonID = per.PersonID 
            LEFT JOIN core.Institution inst ON inst.InstitutionID = es.InstitutionID 
            LEFT JOIN core.SysUser su ON su.PersonID = per.PersonID  
            WHERE
            1=1
            AND per.ValidFrom >= '${validFromDate}' 
                AND su.SysUserID IS NULL  
                AND es.PositionID IN (${PositionEnum.STUDENT_INSTITUTION}, 
                    ${PositionEnum.STUDENT_OTHER_INSTITUTION}, 
                    ${PositionEnum.STUDENT_PLR}, 
                    ${PositionEnum.UNATTENTDING}, 
                    ${PositionEnum.SPECIAL_STUDENT}) 
                AND per.PersonalID IS NOT NULL 
                AND su.DeletedOn IS NULL 
    `);
        return result;
    }

    async getStudentsForCreation() {
        const students = await this.connection.query(
            `
                EXEC [azure_temp].[DAILY_PROCEDURE_STUDENTS_CREATE];
            `,
        );
        return students;
    }

    async getTeachersForCreation() {
        const teachers = await this.connection.query(
            `
                EXEC [azure_temp].[DAILY_PROCEDURE_TEACHERS_CREATE];
            `,
        );
        return teachers;
    }

    async getCurrentSchoolYear() {
        const result = await this.entityManager.query(
            `
            
            SELECT 
                TOP 1 CurrentYearID as currentYearID
            FROM inst_basic.CurrentYear cy
            WHERE cy.IsValid =1 
            ORDER BY CurrentYearID DESC
            `,
        );
        return result[0].currentYearID;
    }
}
