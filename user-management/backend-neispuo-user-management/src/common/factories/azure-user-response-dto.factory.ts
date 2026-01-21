import { AccountEnabledEnum } from '../constants/enum/account-enabled.enum';
import { HasNeispuoAccess } from '../constants/enum/has-neispuo-access.enum';
import { UserRoleType } from '../constants/enum/role-type-enum';
import { ParentCreateRequestDTO } from '../dto/requests/parent-create-request.dto';
import { AzureUsersResponseDTO } from '../dto/responses/azure-users-response.dto';
import { StudentResponseDTO } from '../dto/responses/student-response.dto';
import { TeacherResponseDTO } from '../dto/responses/teacher-response.dto';

export class AzureUserResponseFactory {
    static createFromTeacherResponseDTO(response: TeacherResponseDTO) {
        const {
            firstName,
            middleName,
            lastName,
            grade,
            institutionID,
            birthDate,
            personalID,
            positionID,
            hasNeispuoAccess,
            personID,
            azureID,
            additionalRole,
            sisAccessSecondaryRole,
            assignedAccountantSchools,
        } = response;
        const dto: AzureUsersResponseDTO = {
            identifier: personalID,
            firstName: firstName,
            middleName: middleName,
            lastName: lastName,
            surname: lastName,
            grade: grade,
            schoolId: institutionID,
            birthDate: birthDate,
            userRole: UserRoleType.TEACHER,
            accountEnabled: AccountEnabledEnum.ENABLED,
            userID: personalID,
            additionalRole: additionalRole,
            sisAccessSecondaryRole: sisAccessSecondaryRole,
            deletionType: +positionID,
            hasNeispuoAccess,
            personID: personID,
            azureID: azureID,
            assignedAccountantSchools: assignedAccountantSchools,
            isForArchivation: 0,
        };
        return dto;
    }

    static createFromStudentResponseDTO(response: StudentResponseDTO) {
        const {
            firstName,
            middleName,
            lastName,
            grade,
            institutionID,
            birthDate,
            personalID,
            positionID,
            hasNeispuoAccess,
            personID,
            azureID,
            additionalRole,
        } = response;
        const dto: AzureUsersResponseDTO = {
            identifier: personalID,
            firstName: firstName,
            middleName: middleName,
            lastName: lastName,
            surname: lastName,
            grade: grade,
            schoolId: institutionID,
            birthDate: birthDate,
            userRole: UserRoleType.STUDENT,
            accountEnabled: AccountEnabledEnum.ENABLED,
            userID: personalID,
            additionalRole: additionalRole,
            deletionType: +positionID,
            hasNeispuoAccess,
            personID: personID,
            azureID: azureID,
            isForArchivation: 0,
        };
        return dto;
    }

    static createFromParentCreateRequestDTO(response: ParentCreateRequestDTO) {
        const { firstName, middleName, lastName, personID, email, password, azureID } = response;
        const dto: AzureUsersResponseDTO = {
            identifier: personID?.toString(),
            firstName: firstName,
            middleName: middleName,
            lastName: lastName,
            surname: lastName,
            userRole: UserRoleType.PARENT,
            accountEnabled: AccountEnabledEnum.ENABLED,
            email: email,
            userID: personID?.toString(),
            password: password,
            additionalRole: null,
            deletionType: null,
            hasNeispuoAccess: HasNeispuoAccess.YES,
            personID: personID,
            azureID: azureID,
            isForArchivation: 0,
        };
        return dto;
    }
}
