import { AuditActionEnum } from '@shared/enums/audit-action.enum';

export interface RoleAudit {
    AuditId: number;
    SysUserID: number;
    Username: string;
    SysRoleId: number;
    DateUtc: Date | string;
    Action: AuditActionEnum;
    InstId: number;

    // from Data
    Data: {
        AssignedToSysUserID: number;
        AssignedToSysUsername: string;
        AssignedSysRoleID: number;
        AssignedSysRoleName: string;
        AssignedInstitutionID: number;
    };
}
