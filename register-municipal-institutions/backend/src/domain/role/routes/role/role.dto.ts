import { RoleNameEnum } from '../../enums/role-name.enum';
import { Role } from '../../role.entity';

export class RoleDTO extends Role {
    readonly id?: string;
    readonly roleName: RoleNameEnum;
}
