import { Injectable } from '@nestjs/common';
import { AuditModuleEnum } from '@shared/enums/audit-module.enum';
import { UserManagementAPIRequest } from '@shared/interfaces/user-management-api-request.interface';
import { Connection, EntityManager } from 'typeorm';
import { UserManagementAPIRequestEntity } from './user-management-api-request.entity';

@Injectable()
export class UserManagementAPIRequestService {
    constructor(private connection: Connection) { }

    public async insertUserManagementAPIRequest(
        userManagementAPIRequest: UserManagementAPIRequest,
        transactionManager: Connection | EntityManager = this.connection,
    ) {
        const {
            authedRequest: {
                _authObject: {
                    selectedRole: { InstitutionID, PersonID, SysUserID }
                }
            },
        } = userManagementAPIRequest;

        await transactionManager.getRepository(UserManagementAPIRequestEntity).insert({
            AuditModuleId: AuditModuleEnum.REGISTER_MUNICIPAL_INSTITUTIONS,
            CreateDate: new Date(),
            Request: JSON.stringify({
                InstitutionID,
                PersonId: PersonID,
            }),
            PersonId: PersonID,
            RetryCount: 0,
            LastRetryDate: null,
            CreatedBySysUserId: SysUserID,
            CurriculumID: null,
            InstitutionID,
            ...userManagementAPIRequest
        });
    }
}
