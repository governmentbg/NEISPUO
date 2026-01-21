// mappers are used to convert one object to another

import { StudentResponseDTO } from '../dto/responses/student-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class StudentMapper {
    static transform(studentObjects: any[]) {
        const result: StudentResponseDTO[] = [];
        for (const studentObject of studentObjects) {
            const elementToBeInserted: StudentResponseDTO = {
                sysUserID: studentObject.sysUserID,
                personID: studentObject.personID,
                isAzureUser: studentObject.isAzureUser,
                isAzureSynced: studentObject.isAzureSynced,
                username: studentObject.username,
                firstName: studentObject.firstName,
                middleName: studentObject.middleName,
                lastName: studentObject.lastName,
                threeNames: `${
                    studentObject.firstName
                }${` ${studentObject.middleName}`}${` ${studentObject.lastName}`}`,
                institutionID: studentObject.institutionID,
                institutionName: studentObject.institutionName,
                townName: studentObject.townName,
                municipalityName: studentObject.municipalityName,
                regionName: studentObject.regionName,
                positionName: studentObject.positionName,
                positionID: studentObject.positionID,
                personalID: studentObject.personalID,
                phone: studentObject.phone,
                grade: studentObject.grade,
                birthDate: studentObject.birthDate,
                hasNeispuoAccess: studentObject.hasNeispuoAccess,
                azureID: studentObject.azureID,
                publicEduNumber: studentObject.publicEduNumber,
                additionalRole: studentObject.additionalRole,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
