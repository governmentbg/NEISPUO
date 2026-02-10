// mappers are used to convert one object to another

import { EventStatus } from '../constants/enum/event-status.enum';
import { AzureOrganizationsResponseDTO } from '../dto/responses/azure-organizations-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class AzureOrganizationsMapper {
    static transform(organizationsObjects: any[]) {
        const result: AzureOrganizationsResponseDTO[] = [];
        for (const organizationObject of organizationsObjects) {
            const elementToBeInserted: AzureOrganizationsResponseDTO = {
                rowID: organizationObject.rowID,
                organizationID: organizationObject.organizationID,
                workflowType: organizationObject.workflowType,
                name: organizationObject.name,
                description: organizationObject.description,
                principalId: organizationObject.principalId,
                principalName: organizationObject.principalName,
                principalEmail: organizationObject.principalEmail,
                highestGrade: organizationObject.highestGrade,
                lowestGrade: organizationObject.lowestGrade,
                phone: organizationObject.phone,
                city: organizationObject.city,
                area: organizationObject.area,
                country: organizationObject.country,
                postalCode: organizationObject.postalCode,
                street: organizationObject.street,
                inProcessing: organizationObject.inProcessing,
                errorMessage: organizationObject.errorMessage,
                createdOn: organizationObject.createdOn,
                updatedOn: organizationObject.updatedOn,
                guid: organizationObject.guid,
                retryAttempts: organizationObject.retryAttempts,
                status: EventStatus[organizationObject.status],
                username: organizationObject.username,
                password: organizationObject.password,
                azureID: organizationObject.azureID,
                inProgressResultCount: organizationObject.inProgressResultCount,
                isForArchivation: organizationObject.isForArchivation,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
