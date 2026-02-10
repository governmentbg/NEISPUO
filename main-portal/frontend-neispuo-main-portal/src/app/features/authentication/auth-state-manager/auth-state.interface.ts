import { RoleEnum } from "@shared/enums/role.enum";

export interface AuthState {
  sub?: string;
  roles?: string[];
  selected_role?: {
    BudgetingInstitutionID?: number;
    InstitutionID?: number;
    MunicipalityID?: number;
    PersonID?: number;
    PositionID?: number;
    RegionID?: number;
    SysRoleID: RoleEnum;
    SysUserID?: number;
    Username: string;
  };
  fullName?: string;
  jwt?: string;
  authReady?: boolean;
  oidcAccessToken?: string;
  session_state?: string;
  FirstName?:string;
  LastName?:string;
}
