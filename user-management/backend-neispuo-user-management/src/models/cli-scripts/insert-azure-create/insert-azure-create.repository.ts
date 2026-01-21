import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { AzureUsersMapper } from 'src/common/mappers/azure-users.mapper';
import { PersonMapper } from 'src/common/mappers/person.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class InsertAzureCreateRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getAllAzureAccountsWithNoCreateWorkflow(paging: Paging) {
        const result = await getManager().query(
            `
            SELECT
                DISTINCT p.PersonID as personID
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            left join core.SysUser su on
                su.PersonID = p.PersonID
            LEFT JOIN
        
            (
                Select
                    u.UserID,
                    u.WorkflowType
                from
                    azure_temp.Users u 
                group by
                    u.UserID ,
                    u.WorkflowType 
                            ) t2 ON t2.UserID = p.PersonalID AND t2.WorkflowType = WorkflowType
            WHERE
                1=1
                and p.AzureID IS NOT NULL
                and pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and p.PersonalID is not NULL
                and t2.UserID IS NULL AND t2.WorkflowType IS NULL
            ORDER BY
                PersonID ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
            `,
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult;
    }

    async getAllAzureAccountsWithNoCreateWorkflowCount() {
        const result = await getManager().query(
            `
            SELECT
                count(DISTINCT p.PersonID) as count
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            left join core.SysUser su on
                su.PersonID = p.PersonID
            LEFT JOIN
        
            (
                Select
                    u.UserID,
                    u.WorkflowType
                from
                    azure_temp.Users u 
                group by
                    u.UserID ,
                    u.WorkflowType 
                            ) t2 ON t2.UserID = p.PersonalID AND t2.WorkflowType = WorkflowType
            WHERE
                1=1
                and p.AzureID IS NOT NULL
                and pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and p.PersonalID is not NULL
                and t2.UserID IS NULL AND t2.WorkflowType IS NULL
            `,
        );
        return result[0].count;
    }

    async insertAzureCreate(dto: AzureUsersResponseDTO) {
        const {
            identifier,
            firstName,
            middleName,
            surname,
            grade,
            schoolId,
            birthDate,
            userRole,
            accountEnabled,
            userID,
            additionalRole,
            sisAccessSecondaryRole,
            hasNeispuoAccess,
            personID,
            azureID,
        } = dto;
        const result = await getManager().query(
            `         
                INSERT
                INTO
                azure_temp.Users (
                    WorkflowType,
                    Identifier,
                    FirstName,
                    MiddleName,
                    Surname,
                    Grade,
                    SchoolId,
                    BirthDate,
                    UserRole,
                    AccountEnabled,
                    UserID,
                    AdditionalRole,
                    SisAccessSecondaryRole,
                    HasNeispuoAccess,
                    PersonID,
                    AzureID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15);
            `,
            [
                WorkflowType.USER_CREATE,
                identifier,
                firstName,
                middleName,
                surname,
                grade,
                schoolId,
                birthDate,
                userRole,
                accountEnabled,
                userID,
                additionalRole,
                sisAccessSecondaryRole,
                hasNeispuoAccess,
                personID,
                azureID,
            ],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async updateAzureUser(dto: AzureUsersResponseDTO) {
        const { rowID } = dto;
        const result = await getManager().query(
            `   UPDATE
                    azure_temp.Users 
                SET
                    Status = ${EventStatus.SYNCHRONIZED},
                    ErrorMessage = ''
                OUTPUT Inserted.RowID as rowID
                WHERE RowID = @0
            `,
            [rowID],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async fillUsersAzureIDs() {
        const result = await getManager().query(
            `
            UPDATE 
                u
            SET
                u.AzureID = (SELECT p.AzureID from core.Person p WHERE p.PersonalID = u.UserID)
            OUTPUT INSERTED.*
            FROM  azure_temp.Users u
            WHERE 
            u.AzureID IS NULL
            `,
        );
    }

    async fillUsersPersonIDs() {
        const result = await getManager().query(
            `
            UPDATE 
                u
            SET
                u.PersonID = (SELECT p.PersonID from core.Person p WHERE p.PersonalID = u.UserID)
            OUTPUT INSERTED.*
            FROM  azure_temp.Users u
            WHERE 
            u.PersonID IS NULL
            `,
        );
    }

    async fillClassesAzureIDs() {
        const result = await getManager().query(
            `
            UPDATE 
                c
            SET
                c.AzureID = (SELECT cur.AzureID from inst_year.Curriculum cur WHERE cur.CurriculumID = c.ClassID)
            OUTPUT INSERTED.*
            FROM  azure_temp.Classes c
            WHERE 
            c.AzureID IS NULL
            `,
        );
    }

    async fillOrganizationsAzureIDs() {
        const result = await getManager().query(
            `
            UPDATE 
                o
            SET
                o.AzureID = (Select p.AzureID from core.Institution i JOIN
            core.SysUserSysRole susr ON i.InstitutionID = susr.InstitutionID JOIN 
            core.SysUser su ON su.SysUserID = susr.SysUserID JOIN 
            core.Person p ON p.PersonID = su.PersonID
            WHERE
                1=1 
                AND p.FirstName='Няма' 
                AND p.MiddleName ='Няма' 
                AND p.LastName ='Няма'
                AND i.InstitutionID = o.OrganizationID 
            )
            OUTPUT INSERTED.*
            FROM  azure_temp.Organizations o
            WHERE 
            o.AzureID IS NULL
            `,
        );
    }

    async linkEnrollmentsWithOtherEntities() {
        const result = await getManager().query(
            `
            UPDATE e
                SET 
                    e.UserRowID = CASE 
                        WHEN e.UserRowID IS NULL THEN 
                            (SELECT TOP 1
                                RowID as rowID
                            FROM
                                azure_temp.Users u
                            WHERE
                                1 = 1
                                AND u.UserID = e.UserAzureID
                                AND u.WorkflowType = '${WorkflowType.USER_CREATE}'
                                AND u.Status <> ${EventStatus.IN_USERNAME_GENERATION} 
                                AND u.Status <> ${EventStatus.FAILED_USERNAME_GENERATION})
                        WHEN e.UserRowID IS NOT NULL THEN e.UserRowID
                    END,
                    e.ClassRowID = CASE 
                        WHEN e.ClassRowID IS NULL THEN 
                            (SELECT TOP 1
                                RowID as rowID
                            FROM
                                azure_temp.Classes c
                            WHERE
                                1 = 1
                                AND c.ClassID = e.ClassAzureID
                                AND c.WorkflowType = '${WorkflowType.CLASS_CREATE}')
                        WHEN e.ClassRowID IS NOT NULL THEN e.ClassRowID
                    END,
                    e.OrganizationRowID = CASE 
                        WHEN e.OrganizationRowID IS NULL THEN 
                            (SELECT TOP 1
                                RowID as rowID
                            FROM
                                azure_temp.Organizations o
                            WHERE
                                1 = 1
                                AND o.OrganizationID = e.OrganizationAzureID
                                AND o.WorkflowType = '${WorkflowType.SCHOOL_CREATE}')
                        WHEN e.OrganizationRowID IS NOT NULL THEN e.OrganizationRowID
                    END,
                    e.RetryAttempts = CASE 
                        WHEN e.RetryAttempts >= ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} THEN 
                            0
                            ELSE e.RetryAttempts
                    END
                FROM 
                    azure_temp.Enrollments e
            
            `,
            undefined,
        );
    }
}
