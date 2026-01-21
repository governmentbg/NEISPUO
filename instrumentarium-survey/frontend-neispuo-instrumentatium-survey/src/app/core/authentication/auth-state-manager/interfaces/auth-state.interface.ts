import { SelectedRole } from "./selected-role.interface";

export interface AuthState {
  sub?: string;
  roles?: string[];
  selected_role?: SelectedRole;
  fullName?: string;
  jwt?: string;
  authReady?: boolean;
  oidcAccessToken?: string;
  mySysUser?: any; // TODO: Add model
  myMunicipality?: any; // TODO: Add model
  FirstName?:string;
  LastName?:string;
}
