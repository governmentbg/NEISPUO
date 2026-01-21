import { GraphApiStudentDTO } from './graph-api-student.dto';
import { GraphApiTeacherDTO } from './graph-api-teacher.dto';
import { DTO } from './responses/dto.interface';

export class GraphApiResponseDTO implements DTO {
    // @TODO if we come across a need for some of the other fields except for student teacher. they should be made into seperate DTO object. e.g department field and some others which are not primitive types.
    dataContext: string;

    id: string;

    accountEnabled: boolean;

    businessPhones: [];

    department: {};

    displayName: string;

    givenName: string;

    mail: string;

    mailNickname: string;

    mobilePhone: {};

    passwordPolicies: string;

    officeLocation: {};

    preferredLanguage: string;

    refreshTokensValidFromDateTime: Date;

    showInAddressList: {};

    surname: string;

    usageLocation: string;

    userPrincipalName: string;

    userType: string;

    middleName: string;

    externalSource: string;

    externalSourceDetail: string;

    primaryRole: string;

    passwordProfile: {};

    assignedLicenses: [];

    assignedPlans: [];

    provisionedPlans: [];

    onPremisesInfo: {};

    student: GraphApiStudentDTO;

    teacher: GraphApiTeacherDTO;
}
