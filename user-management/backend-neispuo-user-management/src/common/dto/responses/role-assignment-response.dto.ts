import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { DTO } from './dto.interface';

export class RoleAssignmentResponseDTO implements DTO {
    sysUserID?: number;

    sysRoleID?: RoleEnum;

    institutionID?: number;
}
