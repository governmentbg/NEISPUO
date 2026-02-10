// mappers are used to convert one object to another

import { EventStatus } from '../constants/enum/event-status.enum';
import { AzureClassesResponseDTO } from '../dto/responses/azure-classes-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class AzureClassesMapper {
    static transform(classesObjects: any[]) {
        const result: AzureClassesResponseDTO[] = [];
        for (const classObject of classesObjects) {
            const elementToBeInserted: AzureClassesResponseDTO = {
                rowID: classObject.rowID,
                classID: classObject.classID,
                workflowType: classObject.workflowType,
                title: classObject.title,
                classCode: classObject.classCode,
                orgID: classObject.orgID,
                termID: classObject.termID,
                termName: classObject.termName,
                termStartDate: classObject.termStartDate,
                termEndDate: classObject.termEndDate,
                inProcessing: classObject.inProcessing,
                errorMessage: classObject.errorMessage,
                createdOn: classObject.createdOn,
                updatedOn: classObject.updatedOn,
                guid: classObject.guid,
                retryAttempts: classObject.retryAttempts,
                status: EventStatus[classObject.status],
                azureID: classObject.azureID,
                inProgressResultCount: classObject.inProgressResultCount,
                isForArchivation: classObject.isForArchivation,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
