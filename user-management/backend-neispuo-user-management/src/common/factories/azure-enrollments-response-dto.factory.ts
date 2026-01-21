import { EnrollmentDTO } from '../dto/enrollment-dto';
import { AzureEnrollmentsResponseDTO } from '../dto/responses/azure-enrollments-response.dto';

export class AzureEnrollmentsResponseFactory {
    static createFromEnrollmentDTO(response: EnrollmentDTO) {
        const { curriculumID, userPersonID, organizationPersonID, userRole } = response;
        const dto: AzureEnrollmentsResponseDTO = {
            classAzureID: null,
            organizationAzureID: null,
            userAzureID: null,
            userRole,
            curriculumID: curriculumID ? curriculumID : null,
            organizationPersonID: organizationPersonID ? organizationPersonID : null,
            userPersonID: userPersonID,
        };
        return dto;
    }
}
