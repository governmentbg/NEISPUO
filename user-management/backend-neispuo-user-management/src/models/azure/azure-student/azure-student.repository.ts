/* eslint-disable prettier/prettier */
import { Injectable } from '@nestjs/common';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { MessageLevel } from 'src/common/constants/enum/siem/logger-level.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { AzureUsersMapper } from 'src/common/mappers/azure-users.mapper';
import { UserActionLogType } from 'src/common/types/siem/user-action-log.type';
import { SIEMLogEventType } from 'src/models/siem-logger/siem-log-event-type.enum';
import { SIEMLoggerService } from 'src/models/siem-logger/siem-logger.service';
import { TransactionEventHandlerService } from 'src/models/transaction-event-handler/transaction-event-handler.service';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class AzureStudentRepository {
    entityManager = getManager();

    constructor(
        private connection: Connection,
        private readonly transactionEventHandlerService: TransactionEventHandlerService,
    ) {}

    async insertAzureStudent(dto: AzureUsersResponseDTO, request?: AuthedRequest) {
        const {
            identifier,
            firstName,
            middleName,
            lastName,
            grade,
            schoolId,
            birthDate,
            userRole,
            accountEnabled,
            hasNeispuoAccess,
            userID,
            personID,
            additionalRole,
        } = dto;
        const result = await this.entityManager.query(
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
                    AdditionalRole,
                    HasNeispuoAccess,
                    UserID,
                    PersonID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13);
            `,
            [
                WorkflowType.USER_CREATE,
                identifier,
                firstName,
                middleName,
                lastName,
                grade,
                schoolId,
                birthDate,
                userRole,
                accountEnabled,
                additionalRole,
                hasNeispuoAccess,
                userID,
                personID,
            ],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);

        this.transactionEventHandlerService.registerCommitCallback(
            this.entityManager,
            (dto: UserActionLogType, siemLoggerService: SIEMLoggerService) => {
                siemLoggerService.send(siemLoggerService.buildSIEMLogObject(dto));
            },
            {
                request,
                messageLevel: MessageLevel.WARN,
                event: SIEMLogEventType.USER_CREATED,
                attributes: { rowID: +result.rowID, ...dto },
            },
        );
        return transformedResult[0];
    }

    async updateAzureStudent(dto: AzureUsersResponseDTO, entityManager?: EntityManager, request?: AuthedRequest) {
        const {
            identifier,
            firstName,
            middleName,
            lastName,
            grade,
            schoolId,
            birthDate,
            userRole,
            accountEnabled,
            hasNeispuoAccess,
            userID,
            personID,
            azureID,
            isForArchivation,
            additionalRole,
        } = dto;
        let result = false;
        try {
            await this.connection.transaction(async (m) => {
                const manager = entityManager ? entityManager : m;
                const insertedAzureUserResult = await manager.query(
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
                            AdditionalRole,
                            HasNeispuoAccess,
                            UserID,
                            PersonID,
                            Status,
                            IsForArchivation,
                            AzureID
                        )
                        OUTPUT Inserted.RowID as rowID
                        VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16);
                        `,
                    [
                        WorkflowType.USER_UPDATE,
                        identifier,
                        firstName,
                        middleName,
                        lastName,
                        grade,
                        schoolId,
                        birthDate,
                        userRole,
                        accountEnabled,
                        additionalRole,
                        hasNeispuoAccess,
                        userID,
                        personID,
                        EventStatus.AWAITING_CREATION,
                        isForArchivation,
                        azureID,
                    ],
                );
                const transformedAzureUserResult: AzureUsersResponseDTO[] =
                    AzureUsersMapper.transform(insertedAzureUserResult);
                const insertedAzureUserRowID = transformedAzureUserResult[0]?.rowID;
                dto.rowID = insertedAzureUserRowID;
                if (insertedAzureUserRowID) result = true;

                this.transactionEventHandlerService.registerCommitCallback(
                    manager,
                    (dto: UserActionLogType, siemLoggerService: SIEMLoggerService) => {
                        siemLoggerService.send(siemLoggerService.buildSIEMLogObject(dto));
                    },
                    {
                        request,
                        messageLevel: MessageLevel.WARN,
                        event: SIEMLogEventType.USER_UPDATED,
                        attributes: { ...dto },
                    },
                );
            });
        } catch (error) {}
        return result;
    }

    async deleteAzureStudent(dto: AzureUsersResponseDTO, entityManager?: EntityManager, request?: AuthedRequest) {
        const { deletionType, userID, userRole, personID, isForArchivation, azureID } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `
                INSERT
                INTO
                azure_temp.Users (
                    WorkflowType,
                    UserID,
                    DeletionType,
                    UserRole,
                    PersonID,
                    Status,
                    IsForArchivation,
                    AzureID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7);
            `,
            [
                WorkflowType.USER_DELETE,
                userID,
                deletionType,
                userRole,
                personID,
                EventStatus.AWAITING_CREATION,
                isForArchivation,
                azureID,
            ],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);

        this.transactionEventHandlerService.registerCommitCallback(
            manager,
            (dto: UserActionLogType, siemLoggerService: SIEMLoggerService) => {
                siemLoggerService.send(siemLoggerService.buildSIEMLogObject(dto));
            },
            {
                request,
                messageLevel: MessageLevel.WARN,
                event: SIEMLogEventType.USER_DELETED,
                attributes: { rowID: +result.rowID, ...dto },
            },
        );

        return transformedResult[0];
    }

    async disableAzureStudent(dto: AzureUsersResponseDTO, request?: AuthedRequest) {
        const {
            deletionType,
            userID,
            userRole,
            accountEnabled,
            firstName,
            middleName,
            lastName,
            hasNeispuoAccess,
            personID,
            azureID,
        } = dto;

        const result = await this.entityManager.query(
            `
                INSERT
                INTO
                azure_temp.Users (
                    WorkflowType,
                    UserID,
                    DeletionType,
                    UserRole,
                    accountEnabled,
                    FirstName,
                    MiddleName,
                    Surname,
                    HasNeispuoAccess,
                    PersonID,
                    Status,
                    AzureID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11);
            `,
            [
                WorkflowType.USER_UPDATE,
                userID,
                deletionType,
                userRole,
                accountEnabled,
                firstName,
                middleName,
                lastName,
                hasNeispuoAccess,
                personID,
                EventStatus.AWAITING_CREATION,
                azureID,
            ],
        );
        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);

        this.transactionEventHandlerService.registerCommitCallback(
            this.entityManager,
            (dto: UserActionLogType, siemLoggerService: SIEMLoggerService) => {
                siemLoggerService.send(siemLoggerService.buildSIEMLogObject(dto));
            },
            {
                request,
                messageLevel: MessageLevel.WARN,
                event: SIEMLogEventType.STUDENT_DISABLE,
                attributes: { rowID: +result.rowID, ...dto },
            },
        );

        return transformedResult[0];
    }
}
