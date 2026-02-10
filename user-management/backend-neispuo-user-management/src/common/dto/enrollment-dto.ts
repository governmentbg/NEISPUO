import { UserRoleType } from '../constants/enum/role-type-enum';

export class EnrollmentDTO {
    curriculumID?: number;

    organizationPersonID?: number;

    userPersonID: number;

    userRole: UserRoleType;
}
