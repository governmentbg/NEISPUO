/* eslint-disable prettier/prettier */
import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { InProcessing } from 'src/common/constants/enum/in-processing.enum';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { Paging } from 'src/common/dto/paging.dto';
import { ArchivedResourceQueryDTO } from 'src/common/dto/requests/archived-resource-query.dto';
import { AzureOrganizationsResponseDTO } from 'src/common/dto/responses/azure-organizations-response.dto';
import { OrganizationsArchivedPreviousYearsEntity } from 'src/common/entities/azure-organizations-archived-previous-years.entity';
import { AzureOrganizationsEntity } from 'src/common/entities/azure-organizations.entity';
import { AzureOrganizationsMapper } from 'src/common/mappers/azure-organizations.mapper';
import { SchoolYearService } from 'src/common/services/school-year/school-year.service';
import { VariablesService } from 'src/models/variables/routing/variables.service';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class AzureOrganizationsRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async revertAzureOrganizations() {
        const result = await this.entityManager.query(
            `
            UPDATE "azure_temp"."Organizations" with (
                rowlock,
                updlock,
                readpast
            )
                SET  "InProcessing" = ${InProcessing.NO}
            OUTPUT 
                INSERTED."RowID" AS "rowID"
            WHERE "InProcessing" = ${InProcessing.YES}  AND "UpdatedOn" <= GETUTCDATE() - CAST('00:3' AS datetime)
            `,
        );
    }

    async getAllAzureOrganizations(paging: Paging, filters: any) {
        const result = await this.connection.getRepository(AzureOrganizationsEntity).findAndCount({
            where: filters,
            skip: paging.from,
            take: paging.numberOfElements,
        });
        return result;
    }

    async getAllAzureOrganizationsForSending(workflowType: WorkflowType) {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET  "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                    INSERTED."RowID" AS "rowID",
                    INSERTED."OrganizationID" AS "organizationID",
                    INSERTED."WorkflowType" AS "workflowType",
                    INSERTED."Name" AS "name",
                    INSERTED."Description" AS "description",
                    INSERTED."PrincipalId" AS "principalId",
                    INSERTED."PrincipalName" AS "principalName",
                    INSERTED."PrincipalEmail" AS "principalEmail",
                    INSERTED."HighestGrade" AS "highestGrade",
                    INSERTED."LowestGrade" AS "lowestGrade",
                    INSERTED."Phone" AS "phone",
                    INSERTED."City" AS "city",
                    INSERTED."Area" AS "area",
                    INSERTED."Country" AS "country",
                    INSERTED."PostalCode" AS "postalCode",
                    INSERTED."Street" AS "street",
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
                    workflowType == WorkflowType.SCHOOL_CREATE
                        ? `${VariablesService.CRON_JOB_TOP_ORGANIZATIONS_CREATE_VALUE}`
                        : ``
                }
                ${
                    workflowType == WorkflowType.SCHOOL_UPDATE
                        ? `${VariablesService.CRON_JOB_TOP_ORGANIZATIONS_UPDATE_VALUE}`
                        : ``
                }
                ${
                    workflowType == WorkflowType.SCHOOL_DELETE
                        ? `${VariablesService.CRON_JOB_TOP_ORGANIZATIONS_DELETE_VALUE}`
                        : ``
                } 
                 *
                FROM
                    "azure_temp"."Organizations" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    "WorkflowType" = ${workflowType} AND
                    ("Status" = ${EventStatus.AWAITING_CREATION} OR "Status" = ${EventStatus.FAILED_CREATION}) AND
                    "RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    isForArchivation = ${IsForArchivation.NO} AND
                    InProcessing = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult;
    }

    async getAllAzureOrganizationsForChecking() {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET  "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                    INSERTED."RowID" AS "rowID",
                    INSERTED."GUID" AS "guid",
                    INSERTED."WorkflowType" AS "workflowType",
                    INSERTED."OrganizationID" AS "organizationID",
                    INSERTED."Username" AS "username",
                    INSERTED."Name" AS "name",
                    INSERTED."AzureID" AS "azureID",
                    INSERTED."isForArchivation" AS "isForArchivation",
                    INSERTED."InProgressResultCount" AS "inProgressResultCount",
                    INSERTED."RetryAttempts" AS "retryAttempts",
                    INSERTED."Description" AS "description"
            FROM (
                SELECT TOP ${VariablesService.CRON_JOB_TOP_VALUE} 
                    "RowID",
                    "GUID",
                    "WorkflowType",
                    "OrganizationID",
                    "Username",
                    "Name",
                    "AzureID",
                    "isForArchivation",
                    "InProgressResultCount",
                    "RetryAttempts",
                    "InProcessing",
                    "Description"
                FROM
                    "azure_temp"."Organizations" "org" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    ("org"."Status" = ${EventStatus.IN_CREATION} OR "org"."Status" = ${EventStatus.FAILED}) AND
                    "org"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    "org"."InProgressResultCount" < ${CONSTANTS.JOBS_IN_PROGRESS_RESULT_COUNT_LIMIT} AND
                    "org"."isForArchivation" = ${IsForArchivation.NO} AND
                    "org"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult;
    }

    async getAllAzureOrganizationsForCreating() {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                    INSERTED."RowID" AS "rowID",
                    INSERTED."OrganizationID" AS "organizationID",
                    INSERTED."Username" AS "username",
                    INSERTED."Name" AS "name",
                    INSERTED."Password" AS "password",
                    INSERTED."PrincipalName" AS "principalName",
                    INSERTED."IsForArchivation" AS "isForArchivation",
                    INSERTED."AzureID" AS "azureID",
                    INSERTED."WorkflowType" AS "workflowType"
            FROM (
                SELECT TOP ${VariablesService.CRON_JOB_TOP_VALUE}
                    "RowID",
                    "OrganizationID",
                    "Username",
                    "Name",
                    "Password",
                    "PrincipalName",
                    "isForArchivation",
                    "InProcessing",
                    "AzureID",
                    "WorkflowType"
                FROM
                    "azure_temp"."Organizations" "org" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    ("org"."Status" = ${EventStatus.SUCCESSFUL} OR "org"."Status" = ${EventStatus.FAILED_SYNCRONIZATION}) AND
                    "org"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND 
                    "org"."isForArchivation" = ${IsForArchivation.NO} AND
                    "org"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult;
    }

    async getAzureOrganizationStatus(dto: AzureOrganizationsResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
            SELECT
                RowID AS rowID,
                Status AS status
            FROM
                azure_temp.Organizations
            WHERE
                RowID = @0
            `,
            [rowID],
        );
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult[0];
    }

    async resendWorkflow(dto: AzureOrganizationsResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Organizations
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
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult[0];
    }

    async setHasCompleted(dtos: AzureOrganizationsResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Organizations
            SET
                UpdatedOn = GETUTCDATE(),
                Status = @0,
                Username = dto.Username,
                Password = dto.Password,
                InProcessing = ${InProcessing.NO},
                AzureID = dto.AzureID,
                isForArchivation = dto.isForArchivation
            OUTPUT 
                INSERTED.RowID as rowID
            FROM (
                VALUES ${dtos
                    .map(
                        (dto) =>
                            `(${dto.rowID}, '${dto.username}', '${dto.password}', '${dto.azureID}', '${dto.isForArchivation}')`,
                    )
                    .join(',')}
            ) AS dto(RowID, Username, Password, AzureID, isForArchivation)
            WHERE
                Organizations.RowID = dto.RowID
            `,
            [EventStatus.SUCCESSFUL],
        );
        return AzureOrganizationsMapper.transform(result);
    }

    async setHasFailed(dtos: AzureOrganizationsResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Organizations
            SET
                RetryAttempts = RetryAttempts + 1,
                UpdatedOn = GETUTCDATE(),
                ErrorMessage = dto.ErrorMessage,
                Status = @0,
                isForArchivation = dto.isForArchivation,
                InProcessing = ${InProcessing.NO}
            OUTPUT 
                INSERTED.RowID as rowID
            FROM (
                VALUES ${dtos
                    .map((dto) => `(${dto.rowID}, N'${dto.errorMessage}', '${dto.isForArchivation}')`)
                    .join(',')}
            ) AS dto(RowID, ErrorMessage, IsForArchivation)
            WHERE
                Organizations.RowID = dto.RowID
            `,
            [EventStatus.FAILED],
        );
        return AzureOrganizationsMapper.transform(result);
    }

    async setHasFailedCreation(dtos: AzureOrganizationsResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Organizations
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
            Organizations.RowID = dto.RowID
            `,
            [EventStatus.FAILED_CREATION],
        );
        return AzureOrganizationsMapper.transform(result);
    }

    async setHasFailedSyncronization(dtos: AzureOrganizationsResponseDTO[]) {
        const result = await getManager().query(
            `
        UPDATE
            azure_temp.Organizations
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
            Organizations.RowID = dto.RowID
        `,
            [EventStatus.FAILED_SYNCRONIZATION],
        );
        return AzureOrganizationsMapper.transform(result);
    }

    async setInProcessing(dtos: AzureOrganizationsResponseDTO[]) {
        const rowIDs = dtos.map((dto) => dto.rowID).join(`,`);
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Organizations
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
        return AzureOrganizationsMapper.transform(result);
    }

    async setInProgress(dtos: AzureOrganizationsResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Organizations
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
                Organizations.RowID = dto.RowID
            `,
        );
        return AzureOrganizationsMapper.transform(result);
    }

    async insertAzureOrganization(dto: AzureOrganizationsResponseDTO, entityManager?: EntityManager) {
        const {
            name,
            description,
            principalName,
            principalEmail,
            highestGrade,
            lowestGrade,
            phone,
            city,
            area,
            country,
            postalCode,
            street,
            organizationID,
            azureID,
        } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                INSERT
                INTO
                azure_temp.Organizations (
                    WorkflowType,
                    Name,
                    Description,
                    PrincipalName,
                    PrincipalEmail,
                    HighestGrade,
                    LowestGrade,
                    Phone,
                    City,
                    Area,
                    Country,
                    PostalCode,
                    Street,
                    OrganizationID,
                    AzureID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14);
            `,
            [
                WorkflowType.SCHOOL_CREATE,
                name,
                description,
                principalName,
                principalEmail,
                highestGrade,
                lowestGrade,
                phone,
                city,
                area,
                country,
                postalCode,
                street,
                organizationID,
                azureID,
            ],
        );
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult[0];
    }

    async updateAzureOrganization(dto: AzureOrganizationsResponseDTO) {
        const {
            sysUserID,
            name,
            description,
            principalName,
            principalEmail,
            highestGrade,
            lowestGrade,
            phone,
            city,
            area,
            country,
            postalCode,
            street,
            organizationID,
            personID,
            azureID,
        } = dto;
        let result = false;
        try {
            await this.connection.transaction(async (manager) => {
                const insertedAzureOrganization = await manager.query(
                    `
                    INSERT
                    INTO
                    azure_temp.Organizations (
                        WorkflowType,
                        Name,
                        Description,
                        PrincipalName,
                        PrincipalEmail,
                        HighestGrade,
                        LowestGrade,
                        Phone,
                        City,
                        Area,
                        Country,
                        PostalCode,
                        Street,
                        OrganizationID,
                        PersonID,
                        AzureID
                    )
                    OUTPUT Inserted.RowID as rowID
                    VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15);
                    `,
                    [
                        WorkflowType.SCHOOL_UPDATE,
                        name,
                        description,
                        principalName,
                        principalEmail,
                        highestGrade,
                        lowestGrade,
                        phone,
                        city,
                        area,
                        country,
                        postalCode,
                        street,
                        organizationID,
                        personID,
                        azureID,
                    ],
                );
                const transformedOrganizationResult: AzureOrganizationsResponseDTO[] =
                    AzureOrganizationsMapper.transform(insertedAzureOrganization);
                const insertedAzureOrganizationRowID = transformedOrganizationResult[0]?.rowID;
                dto.rowID = insertedAzureOrganizationRowID;
                if (insertedAzureOrganizationRowID) result = true;
            });
        } catch (error) {}
        return result;
    }

    async deleteAzureOrganization(dto: AzureOrganizationsResponseDTO) {
        const { organizationID, azureID, personID } = dto;
        const result = await this.entityManager.query(
            `           
                INSERT
                INTO
                azure_temp.Organizations (
                    WorkflowType,
                    OrganizationID,
                    PersonID,
                    AzureID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3);
            `,
            [WorkflowType.SCHOOL_DELETE, organizationID, personID, azureID],
        );
        const transformedResult: AzureOrganizationsResponseDTO[] = AzureOrganizationsMapper.transform(result);
        return transformedResult[0];
    }

    async restartFailedWorkflows(dtos: AzureOrganizationsResponseDTO[]) {
        const rowIDs = dtos.map((dto) => dto.rowID).join(`,`);
        const result = await this.entityManager.query(
            `
                UPDATE 
                    azure_temp.Organizations
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
    }

    async getArchivedPreviousYears(query: ArchivedResourceQueryDTO) {
        const { identifier, schoolYear } = query;

        let queryBuilder = this.connection.createQueryBuilder(OrganizationsArchivedPreviousYearsEntity, 'oapy');

        if (schoolYear) {
            const dateInterval = SchoolYearService.mapSchoolYearToDateInterval(schoolYear);
            queryBuilder = queryBuilder.where('oapy.CreatedOn >= :startDate AND oapy.CreatedOn <= :endDate', {
                startDate: dateInterval.startDate,
                endDate: dateInterval.endDate,
            });
            queryBuilder = queryBuilder.andWhere('oapy.OrganizationID = :identifier', { identifier });
        } else {
            queryBuilder = queryBuilder.where('oapy.OrganizationID = :identifier', { identifier });
        }

        return queryBuilder.getMany();
    }
}
