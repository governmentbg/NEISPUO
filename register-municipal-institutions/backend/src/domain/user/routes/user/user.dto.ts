import { User } from '../../user.entity';
import { RoleDTO } from '../../../role/routes/role/role.dto';

export class UserDTO extends User {
    confirmPassword?: string;
    recaptchaToken?: string;
    roles?: RoleDTO[];
}

export class UserResponseDTO {}
