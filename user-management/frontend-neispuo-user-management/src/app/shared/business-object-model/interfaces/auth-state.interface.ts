export interface AuthState {
    sub?: string;
    roles?: string[];
    selected_role?: {
        SysUserID: number;
        SysRoleID: number;
        IsLeadTeacher?: boolean;
    };
    fullName?: string;
    FirstName?: string;
    LastName?: string;
    jwt?: string;
    authReady?: boolean;
    oidcAccessToken?: string;
    session_state?: string;
    impersonator?: string;
    impersonatorSysUserID?: number;
}
