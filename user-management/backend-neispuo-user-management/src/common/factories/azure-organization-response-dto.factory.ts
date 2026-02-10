import { RequestContext } from 'nestjs-request-context';
import { AzureOrganizationsResponseDTO } from '../dto/responses/azure-organizations-response.dto';
import { InstitutionResponseDTO } from '../dto/responses/institution-response.dto';

export class AzureOrganizationResponseFactory {
    static createFromInstitutionResponseDTO(response: InstitutionResponseDTO) {
        const requestObject: any = RequestContext?.currentContext?.req;
        const userName = requestObject?._authObject?.selectedRole?.Username ?? null;
        const {
            institutionName,
            description,
            principalName,
            highestGrade,
            lowestGrade,
            phone,
            townName,
            areaName,
            countryName,
            postalCode,
            street,
            institutionID,
            sysUserID,
            personID,
            azureID,
        } = response;
        const dto: AzureOrganizationsResponseDTO = {
            name: institutionName,
            description: description,
            principalName: principalName,
            principalEmail: userName,
            highestGrade: highestGrade,
            lowestGrade: lowestGrade,
            phone: phone,
            city: townName,
            area: areaName,
            country: countryName,
            postalCode: postalCode,
            street: street,
            organizationID: institutionID,
            sysUserID: sysUserID,
            personID: personID,
            azureID: azureID,
        };
        return dto;
    }
}
