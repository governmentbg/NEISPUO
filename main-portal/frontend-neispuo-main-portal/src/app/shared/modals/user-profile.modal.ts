export interface IUserProfile {
  SysUserID: number;
  Username: string;
  SysRoleID: number;
  SysRoleName: number;
  ThreeNames: string;
  PersonIDs: number;
  InstitutionID?: number;
  InstitutionName?: string;
  MunicipalityID?: number;
  MunicipalityName?: string;
  RegionID?: number;
  RegionName?: string;
  PositionID?: string;
  PositionName?: string;
  BudgetingInstitutionID?: number;
  BudgetingInstitutionName?: string;
}
