import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { DTO } from '../responses/dto.interface';

export class EnrollmentGenerateDTO implements DTO {
    curriculumID?: number;

    institutionID?: number;

    personID: number;

    userRole?: UserRoleType;
}
