/* eslint-disable prettier/prettier */
import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { InProcessing } from 'src/common/constants/enum/in-processing.enum';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { ArchivedResourceQueryDTO } from 'src/common/dto/requests/archived-resource-query.dto';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { UsersArchivedPreviousYearsEntity } from 'src/common/entities/azure-users-archived-previous-years.entity';
import { AzureUsersMapper } from 'src/common/mappers/azure-users.mapper';
import { SchoolYearService } from 'src/common/services/school-year/school-year.service';
import { VariablesService } from 'src/models/variables/routing/variables.service';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class AzureUsersRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async revertAzureUsers() {
        const result = await this.entityManager.query(
            `
            UPDATE "azure_temp"."Users" with (
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

    async getAllAzureUsersForSending(workflowType: WorkflowType) {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                    INSERTED."RowID" AS "rowID",
                    INSERTED."UserID" AS "userID",
                    INSERTED."WorkflowType" AS "workflowType",
                    INSERTED."Identifier" AS "identifier",
                    INSERTED."FirstName" AS "firstName",
                    INSERTED."MiddleName" AS "middleName",
                    INSERTED."Surname" AS "surname",
                    INSERTED."Password" AS "password",
                    INSERTED."Email" AS "email",
                    INSERTED."Phone" AS "phone",
                    INSERTED."Grade" AS "grade",
                    INSERTED."SchoolId" AS "schoolId",
                    INSERTED."BirthDate" AS "birthDate",
                    INSERTED."UserRole" AS "userRole",
                    INSERTED."AccountEnabled" AS "accountEnabled",
                    INSERTED."ErrorMessage" AS "errorMessage",
                    INSERTED."isForArchivation" AS "isForArchivation",
                    INSERTED."CreatedOn" AS "createdOn",
                    INSERTED."UpdatedOn" AS "updatedOn",
                    INSERTED."GUID" AS "guid",
                    INSERTED."RetryAttempts" AS "retryAttempts",
                    INSERTED."AdditionalRole" AS "additionalRole",
                    INSERTED."SisAccessSecondaryRole" AS "sisAccessSecondaryRole",
                    INSERTED."HasNeispuoAccess" AS "hasNeispuoAccess",
                    INSERTED."AssignedAccountantSchools" AS "assignedAccountantSchools",
                    INSERTED."Username" AS "username",
                    INSERTED."AzureID" AS "azureID",
                    INSERTED."PersonID" AS "personID"
            FROM (
                SELECT TOP 
                ${workflowType == WorkflowType.USER_CREATE ? `${VariablesService.CRON_JOB_TOP_USERS_CREATE_VALUE}` : ``}
                ${workflowType == WorkflowType.USER_UPDATE ? `${VariablesService.CRON_JOB_TOP_USERS_UPDATE_VALUE}` : ``}
                ${
                    workflowType == WorkflowType.USER_DELETE
                        ? `${VariablesService.CRON_JOB_TOP_USERS_DELETE_VALUE}`
                        : ``
                } 
                 *
                FROM
                    "azure_temp"."Users" "usr" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    "usr"."WorkflowType" = ${workflowType} AND
                    ("usr"."Status" = ${EventStatus.AWAITING_CREATION} OR "usr"."Status" = ${
                EventStatus.FAILED_CREATION
            }) AND
                    "usr"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    "usr"."isForArchivation" = ${IsForArchivation.NO} AND
                    "usr"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult;
    }

    async getAllAzureUsersForCreating() {
        const result = await this.entityManager.query(
            `
            UPDATE WORKFLOWS
                SET "InProcessing" = ${InProcessing.YES}
                OUTPUT 
                    INSERTED."RowID" AS "rowID",
                    INSERTED."UserID" AS "userID",
                    INSERTED."Username" AS "username",
                    INSERTED."Password" AS "password",
                    INSERTED."WorkflowType" AS "workflowType",
                    INSERTED."UserRole" AS "userRole",
                    INSERTED."AdditionalRole" AS "additionalRole",
                    INSERTED."SisAccessSecondaryRole" AS "sisAccessSecondaryRole",
                    INSERTED."FirstName" AS "firstName",
                    INSERTED."MiddleName" AS "middleName",
                    INSERTED."Surname" AS "surname",
                    INSERTED."PersonID" AS "personID",
                    INSERTED."AzureID" AS "azureID",
                    INSERTED."IsForArchivation" AS "isForArchivation",
                    INSERTED."InProgressResultCount" AS "inProgressResultCount",
                    INSERTED."RetryAttempts" AS "retryAttempts",
                    INSERTED."Email" AS "email"
            FROM (
                SELECT TOP ${VariablesService.CRON_JOB_TOP_VALUE} 
                    "RowID",
                    "UserID",
                    "Username",
                    "Password",
                    "WorkflowType",
                    "UserRole",
                    "AdditionalRole",
                    "sisAccessSecondaryRole",
                    "InProcessing",
                    "FirstName",
                    "MiddleName",
                    "Surname",
                    "PersonID",
                    "AzureID",
                    "IsForArchivation",
                    "InProgressResultCount",
                    "RetryAttempts",
                    "Email"
                FROM
                    "azure_temp"."Users" "usr" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    ("usr"."Status" = ${EventStatus.SUCCESSFUL} OR "usr"."Status" = ${EventStatus.FAILED_SYNCRONIZATION}) AND
                    "usr"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    "usr"."isForArchivation" = ${IsForArchivation.NO} AND
                    "usr"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult;
    }

    async getAzureUserStatus(dto: AzureUsersResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
            SELECT
                "usr"."status" AS "status"
            FROM
                "azure_temp"."Users" "usr"
            WHERE
                "usr"."RowID" = @0
            `,
            [rowID],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async resendWorkflow(dto: AzureUsersResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Users
            SET
                UpdatedOn = GETUTCDATE(),
                Status = ${EventStatus.AWAITING_CREATION}, 
                InProcessing = ${InProcessing.NO}
            OUTPUT 
                INSERTED."RowID" AS "rowID"
            WHERE
                RowID = @0
            `,
            [rowID],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async stopWorkflow(dto: AzureUsersResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Users
            SET
                RetryAttempts = 5,
                UpdatedOn = GETUTCDATE(),
                Status = ${EventStatus.FAILED}
            OUTPUT 
                INSERTED."RowID" AS "rowID"
            WHERE
                RowID = @0
            `,
            [rowID],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async setHasCompleted(dtos: AzureUsersResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            
            UPDATE
                azure_temp.Users
            SET
                UpdatedOn = GETUTCDATE(),
                Status = @0,
                Username = dto.Username,
                Password = CASE WHEN dto.UserRole = '${UserRoleType.PARENT}' THEN Users.Password ELSE dto.Password END,
                InProcessing = ${InProcessing.NO},
                AzureID = dto.AzureID,
                isForArchivation = dto.isForArchivation
            OUTPUT 
                INSERTED.RowID as rowID
            FROM (
                VALUES ${dtos
                    .map(
                        (dto) =>
                            `(${dto.rowID}, '${dto.userRole}', ${dto.username ? `'${dto.username}'` : 'NULL'}, ${
                                dto.password ? `'${dto.password}'` : 'NULL'
                            }, '${dto.azureID}', '${dto.isForArchivation}')`,
                    )
                    .join(',')}
            ) AS dto(RowID, UserRole, Username, Password, AzureID, isForArchivation)
            WHERE
                Users.RowID = dto.RowID
            `,
            [EventStatus.SUCCESSFUL],
        );
        return AzureUsersMapper.transform(result);
    }

    async setHasFailed(dtos: AzureUsersResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Users
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
                Users.RowID = dto.RowID
            `,
            [EventStatus.FAILED],
        );
        return AzureUsersMapper.transform(result);
    }

    async setHasFailedSyncronization(dtos: AzureUsersResponseDTO[]) {
        const result = await getManager().query(
            `
        UPDATE
            azure_temp.Users
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
            VALUES ${dtos.map((dto) => `(${dto.rowID}, N'${dto.errorMessage}', '${dto.isForArchivation}')`).join(',')}
        ) AS dto(RowID, ErrorMessage, IsForArchivation)
        WHERE
            Users.RowID = dto.RowID
        `,
            [EventStatus.FAILED_SYNCRONIZATION],
        );
        return AzureUsersMapper.transform(result);
    }

    async setHasFailedCreation(dtos: AzureUsersResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Users
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
                Users.RowID = dto.RowID
            `,
            [EventStatus.FAILED_CREATION],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async setInProcessing(dtos: AzureUsersResponseDTO[]) {
        const rowIDs = dtos.map((dto) => dto.rowID).join(`,`);
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Users
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
        return AzureUsersMapper.transform(result);
    }

    async setInProgress(dtos: AzureUsersResponseDTO[]) {
        const result = await this.entityManager.query(
            `
            UPDATE
                azure_temp.Users
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
            Users.RowID = dto.RowID
            `,
        );
        return AzureUsersMapper.transform(result);
    }

    async setSyncronized(dto: AzureUsersResponseDTO) {
        const { rowID } = dto;
        const result = await this.entityManager.query(
            `
              UPDATE
                  azure_temp.Users
              SET
                  UpdatedOn = GETUTCDATE(),
                  Status = @1,
                  InProcessing = ${InProcessing.NO}
              OUTPUT 
                  INSERTED."RowID" AS "rowID"
              WHERE
                  RowID = @0
              `,
            [rowID, EventStatus.SYNCHRONIZED],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async getAllAzureUsersForChecking() {
        const result = await this.entityManager.query(
            `
            UPDATE 
                WORKFLOWS
            SET "InProcessing" = ${InProcessing.YES}
            OUTPUT 
                INSERTED."RowID" AS "rowID",
                INSERTED."WorkflowType" AS "workflowType",
                INSERTED."GUID" AS "guid",
                INSERTED."AzureID" AS "azureID",
                INSERTED."UserRole" AS "userRole",
                INSERTED."isForArchivation" AS "isForArchivation",
                INSERTED."InProgressResultCount" AS "inProgressResultCount",
                INSERTED."RetryAttempts" AS "retryAttempts",
                INSERTED."PersonID" AS "personID"
            FROM (
                SELECT TOP ${VariablesService.CRON_JOB_TOP_VALUE} 
                    
                    "RowID",
                    "InProcessing",
                    "WorkflowType",
                    "GUID",
                    "AzureID",
                    "UserRole",
                    "isForArchivation",
                    "InProgressResultCount",
                    "RetryAttempts",
                    "PersonID"
                FROM
                    "azure_temp"."Users" "usr" with (
                        rowlock,
                        updlock,
                        readpast
                    )
                WHERE
                    ("usr"."Status" = ${EventStatus.IN_CREATION} OR "usr"."Status" = ${EventStatus.FAILED}) AND
                    "usr"."RetryAttempts" < ${CONSTANTS.JOBS_RETRY_ATTEMPTS_LIMIT} AND
                    "usr"."InProgressResultCount" < ${CONSTANTS.JOBS_IN_PROGRESS_RESULT_COUNT_LIMIT} AND
                    "usr"."isForArchivation" = ${IsForArchivation.NO} AND
                    "usr"."InProcessing" = ${InProcessing.NO}
            ) WORKFLOWS
            `,
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult;
    }

    async generateUsername(dto: AzureUsersResponseDTO, entityManager?: EntityManager) {
        const { rowID, firstName, middleName, lastName, email, userRole, personID } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `
            BEGIN
                --declare variables 
                DECLARE @RowID INT = @0;
                DECLARE @FirstName VARCHAR(100) = @1;
                DECLARE @MiddleName VARCHAR(100) = @2;
                DECLARE @Surname VARCHAR(100) = @3;
                DECLARE @Email VARCHAR(100) = @4;
                DECLARE @UserRole VARCHAR(100) = @5;
                DECLARE @PersonID INT = @6
                DECLARE @WorkflowType VARCHAR(100) = @7;
                
                BEGIN
                    
                    SET NOCOUNT ON
                    DECLARE @RETURNVALUE int;
                    DECLARE @RESULTOUT varchar(100) = '';
                    DECLARE @ERRORSOUT varchar(100) = '';
                
                    IF @WorkflowType = 'USER_CREATE'
                        BEGIN
                            --call generate function
                            SELECT @FirstName = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@FirstName);
                            SELECT @MiddleName = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@MiddleName);
                            SELECT @Surname = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@Surname);
                            EXEC @RETURNVALUE = azure_temp.GENERATE_USERNAME 
                                @FirstName = @FirstName,
                                @MiddleName = @MiddleName,
                                @LastName = @Surname, 
                                @Email = @Email, 
                                @AzureUserRole = @UserRole, 
                                @PersonID = @PersonID, 
                                @ERRORS = @ERRORSOUT OUTPUT,
                                @RESULT = @RESULTOUT OUTPUT;
                            IF LEN(@ERRORSOUT) > 0
                                BEGIN
                                    --this will set the status to error and set the error message
                                    UPDATE
                                        azure_temp.Users
                                    SET
                                        Status = ${EventStatus.FAILED_USERNAME_GENERATION},
                                        ErrorMessage = @ERRORSOUT
                                    OUTPUT 
                                        INSERTED.Status as status
                                    WHERE RowID = @RowID;
                                END
                            ELSE
                                BEGIN
                                    --if all is good the username should be generated and the status should be set to 0 so the create job can pick up the row.
                                    UPDATE
                                        azure_temp.Users
                                    SET
                                        Status = ${EventStatus.AWAITING_CREATION},
                                        Username = @RESULTOUT
                                    OUTPUT
                                        INSERTED.Status as status
                                    WHERE RowID = @RowID;
                                END
                        END
                    ELSE
                        BEGIN
                            --if all is good the username should be generated and the status should be set to 0 so the create job can pick up the row.
                            UPDATE
                                azure_temp.Users
                            SET
                                Status = ${EventStatus.AWAITING_CREATION},
                                Username = NULL
                            OUTPUT
                                INSERTED.Status as status
                            WHERE RowID = @RowID;
                        END
                END;
            END
            `,
            [rowID, firstName, middleName, lastName, email, userRole, personID, WorkflowType.USER_CREATE],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async restartFailedWorkflows(dtos: AzureUsersResponseDTO[]) {
        const rowIDs = dtos.map((dto) => dto.rowID).join(`,`);
        const result = await this.entityManager.query(
            `
                UPDATE 
                    azure_temp.Users
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

    async getGeneratedUser(dto: AzureUsersResponseDTO) {
        const { username } = dto;
        const result = await this.entityManager.query(
            `
                SELECT 
                    Username as username
                FROM
                    azure_temp.GeneratedUsers
                WHERE
                    Username = @0
            `,
            [username],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);
        return transformedResult[0];
    }

    async getArchivedPreviousYears(query: ArchivedResourceQueryDTO) {
        const { identifier, schoolYear } = query;

        let queryBuilder = this.connection.createQueryBuilder(UsersArchivedPreviousYearsEntity, 'uapy');

        if (schoolYear) {
            const dateInterval = SchoolYearService.mapSchoolYearToDateInterval(schoolYear);
            queryBuilder = queryBuilder.where('uapy.CreatedOn >= :startDate AND uapy.CreatedOn <= :endDate', {
                startDate: dateInterval.startDate,
                endDate: dateInterval.endDate,
            });
            queryBuilder = queryBuilder.andWhere('uapy.PersonID = :identifier', { identifier });
        } else {
            queryBuilder = queryBuilder.where('uapy.PersonID = :identifier', { identifier });
        }

        return queryBuilder.getMany();
    }
}
