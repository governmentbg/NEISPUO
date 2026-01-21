export interface SelectedRole {
    Username: string;
    SysUserID: number;
    SysRoleID: number;
    InstitutionID?: number;
    PositionID?: number;
    MunicipalityID: number;
    RegionID?: number;
    BudgetingInstitutionID?: number;
    _concatID: string;
    PersonID: number;
}

export interface JwtPayloadDTO {
    sub: string;
    selected_role: SelectedRole;
    at_hash: string;
    aud: string;
    exp: number;
    iat: number;
    iss: string;
}
