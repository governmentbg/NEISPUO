import { DTO } from './responses/dto.interface';

export class SysUserSysRoleCreateDTO implements DTO {
    sysUserID?: number;

    sysRoleID?: number;
}
