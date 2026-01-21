import { RoleEnum } from '../../enums/roles.enum';

export interface UserRole {
    sysUserID?: number;
    sysRoleID?: RoleEnum;
    institutionID?: number;
}
export interface UserRolesResponse {
    status: number;
    message: string;
    payload: {
        data: UserRole[][];
    };
}
