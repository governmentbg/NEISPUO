import { AuthedRequest } from './authed-request.interface';

interface UserManagementAPIRequestOptional {
    Id: number;
    AuditModuleId: number;
    CreateDate: Date;
    PersonID: number;
    RetryCount: number;
    LastRetryDate: Date;
    CreatedBySysUserId: number;
    CurriculumID: number;
    InstitutionID: number;
}

export interface UserManagementAPIRequest extends Partial<UserManagementAPIRequestOptional> {
    authedRequest: AuthedRequest;
    Url: string;
    Operation: string;
    Response: string;
    ResponseHttpCode: number;
    IsError: number;
}