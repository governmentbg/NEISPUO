import { DTO } from './responses/dto.interface';

export class GraphApiSchoolResponseDTO implements DTO {
    dataContext: string;

    id: string;

    displayName: string;

    principalName: string;

    principalEmail: string;

    phone: string;

    externalPrincipalId: string;

    highestGrade: string;

    lowestGrade: string;

    schoolNumber: string;

    externalId: string;

    externalSource: string;

    externalSourceDetail: string;
}
