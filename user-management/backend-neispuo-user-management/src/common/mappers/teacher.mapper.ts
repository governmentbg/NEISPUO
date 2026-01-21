// mappers are used to convert one object to another

import { TeacherResponseDTO } from '../dto/responses/teacher-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class TeacherMapper {
    static transform(teacherObjects: any[]) {
        const result: TeacherResponseDTO[] = [];
        for (const teacherObject of teacherObjects) {
            const elementToBeInserted: TeacherResponseDTO = {
                sysUserID: teacherObject.sysUserID,
                personID: teacherObject.personID,
                isAzureUser: teacherObject.isAzureUser,
                isAzureSynced: teacherObject.isAzureSynced,
                username: teacherObject.username,
                firstName: teacherObject.firstName,
                middleName: teacherObject.middleName,
                lastName: teacherObject.lastName,
                institutionID: teacherObject.institutionID,
                institutionName: teacherObject.institutionName,
                threeNames: `${
                    teacherObject.firstName
                }${` ${teacherObject.middleName}`}${` ${teacherObject.lastName}`}`,
                townName: teacherObject.townName,
                municipalityName: teacherObject.municipalityName,
                regionName: teacherObject.regionName,
                positionName: teacherObject.positionName,
                positionID: teacherObject.positionID,
                personalID: teacherObject.personalID,
                grade: teacherObject.grade,
                phone: teacherObject.phone,
                birthDate: teacherObject.birthDate,
                hasNeispuoAccess: teacherObject.hasNeispuoAccess,
                azureID: teacherObject.azureID,
                publicEduNumber: teacherObject.publicEduNumber,
                additionalRole: teacherObject.additionalRole,
                sisAccessSecondaryRole: teacherObject.sisAccessSecondaryRole,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
