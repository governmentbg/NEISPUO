import { Injectable } from '@nestjs/common';
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
export class AzureParentRepository {
    entityManager = getManager();

    constructor(
        private connection: Connection,
        private readonly transactionEventHandlerService: TransactionEventHandlerService,
    ) {}

    async insertAzureParent(dto: AzureUsersResponseDTO, entityManager: EntityManager, request: AuthedRequest) {
        const manager = entityManager ? entityManager : this.entityManager;
        const {
            firstName,
            middleName,
            lastName,
            password,
            email,
            personID,
            userRole,
            accountEnabled,
            hasNeispuoAccess,
        } = dto;
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
                    Password,
                    Email,
                    UserRole,
                    AccountEnabled,
                    UserID,
                    HasNeispuoAccess,
                    PersonID
                )
                OUTPUT Inserted.RowID as rowID
                VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11);
            `,
            [
                WorkflowType.USER_CREATE,
                personID,
                firstName,
                middleName,
                lastName,
                password,
                email,
                userRole,
                accountEnabled,
                personID,
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
                event: SIEMLogEventType.PARENT_CREATED,
                attributes: { rowID: transformedResult?.[0]?.rowID, ...dto },
            },
        );

        return transformedResult[0];
    }
}
