export interface SelectedRole {
  Username: string;
  PersonID: number;
  SysUserID: number;
  SysRoleID: number;
  InstitutionID?: number;
  PositionID?: number;
  MunicipalityID: number;
  RegionID?: number;
  BudgetingInstitutionID?: number;
  IsLeadTeacher?: boolean;
  _concatID: string;
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
