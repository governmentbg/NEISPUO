import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';

export class UserEventDto {
    id: string;

    identifier: string;

    firstName: string;

    middleName: string;

    surname: string;

    password: string;

    email: string;

    phone: string;

    grade: string;

    schoolId: string;

    birthDate: string;

    role: UserRoleType;

    accountEnabled: boolean;

    sisAccessRole: RoleEnum;

    hasSisAccess: boolean;

    azureId: string;

    username: string;

    sisAccountantSchools: string[];
}
