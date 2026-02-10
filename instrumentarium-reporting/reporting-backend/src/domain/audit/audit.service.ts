import { Injectable, Logger } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { AuditEntity } from './audit.entity';
import { Repository } from 'typeorm';
import { AuthedRequest } from 'src/shared/interfaces/authed-request.interface';
import { AuditingModule } from './auditing-module.enum';
import { Action } from './action.enum';

@Injectable()
export class AuditLoggerService {
  private readonly logger = new Logger();

  constructor(
    @InjectRepository(AuditEntity)
    private readonly auditRepo: Repository<AuditEntity>,
  ) { }

  private getAction(request: AuthedRequest): Action {
    const { method, url } = request;
    const isLoadDataEndpoint = url.includes('/v1/cubejs/load');
    const isDownloadExcelEndpoint = url.includes('/v1/cubejs/download-excel');

    if (method === 'POST' && isDownloadExcelEndpoint) {
      return 'DOWNLOAD';
    } else if ((method === 'POST' || method === 'GET') && isLoadDataEndpoint) {
      return 'READ';
    } else if (
      method === 'POST' &&
      !(isLoadDataEndpoint && isDownloadExcelEndpoint)
    ) {
      return 'CREATE';
    }
    if (
      method === 'DELETE' &&
      !(isLoadDataEndpoint && isDownloadExcelEndpoint)
    ) {
      return 'DELETE';
    } else if (
      method === 'PUT' &&
      !(isLoadDataEndpoint && isDownloadExcelEndpoint)
    ) {
      return 'UPDATE';
    }
    return 'UNHANDLED';
  }

  private getObjectName(request: AuthedRequest) {
    try {
      let queryObject = request.query.query
        ? JSON.parse(request.query.query as string)
        : request.body.query;
      let objectName;
      objectName = queryObject.dimensions[0].split('.')[0];
      return objectName;
    } catch (e) {
      this.logger.error(`Failed to parse cubeJsQueryString.`);
      this.logger.error(e);
    }
    return null;
  }
  async logRequest(request: AuthedRequest) {
    const selectedRole = request._authObject.selectedRole;
    const { SysUserID, Username, PersonID, InstitutionID, SysRoleID } =
      selectedRole;
    const action = this.getAction(request);

    const auditEntity: Partial<AuditEntity> = {
      SysUserId: SysUserID,
      SysRoleId: SysRoleID,
      Username,
      PersonId: PersonID,
      DateUtc: new Date(),
      Action: action,
      InstId: InstitutionID,
      AuditModuleId: AuditingModule.REPORTING,
      LoginSessionId: request.requestId,
      RemoteIpAddress: request.ip,
      UserAgent: request.get('User-Agent'),
      ObjectName: this.getObjectName(request),
      ObjectId: null,
      Data: {
        originalURL: request.originalUrl,
        cubeQuery: request.query || request.body,
      },
    };

    await this.auditRepo.save(this.auditRepo.create(auditEntity));
  }
}
