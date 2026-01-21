export interface SelectedRole {
    Username: string;
    SysUserID: number;
    SysRoleID: number;
    PersonID?: number;
    InstitutionID?: number;
    PositionID?: number;
    MunicipalityID: number;
    RegionID?: number;
    BudgetingInstitutionID?: number;
    LeadTeacherClasses?: number[];
    IsLeadTeacher?: boolean;
    _concatID: string;
}

export interface JwtPayloadDTO {
    sub: string;
    sessionID: string;
    selected_role: SelectedRole;
    at_hash: string;
    aud: string;
    exp: number;
    iat: number;
    iss: string;
}
