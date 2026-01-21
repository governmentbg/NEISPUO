import { PrimaryColumn, ViewColumn, ViewEntity } from 'typeorm';
import { AuditActionEnum } from '../constants/enum/audit-log-action.enum';

@ViewEntity({ schema: 'logs', name: 'RoleAudit' })
export class RoleAuditView {
    @PrimaryColumn()
    AuditId: number;

    @ViewColumn()
    AuditModuleId: number;

    @ViewColumn()
    SysUserID: number;

    @ViewColumn()
    Username: string;

    @ViewColumn()
    SysRoleId: number;

    @ViewColumn()
    ObjectName: string;

    @ViewColumn()
    ObjectId: number;

    @ViewColumn()
    DateUtc: Date | string;

    @ViewColumn()
    Action: AuditActionEnum;

    @ViewColumn()
    InstId: number;

    @ViewColumn()
    AssignedToSysUserID: number;

    @ViewColumn()
    AssignedToSysUsername: string;

    @ViewColumn()
    AssignedSysRoleID: number;

    @ViewColumn()
    AssignedSysRoleName: string;

    @ViewColumn()
    AssignedInstitutionID: number;
}
