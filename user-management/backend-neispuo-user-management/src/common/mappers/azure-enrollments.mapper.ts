// mappers are used to convert one object to another

import { EventStatus } from '../constants/enum/event-status.enum';
import { AzureEnrollmentsResponseDTO } from '../dto/responses/azure-enrollments-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class AzureEnrollmentsMapper {
    static transform(enrollmentsObjects: any[]) {
        const result: AzureEnrollmentsResponseDTO[] = [];
        for (const enrollmentObject of enrollmentsObjects) {
            const elementToBeInserted: AzureEnrollmentsResponseDTO = {
                rowID: enrollmentObject.rowID,
                workflowType: enrollmentObject.workflowType,
                userAzureID: enrollmentObject.userAzureID,
                classAzureID: enrollmentObject.classAzureID,
                organizationAzureID: enrollmentObject.organizationAzureID,
                userRole: enrollmentObject.userRole,
                curriculumID: enrollmentObject.curriculumID,
                userPersonID: enrollmentObject.userPersonID,
                organizationPersonID: enrollmentObject.organizationPersonID,
                inProcessing: enrollmentObject.onProcessing,
                errorMessage: enrollmentObject.errorMessage,
                createdOn: enrollmentObject.createdOn,
                updatedOn: enrollmentObject.updatedOn,
                guid: enrollmentObject.guid,
                retryAttempts: enrollmentObject.retryAttempts,
                status: EventStatus[enrollmentObject.status],
                inProgressResultCount: enrollmentObject.inProgressResultCount,
                isForArchivation: enrollmentObject.isForArchivation,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
