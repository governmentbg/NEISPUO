import { RoleEnum } from '../../enums/roles.enum';

export class UpdateUserRolesRequestDTO {
    sysUserID?: number;

    sysRoleID?: RoleEnum;

    institutionID?: number;

    isDeleted?: boolean;
}
