import { UserRoleType } from 'src/common/constants/enum/role-type-enum';

export class DeleteUserEventDto {
    id: string;

    role: UserRoleType;
}
