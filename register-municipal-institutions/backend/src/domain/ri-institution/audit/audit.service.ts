import { AuditModuleEnum } from '@shared/enums/audit-module.enum';
import { Injectable } from '@nestjs/common';
import { Connection, EntityManager } from 'typeorm';
import { v4 as uuid } from 'uuid';
import { AuditObject } from '@shared/interfaces/audit-request.interface';
import { AuditEntity } from './audit.entity';

@Injectable()
export class AuditService {
  constructor(
    private connection: Connection,
  ) {

  }

  public async insertAudit(auditRequest: AuditObject, transactionManager: Connection | EntityManager =
    this.connection) {
    const correlationUuid = uuid();

    await transactionManager.getRepository(AuditEntity).insert({
      AuditCorrelationId: correlationUuid,
      AuditModuleId: AuditModuleEnum.REGISTER_MUNICIPAL_INSTITUTIONS,
      SysUserId: auditRequest.authedRequest._authObject.selectedRole.SysUserID,
      SysRoleId: auditRequest.authedRequest._authObject.selectedRole.SysRoleID,
      Username: auditRequest.authedRequest._authObject.selectedRole.Username,
      FirstName: null,
      MiddleName: null,
      LastName: null,
      LoginSessionId: null,
      RemoteIpAddress: auditRequest.authedRequest.ip,
      UserAgent: auditRequest.authedRequest.get('User-Agent'),
      DateUtc: new Date(),
      SchoolYear: null,
      InstId: auditRequest.authedRequest._authObject.selectedRole.InstitutionID,
      PersonId: auditRequest.authedRequest._authObject.selectedRole.PersonID,
      ObjectName: auditRequest.objectName,
      ObjectId: auditRequest.objectID,
      Action: auditRequest.action,
      Data: JSON.stringify({
        OldValue: auditRequest.oldValue,
        NewValue: auditRequest.newValue,
      }),
    });
  }
}
