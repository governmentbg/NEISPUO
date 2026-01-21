/* eslint-disable @typescript-eslint/no-unused-expressions */
import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EnrollmentType } from 'src/common/constants/enum/enrollment-type.enum';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { InProcessing } from 'src/common/constants/enum/in-processing.enum';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { ArchivedResourceQueryDTO } from 'src/common/dto/requests/archived-resource-query.dto';
import { AzureEnrollmentsResponseDTO } from 'src/common/dto/responses/azure-enrollments-response.dto';
import { EnrollmentsArchivedPreviousYearsEntity } from 'src/common/entities/azure-enrollments-archived-previous-years.entity';
import { EnrollmentsEntity } from 'src/common/entities/azure-enrollments.entity';
import { AzureEnrollmentsMapper } from 'src/common/mappers/azure-enrollments.mapper';
import { SchoolYearService } from 'src/common/services/school-year/school-year.service';
import { VariablesService } from 'src/models/variables/routing/variables.service';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class AzureEnrollmentsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async revertAzureEnrollments() {
        const result = await this.entityManager.query(
            `
            UPDATE "azure_temp"."Enrollments" with (
                rowlock,
                updlock,
                readpast
            )
                SET  "InProcessing" = ${InProcessing.NO}
            OUTPUT 
                INSERTED."RowID" AS "rowID"
            WHERE "InProcessing" = ${InProcessing.YES}  AND "UpdatedOn" <= getutcdate() - CAST('00:3' AS datetime)
            `,
        );
    }

    async getAllAzureEnrollments(paging: Paging, filters: any) {
        const result = await this.connection.getRepository(EnrollmentsEntity).findAndCount({
            where: filters,
            skip: paging.from,
            take: paging.numberOfElements,
        });
        return result;
    }

    async getAllAzureEnrollmentsForSending(workflowType: WorkflowType, userRole: UserRoleType) {
        const result = await this.entityManager.query(
            `
            UPDATE 
                enrollments
            SET  		
                InProcessing = ${InProcessing.YES},
                UserAzureID = COALESCE(WORKFLOWS.userAzureID, NULL),
                ClassAzureID = COALESCE(WORKFLOWS.classAzureID, NULL),
                OrganizationAzureID = COALESCE(WORKFLOWS.organizationAzureID, NULL)
            OUTPUT 
                INSERTED."RowID" AS "rowID",
                INSERTED."Status" AS "status",
                INSERTED."WorkflowType" AS "workflowType",
                INSERTED."UserAzureID" AS "userAzureID",
                INSERTED."ClassAzureID" AS "classAzureID",
                INSERTED."OrganizationAzureID" AS "organizationAzureID",
                INSERTED."UserRole" AS "userRole",
                INSERTED."InProcessing" AS "inProcessing",
                INSERTED."ErrorMessage" AS "errorMessage",
                INSERTED."isForArchivation" AS "isForArchivation",
                INSERTED."CreatedOn" AS "createdOn",
                INSERTED."UpdatedOn" AS "updatedOn",
                INSERTED."GUID" AS "guid",
                INSERTED."RetryAttempts" AS "retryAttempts"
            FROM  azure_temp.Enrollments enrollments JOIN (
                SELECT TOP 
                    ${
                        workflowType == WorkflowType.ENROLLMENT_CLASS_CREATE && userRole == UserRoleType.TEACHER
                            ? `${VariablesService.CRON_JOB_TOP_ENROLLMENTS_TEACHER_TO_CLASS_CREATE_VALUE}`
                            : ``
                    }
                    ${
                        workflowType == WorkflowType.ENROLLMENT_CLASS_CREATE && userRole == UserRoleType.STUDENT
                            ? `${VariablesService.CRON_JOB_TOP_ENROLLMENTS_STUDENT_TO_CLASS_CREATE_VALUE}`
                            : ``
                    }
                    ${
                        workflowType == WorkflowType.ENROLLMENT_CLASS_DELETE && userRole == UserRoleType.TEACHER
                            ? `${VariablesService.CRON_JOB_TOP_ENROLLMENTS_TEACHER_TO_CLASS_DELETE_VALUE}`
                            : ``
                    }
                    ${
                        workflowType == WorkflowType.ENROLLMENT_CLASS_DELETE && userRole == UserRoleType.STUDENT
                            ? `${VariablesService.CRON_JOB_TOP_ENROLLMENTS_STUDENT_TO_CLASS_DELETE_VALUE}`
                            : ``
                    }
                    ${
                        workflowType == WorkflowType.ENROLLMENT_SCHOOL_CREATE && userRole == UserRoleType.TEACHER
                            ? `${VariablesService.CRON_JOB_TOP_ENROLLMENTS_TEACHER_TO_SCHOOL_CREATE_VALUE}`
                            : ``
                    } 
                    ${
                        workflowType == WorkflowType.ENROLLMENT_SCHOOL_CREATE && userRole == UserRoleType.STUDENT
                            ? `${VariablesService.CRON_JOB_TOP_ENROLLMENTS_STUDENT_TO_SCHOOL_CREATE_VALUE}`
                            : ``
                    } 
                    ${
                        workflowType == WorkflowType.ENROLLMENT_SCHOOL_DELETE && userRole == UserRoleType.TEACHER
                            ? `${VariablesService.CRON_JOB_TOP_ENROLLMENTS_TEACHER_TO_SCHOOL_DELETE_VALUE}`
                            : ``
                    }  
                    ${
                        workflowType == WorkflowType.ENROLLMENT_SCHOOL_DELETE && userRole == UserRoleType.STUDENT
                            ? `${VariablesService.CRON_JOB_TOP_ENROLLMENTS_STUDENT_TO_SCHOOL_DELETE_VALUE}`
                            : ``
                    } 
                    ${userRole === null ? `${VariablesService.CRON_JOB_TOP_VALUE}` : ``} 
                    "enr"."RowID" AS "rowID",
                    "enr"."Status" AS "status",
                    "enr"."WorkflowType" AS "workflowType",
                    "userP"."AzureID" AS "userAzureID",
                    "cl"."AzureID" AS "classAzureID",
                    "organizationP"."AzureID" AS "organizationAzureID",
                    "enr"."UserRole" AS "userRole",
                    "enr"."InProcessing" AS "inProcessing",
                    "enr"."ErrorMessage" AS "errorMessage",
                    "enr"."CreatedOn" AS "createdOn",
                    "enr"."UpdatedOn" AS "updatedOn",
                    "enr"."GUID" AS "guid",
                    "enr"."isForArchivation" AS "isForArchivation",
                    "enr"."RetryAttempts" AS "retryAttempts"
                FROM
                    "azure_temp"."Enrollments" enr with (
                        rowlock,
                        updlock,
                        readpast
                    )
                    JOIN core.Person userP
                    on userP.PersonID = enr.userPersonID
                    LEFT JOIN core.Person organizationP 
                    on organizationP.PersonID  = enr.organizationPersonID
                    LEFT JOIN inst_year.Curriculum cl
                    on cl.CurriculumID  = enr.curriculumID
                    LEFT JOIN inst_basic.CurrentYear cy ON cl.SchoolYear =cy.CurrentYearID
                WHERE 
                    "enr"."WorkflowType" = ${workflowType} AND
                    ${userRole === null ? `` : `"enr"."UserRole" = '${userRole}' AND`}
                    ${
                        [WorkflowType.ENROLLMENT_CLASS_CREATE, WorkflowType.ENROLLMENT_CLASS_DELETE].includes(
                            workflowType,
                        )
                            ? `cl.IsValid = 1 AND cy.IsValid = 1 AND `
                            : ``
                    }
                    "enr"."Status" IN ( ${EventStatus.AWAITING_CREATION} , ${EventStatus.FAILED_CREATION}) AND
                    "enr"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    (userP.AzureID IS NOT NULL) AND
                    ("enr"."curriculumID" IS NULL OR  ("enr"."curriculumID" IS NOT NULL AND (cl.AzureID IS NOT NULL))) AND
                    ("enr"."organizationPersonID" IS NULL OR ("enr"."organizationPersonID" IS NOT NULL AND (organizationP.AzureID IS NOT NULL))) AND
                    "enr"."isForArchivation" = ${IsForArchivation.NO} AND
                    "enr"."InProcessing" = ${InProcessing.NO} 
                    ) WORKFLOWS ON enrollments.RowID = WORKFLOWS.rowID
                    
            `,
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult;
    }

    getAllAzureEnrollmentsToSchoolForChecking() {
        return this.getAllAzureEnrollmentsForChecking(EnrollmentType.SCHOOL);
    }

    getAllAzureEnrollmentsToClassForChecking() {
        return this.getAllAzureEnrollmentsForChecking(EnrollmentType.CLASS);
    }

    async getAllAzureEnrollmentsForChecking(enrollmentType: EnrollmentType) {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET  "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                    INSERTED."RowID" AS "rowID",
                    INSERTED."UserRole" AS "userRole",
                    INSERTED."UserPersonID" AS "userPersonID",
                    INSERTED."CurriculumID" AS "curriculumID",
                    INSERTED."isForArchivation" AS "isForArchivation",
                    INSERTED."InProgressResultCount" AS "inProgressResultCount",
                    INSERTED."RetryAttempts" AS "retryAttempts",
                    INSERTED."GUID" AS "guid"
            FROM (
                SELECT TOP ${VariablesService.CRON_JOB_TOP_VALUE}
                    "RowID",
                    "UserRole",
                    "UserPersonID",
                    "CurriculumID",
                    "InProcessing",
                    "isForArchivation",
                    "InProgressResultCount",
                    "RetryAttempts",
                    "GUID"
                FROM
                    "azure_temp"."Enrollments" "enr" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    ${
                        enrollmentType === EnrollmentType.SCHOOL
                            ? `enr.WorkflowType IN (${WorkflowType.ENROLLMENT_SCHOOL_CREATE}, ${WorkflowType.ENROLLMENT_SCHOOL_DELETE} ) AND `
                            : `enr.WorkflowType IN (${WorkflowType.ENROLLMENT_CLASS_CREATE}, ${WorkflowType.ENROLLMENT_CLASS_DELETE} ) AND `
                    }
                    ("enr"."Status" = ${EventStatus.IN_CREATION} OR "enr"."Status" = ${EventStatus.FAILED}) AND
                    "enr"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    "enr"."InProgressResultCount" < ${CONSTANTS.JOBS_IN_PROGRESS_RESULT_COUNT_LIMIT} AND
                    "enr"."isForArchivation" = ${IsForArchivation.NO} AND
                    "enr"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult;
    }

    async getAzureEnrollmentStatus(dto: AzureEnrollmentsResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
            SELECT
                RowID AS rowID,
                Status AS status
            FROM
                azure_temp.Enrollments
            WHERE
                RowID = @0
            `,
            [rowID],
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async resendWorkflow(dto: AzureEnrollmentsResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Enrollments
            SET
                UpdatedOn = GETUTCDATE(),
                Status = ${EventStatus.AWAITING_CREATION},
                InProcessing = ${InProcessing.NO}
            OUTPUT 
                INSERTED.RowID AS rowID
            WHERE
                RowID = @0
            `,
            [rowID],
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async setHasCompleted(dtos: AzureEnrollmentsResponseDTO[]) {
        const rowIDs = dtos.map((dto) => dto.rowID).join(`,`);
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Enrollments
            SET
                UpdatedOn = GETUTCDATE(),
                Status = ${EventStatus.SYNCHRONIZED},
                InProcessing = ${InProcessing.NO},
                isForArchivation = ${IsForArchivation.YES}
            OUTPUT 
                INSERTED.RowID as rowID
            WHERE
                RowID IN (${rowIDs})
            `,
            //this here is different from the other entites because in neispuo there is nothing to sync with azure
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async setHasFailed(dtos: AzureEnrollmentsResponseDTO[]) {
        const queryString = `
            UPDATE 
                azure_temp.Enrollments
            SET 
                RetryAttempts = RetryAttempts + 1,
                UpdatedOn = GETUTCDATE(),
                ErrorMessage = dto.ErrorMessage,
                Status = @0,
                isForArchivation = dto.IsForArchivation,
                InProcessing = ${InProcessing.NO}
            OUTPUT 
                INSERTED.RowID as rowID
            FROM (
                VALUES ${dtos
                    .map((dto) => `(${dto.rowID}, N'${dto.errorMessage}', '${dto.isForArchivation}')`)
                    .join(',')}
            ) AS dto(RowID, ErrorMessage, IsForArchivation)
            WHERE 
                Enrollments.RowID = dto.RowID
        `;
        const result = await this.entityManager.query(queryString, [EventStatus.FAILED]);
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async setHasFailedCreation(dtos: AzureEnrollmentsResponseDTO[]) {
        const queryString = `
            UPDATE 
                azure_temp.Enrollments
            SET 
                RetryAttempts = RetryAttempts + 1,
                UpdatedOn = GETUTCDATE(),
                ErrorMessage = dto.ErrorMessage,
                Status = @0,
                isForArchivation = dto.IsForArchivation,
                InProcessing = ${InProcessing.NO}
            OUTPUT 
                INSERTED.RowID as rowID
            FROM (
                VALUES ${dtos
                    .map((dto) => `(${dto.rowID}, N'${dto.errorMessage}', '${dto.isForArchivation}')`)
                    .join(',')}
            ) AS dto(RowID, ErrorMessage, IsForArchivation)
            WHERE 
                Enrollments.RowID = dto.RowID
        `;
        const result = await this.entityManager.query(queryString, [EventStatus.FAILED_CREATION]);
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async setInProcessing(dtos: AzureEnrollmentsResponseDTO[]) {
        const rowIDs = dtos.map((dto) => `(${dto.rowID})`).join(`,`);
        const result = await this.entityManager.query(
            `
            UPDATE e
            SET
                e.UpdatedOn = GETUTCDATE(),
                e.Status = ${EventStatus.IN_CREATION},
                e.InProcessing = ${InProcessing.NO}
            OUTPUT 
                INSERTED.RowID as rowID
            FROM azure_temp.Enrollments e
            INNER JOIN (VALUES ${rowIDs}) AS v(RowID) 
            ON e.RowID = v.RowID
            `,
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async setInProgress(dtos: AzureEnrollmentsResponseDTO[]) {
        const queryString = `
            UPDATE
                azure_temp.Enrollments
            SET
                UpdatedOn = GETUTCDATE(),
                InProgressResultCount = InProgressResultCount + 1,

                Status = CASE WHEN InProgressResultCount >= (${
                    CONSTANTS.JOBS_IN_PROGRESS_RESULT_COUNT_LIMIT
                } - 1) THEN ${EventStatus.STUCK} ELSE Status END,

                isForArchivation = dto.IsForArchivation,
                InProcessing = ${InProcessing.NO}
            OUTPUT 
                INSERTED.RowID as rowID
            FROM (
                VALUES ${dtos.map((dto) => `(${dto.rowID}, '${dto.isForArchivation}')`).join(',')}
            ) AS dto(RowID, IsForArchivation)
            WHERE 
                Enrollments.RowID = dto.RowID
            `;
        const result = await this.entityManager.query(queryString);
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async insertAzureEnrollment(dto: AzureEnrollmentsResponseDTO) {
        const { userPersonID, curriculumID, organizationPersonID, userRole } = dto;
        const result = await this.entityManager.query(
            `           
                INSERT
                INTO
                azure_temp.Enrollments (
                    WorkflowType,
                    UserRole,
                    UserPersonID,
                    CurriculumID,
                    OrganizationPersonID
                )
                OUTPUT 
                    INSERTED.RowID as rowID
                VALUES (@0,@1,@2,@3,@4);
            `,
            [WorkflowType.ENROLLMENT_CREATE, userRole, userPersonID, curriculumID, organizationPersonID],
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async insertAzureClassEnrollment(dto: AzureEnrollmentsResponseDTO, entityManager?: EntityManager) {
        const { userPersonID, curriculumID, userRole } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                INSERT
                INTO
                azure_temp.Enrollments (
                    WorkflowType,
                    UserRole,
                    UserPersonID,
                    CurriculumID
                )
                OUTPUT 
                    INSERTED.RowID as rowID
                VALUES (@0,@1,@2,@3);
            `,
            [WorkflowType.ENROLLMENT_CLASS_CREATE, userRole, userPersonID, curriculumID],
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async insertAzureSchoolEnrollment(dto: AzureEnrollmentsResponseDTO, entityManager?: EntityManager) {
        const { userPersonID, organizationPersonID, userRole } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                INSERT
                INTO
                azure_temp.Enrollments (
                    WorkflowType,
                    UserRole,
                    UserPersonID,
                    OrganizationPersonID
                )
                OUTPUT 
                    INSERTED.RowID as rowID
                VALUES (@0,@1,@2,@3);
            `,
            [WorkflowType.ENROLLMENT_SCHOOL_CREATE, userRole, userPersonID, organizationPersonID],
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async deleteAzureClassEnrollment(dto: AzureEnrollmentsResponseDTO, entityManager: EntityManager) {
        const { userPersonID, curriculumID, userRole } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `                
                INSERT
                INTO
                azure_temp.Enrollments (
                    WorkflowType,
                    UserRole,
                    UserPersonID,
                    CurriculumID
                )
                OUTPUT 
                    INSERTED.RowID as rowID
                VALUES (@0,@1,@2,@3);
            `,
            [WorkflowType.ENROLLMENT_CLASS_DELETE, userRole, userPersonID, curriculumID],
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async deleteAzureSchoolEnrollment(dto: AzureEnrollmentsResponseDTO) {
        const { userPersonID, organizationPersonID, userRole } = dto;
        const result = await this.entityManager.query(
            `                
                INSERT
                INTO
                azure_temp.Enrollments (
                    WorkflowType,
                    UserRole,
                    UserPersonID,
                    OrganizationPersonID
                )
                OUTPUT 
                    INSERTED.RowID as rowID
                VALUES (@0,@1,@2,@3);
            `,
            [WorkflowType.ENROLLMENT_SCHOOL_DELETE, userRole, userPersonID, organizationPersonID],
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async restartFailedWorkflows(dtos: AzureEnrollmentsResponseDTO[]) {
        const rowIDs = dtos.map((dto) => dto.rowID).join(`,`);
        const result = await this.entityManager.query(
            `
            UPDATE 
                azure_temp.Enrollments with (
                    rowlock,
                    updlock,
                    readpast
                )
            SET
                UserAzureID = NULL,
                ClassAzureID = NULL,
                OrganizationAzureID = NULL,
                RetryAttempts = 0,
                Status = 0,
                InProcessing = 0,
                ErrorMessage = NULL
            OUTPUT 
                INSERTED.RowID as rowID
            WHERE
                1=1
                AND RowID IN (${rowIDs})
            `,
        );
        const transformedResult: AzureEnrollmentsResponseDTO[] = AzureEnrollmentsMapper.transform(result);
        return transformedResult[0];
    }

    async archiveNotStartedWorkflows() {
        const result = await this.entityManager.query(
            `
            BEGIN
                BEGIN TRY
                    BEGIN TRANSACTION
                        INSERT
                            INTO
                            "azure_temp"."EnrollmentsArchived"
                        SELECT
                            *
                        FROM
                            "azure_temp"."Enrollments" with (
                                rowlock,
                                updlock,
                                readpast
                            )
                            WHERE 
                                1=1 AND
                                Status = 0 AND
                                RetryAttempts = 0 AND
                                InProcessing = 0 AND
                                CreatedOn < DATEADD(DAY, -30, GETUTCDATE());
                        DELETE FROM "azure_temp"."Enrollments" with (
                            rowlock,
                            updlock,
                            readpast
                        )
                        WHERE 
                            1=1 AND
                            Status = 0 AND
                            RetryAttempts = 0 AND
                            InProcessing = 0 AND
                            CreatedOn < DATEADD(DAY, -30, GETUTCDATE());
                    COMMIT;
                END TRY
                BEGIN CATCH
                ROLLBACK;
                END CATCH
            END;

            `,
        );
    }

    async getArchivedPreviousYears(query: ArchivedResourceQueryDTO) {
        const { identifier, schoolYear } = query;

        let queryBuilder = this.connection.createQueryBuilder(EnrollmentsArchivedPreviousYearsEntity, 'eapy');

        if (schoolYear) {
            const dateInterval = SchoolYearService.mapSchoolYearToDateInterval(schoolYear);
            queryBuilder = queryBuilder.where('eapy.CreatedOn >= :startDate AND eapy.CreatedOn <= :endDate', {
                startDate: dateInterval.startDate,
                endDate: dateInterval.endDate,
            });
            queryBuilder = queryBuilder.andWhere('eapy.userPersonID = :identifier', { identifier });
        } else {
            queryBuilder = queryBuilder.where('eapy.userPersonID = :identifier', { identifier });
        }

        return queryBuilder.getMany();
    }
}
