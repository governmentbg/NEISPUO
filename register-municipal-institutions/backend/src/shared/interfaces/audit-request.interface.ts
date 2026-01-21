import { AuthedRequest } from './authed-request.interface';

export interface AuditObject {
    authedRequest: AuthedRequest;
    AuditModuleId?: number,
    SysUserId?: number,
    SysRoleId?: number,
    Username?: string,
    FirstName?: null,
    MiddleName?: null,
    LastName?: null,
    LoginSessionId?: null,
    RemoteIpAddress?: number,
    UserAgent?: string,
    DateUtc?: Date,
    SchoolYear?: null,
    InstId?: number,
    PersonId?: number,
    objectName: string,
    objectID: number,
    action: string,
    oldValue: object,
    newValue: object,
}
