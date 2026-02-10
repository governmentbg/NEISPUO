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
export class AzureTeachersRepository {
    entityManager = getManager();

    constructor(
        private connection: Connection,
        private readonly transactionEventHandlerService: TransactionEventHandlerService,
    ) {}

    async insertAzureTeacher(dto: AzureUsersResponseDTO, entityManager: EntityManager, request?: AuthedRequest) {
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
        } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
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
                    PersonID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14);
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
                event: SIEMLogEventType.USER_CREATED,
                attributes: { rowID: +result.rowID, ...dto },
            },
        );

        return transformedResult[0];
    }

    async updateAzureTeacher(dto: AzureUsersResponseDTO, request?: AuthedRequest) {
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
            isForArchivation,
            assignedAccountantSchools,
        } = dto;
        let result = false;
        try {
            await this.connection.transaction(async (manager) => {
                const insertAzureUserResult = await manager.query(
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
                            AzureID,
                            Status,
                            IsForArchivation,
                            AssignedAccountantSchools
                        )
                        OUTPUT Inserted.RowID as rowID
                        VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18);
                    `,
                    [
                        WorkflowType.USER_UPDATE,
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
                        EventStatus.AWAITING_CREATION,
                        isForArchivation,
                        assignedAccountantSchools,
                    ],
                );
                const transformedAzureUserResult: AzureUsersResponseDTO[] =
                    AzureUsersMapper.transform(insertAzureUserResult);
                const insertedAzureUserRowID = transformedAzureUserResult[0].rowID;
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

    async deleteAzureTeacher(dto: AzureUsersResponseDTO, request?: AuthedRequest) {
        const { userID, deletionType, userRole, personID, isForArchivation, azureID } = dto;

        const result = await this.entityManager.query(
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
            this.entityManager,
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
}
