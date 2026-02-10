// mappers are used to convert one object to another

import { EventStatus } from '../constants/enum/event-status.enum';
import { AzureUsersResponseDTO } from '../dto/responses/azure-users-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class AzureUsersMapper {
    static transform(usersObjects: any[]) {
        const result: AzureUsersResponseDTO[] = [];
        for (const usersObject of usersObjects) {
            const elementToBeInserted: AzureUsersResponseDTO = {
                rowID: usersObject.rowID,
                userID: usersObject.userID,
                workflowType: usersObject.workflowType,
                identifier: usersObject.identifier,
                firstName: usersObject.firstName,
                middleName: usersObject.middleName,
                surname: usersObject.surname,
                password: usersObject.password,
                email: usersObject.email,
                phone: usersObject.phone,
                grade: usersObject.grade,
                schoolId: usersObject.schoolId,
                birthDate: usersObject.birthDate,
                userRole: usersObject.userRole,
                additionalRole: usersObject.additionalRole,
                sisAccessSecondaryRole: usersObject.sisAccessSecondaryRole,
                accountEnabled: usersObject.accountEnabled,
                inProcessing: usersObject.inProcessing,
                errorMessage: usersObject.errorMessage,
                createdOn: usersObject.createdOn,
                updatedOn: usersObject.updatedOn,
                guid: usersObject.guid,
                retryAttempts: usersObject.retryAttempts,
                username: usersObject.username,
                hasNeispuoAccess: usersObject.hasNeispuoAccess,
                status: EventStatus[usersObject.status],
                personID: usersObject.personID,
                azureID: usersObject.azureID,
                assignedAccountantSchools: usersObject.assignedAccountantSchools,
                inProgressResultCount: usersObject.inProgressResultCount,
                isForArchivation: usersObject.isForArchivation,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
