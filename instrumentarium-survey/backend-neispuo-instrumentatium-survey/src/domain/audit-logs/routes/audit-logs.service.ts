import { Injectable } from '@nestjs/common';
import { AuditObject } from '@shared/interfaces/audit-request.interface';
import { Connection, EntityManager } from 'typeorm';
import { AuditLogsEntity } from '../audit-logs.entity';
import { AuditModuleEnum } from '../enums/audit-module.enum';
import { v4 as uuid } from 'uuid';

@Injectable()
export class AuditLogsService {
    constructor(
        private connection: Connection
      ) {
    
      }
    

    public async insertAudit(auditRequest: AuditObject, transactionManager: Connection | EntityManager =
        this.connection) {
        const correlationUuid = uuid();

        let selectedRole = null;
        if(auditRequest.authedRequest._authObject) {
          selectedRole = auditRequest.authedRequest._authObject.selectedRole;
        }
        await transactionManager.getRepository(AuditLogsEntity).insert({
          AuditCorrelationId: correlationUuid,
          AuditModuleId: AuditModuleEnum.SURVEY,
          SysUserId: selectedRole ? selectedRole.SysUserID : null,
          SysRoleId: selectedRole ? selectedRole.SysRoleID : null, 
          Username: selectedRole ? selectedRole.Username : null,
          FirstName: null,
          MiddleName: null,
          LastName: null,
          LoginSessionId: null,
          RemoteIpAddress: auditRequest.authedRequest.ip,
          UserAgent: auditRequest.authedRequest.get('User-Agent'),
          DateUtc: new Date(),
          SchoolYear: null,
          InstId: selectedRole ? selectedRole.InstitutionID : null,
          PersonId: selectedRole ? selectedRole.PersonID : null,
          ObjectName: auditRequest.objectName,
          ObjectId: auditRequest.objectID,
          Action: auditRequest.action,
          Data: JSON.stringify({
            OldValue: auditRequest.oldValue,
            NewValue: auditRequest.newValue,
          })
        })
      }
}
