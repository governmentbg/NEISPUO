import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { InProcessing } from 'src/common/constants/enum/in-processing.enum';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { AzureClassesMapper } from 'src/common/mappers/azure-classes.mapper';
import { VariablesService } from 'src/models/variables/routing/variables.service';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class AzureClassesRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async revertAzureClasses() {
        const result = await this.entityManager.query(
            `
            UPDATE "azure_temp"."Classes" with (
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

    async getAllAzureClassesForSending(workflowType: WorkflowType) {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET  "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                    INSERTED."RowID" AS "rowID",
                    INSERTED."ClassID" AS "classID",
                    INSERTED."WorkflowType" AS "workflowType",
                    INSERTED."Title" AS "title",
                    INSERTED."ClassCode" AS "classCode",
                    INSERTED."OrgID" AS "orgID",
                    INSERTED."TermID" AS "termID",
                    INSERTED."TermName" AS "termName",
                    CAST(INSERTED.TermStartDate AS varchar(10)) AS termStartDate,
                    CAST(INSERTED.TermEndDate AS varchar(10)) AS termEndDate,
                    INSERTED."InProcessing" AS "inProcessing",
                    INSERTED."ErrorMessage" AS "errorMessage",
                    INSERTED."isForArchivation" AS "isForArchivation",
                    INSERTED."CreatedOn" AS "createdOn",
                    INSERTED."UpdatedOn" AS "updatedOn",
                    INSERTED."GUID" AS "guid",
                    INSERTED."AzureID" AS "azureID",
                    INSERTED."RetryAttempts" AS "retryAttempts"
            FROM (
                SELECT TOP 
                ${
                    workflowType == WorkflowType.CLASS_CREATE
                        ? `${VariablesService.CRON_JOB_TOP_CLASSES_CREATE_VALUE}`
                        : ``
                }
                ${
                    workflowType == WorkflowType.CLASS_UPDATE
                        ? `${VariablesService.CRON_JOB_TOP_CLASSES_UPDATE_VALUE}`
                        : ``
                }
                ${
                    workflowType == WorkflowType.CLASS_DELETE
                        ? `${VariablesService.CRON_JOB_TOP_CLASSES_DELETE_VALUE}`
                        : ``
                } 
                *
                FROM
                    "azure_temp"."Classes" "classes" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    "classes"."WorkflowType" = ${workflowType} AND
                    ("classes"."Status" = ${EventStatus.AWAITING_CREATION} OR "classes"."Status" = ${
                EventStatus.FAILED_CREATION
            }) AND
                    "classes"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    "classes"."isForArchivation" = ${IsForArchivation.NO} AND
                    "classes"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult;
    }

    async setHasCompleted(dtos: AzureClassesResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Classes
            SET
                UpdatedOn = GETUTCDATE(),
                Status = @0,
                InProcessing = ${InProcessing.NO},
                AzureID = dto.azureID,
                isForArchivation = ${IsForArchivation.YES}
            OUTPUT 
                INSERTED.RowID as rowID
            FROM (
                VALUES ${dtos.map((dto) => `(${dto.rowID}, '${dto.azureID}')`).join(',')}
            ) AS dto(RowID, AzureID)
            WHERE
                Classes.RowID = dto.RowID
            `,
            [EventStatus.SUCCESSFUL],
        );
    }

    async setHasFailed(dtos: AzureClassesResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Classes
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
                Classes.RowID = dto.RowID
            `,
            [EventStatus.FAILED],
        );
    }

    async setHasFailedCreation(dtos: AzureClassesResponseDTO[]) {
        const queryString = `
        UPDATE
            azure_temp.Classes
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
            VALUES ${dtos.map((dto) => `(${dto.rowID}, N'${dto.errorMessage}', '${dto.isForArchivation}')`).join(',')}
        ) AS dto(RowID, ErrorMessage, IsForArchivation)
        WHERE
            Classes.RowID = dto.RowID
        `;
        const result = await this.entityManager.query(queryString, [EventStatus.FAILED_CREATION]);
    }

    async setHasFailedSyncronization(dtos: AzureClassesResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Classes
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
                Classes.RowID = dto.RowID
            `,
            [EventStatus.FAILED_SYNCRONIZATION],
        );
    }

    async setInProcessing(dtos: AzureClassesResponseDTO[]) {
        const rowIDs = dtos.map((dto) => dto.rowID).join(`,`);
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Classes
            SET
                UpdatedOn = GETUTCDATE(),
                Status = @0,
                InProcessing = ${InProcessing.NO}
            OUTPUT 
                INSERTED."RowID" AS "rowID"
            WHERE
                RowID IN (${rowIDs})
            `,
            [EventStatus.IN_CREATION],
        );
        return AzureClassesMapper.transform(result);
    }

    async setInProgress(dtos: AzureClassesResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Classes
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
                Classes.RowID = dto.RowID
            `,
        );
    }

    async getAllAzureClassesForChecking() {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET  "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                    INSERTED."RowID" AS "rowID",
                    INSERTED."WorkflowType" AS "workflowType",
                    INSERTED."AzureID" AS "azureID",
                    INSERTED."isForArchivation" AS "isForArchivation",
                    INSERTED."InProgressResultCount" AS "inProgressResultCount",
                    INSERTED."RetryAttempts" AS "retryAttempts",
                    INSERTED."GUID" AS "guid"
            FROM (
                SELECT TOP ${VariablesService.CRON_JOB_TOP_VALUE}
                    "RowID",
                    "WorkflowType",
                    "AzureID",
                    "InProcessing",
                    "isForArchivation",
                    "InProgressResultCount",
                    "RetryAttempts",
                    "GUID"
                FROM
                    "azure_temp"."Classes" "cls" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    ("cls"."Status" = ${EventStatus.IN_CREATION} OR "cls"."Status" = ${EventStatus.FAILED}) AND
                    "cls"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    "cls"."InProgressResultCount" < ${CONSTANTS.JOBS_IN_PROGRESS_RESULT_COUNT_LIMIT} AND
                    "cls"."isForArchivation" = ${IsForArchivation.NO} AND
                    "cls"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult;
    }

    async getAzureClassStatus(dto: AzureClassesResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
            SELECT 
                "cls"."status" AS "status"
            FROM
                "azure_temp"."Classes" "cls"
            WHERE
                "cls"."RowID" = @0
            `,
            [rowID],
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult[0];
    }

    async resendWorkflow(dto: AzureClassesResponseDTO) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Classes
            SET
                UpdatedOn = GETUTCDATE(),
                Status = ${EventStatus.AWAITING_CREATION},
                InProcessing = ${InProcessing.NO},
                isForArchivation = @2
            OUTPUT 
                INSERTED.RowID as rowID
            WHERE
                RowID = @0
            `,
            [dto.rowID, dto.isForArchivation],
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult[0];
    }

    async insertAzureClass(dto: AzureClassesResponseDTO, entityManager?: EntityManager) {
        const { title, orgID, termStartDate, termEndDate, classID } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                INSERT
                INTO
                azure_temp.Classes (
                    WorkflowType,
                    Title,
                    OrgID,
                    TermStartDate,
                    TermEndDate,
                    ClassID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5);
            `,
            [WorkflowType.CLASS_CREATE, title, orgID, termStartDate, termEndDate, classID],
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult[0];
    }

    async getAllAzureClassesForCreating() {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                INSERTED."ClassID" AS "classID",
                INSERTED."WorkflowType" AS "workflowType",
                INSERTED."AzureID" AS "azureID",
                INSERTED."isForArchivation" AS "isForArchivation",
                INSERTED."RowID" AS "rowID"
            FROM (
                SELECT TOP ${VariablesService.CRON_JOB_TOP_VALUE}
                    "RowID",
                    "ClassID",
                    "WorkflowType",
                    "AzureID",
                    "InProcessing",
                    "isForArchivation"
                FROM
                    "azure_temp"."Classes" "cls" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    ("cls"."Status" = ${EventStatus.SUCCESSFUL} OR "cls"."Status" = ${EventStatus.FAILED_SYNCRONIZATION}) AND
                    "cls"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND 
                    "cls"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult;
    }

    async updateAzureClass(dto: AzureClassesResponseDTO, entityManager?: EntityManager) {
        const { title, orgID, termStartDate, termEndDate, classID, isForArchivation, azureID } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                INSERT
                INTO
                azure_temp.Classes (
                    WorkflowType,
                    Title,
                    OrgID,
                    TermStartDate,
                    TermEndDate,
                    ClassID,
                    IsForArchivation,
                    AzureID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7);
            `,
            [WorkflowType.CLASS_UPDATE, title, orgID, termStartDate, termEndDate, classID, isForArchivation, azureID],
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult[0];
    }

    async deleteAzureClass(dto: AzureClassesResponseDTO) {
        const { classID, isForArchivation, azureID } = dto;
        const result = await this.entityManager.query(
            `           
            INSERT
                INTO
                azure_temp.Classes (
                    WorkflowType,
                    ClassID,
                    IsForArchivation,
                    AzureID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3);
            `,
            [WorkflowType.CLASS_DELETE, classID, isForArchivation, azureID],
        );
        const transformedResult: AzureClassesResponseDTO[] = AzureClassesMapper.transform(result);
        return transformedResult[0];
    }

    async restartFailedWorkflows(dtos: AzureClassesResponseDTO[]) {
        const rowIDs = dtos.map((dto) => dto.rowID).join(`,`);
        const result = await this.entityManager.query(
            `
            UPDATE 
                azure_temp.Classes with (
                    rowlock,
                    updlock,
                    readpast
                )
            SET
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
        return AzureClassesMapper.transform(result);
    }
}
