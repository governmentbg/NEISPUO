import { FixAccountantRolesDTO } from '../dto/fix-accountant-roles.dto';

export class FixAccountantRolesMapper {
    static transform(teacherObjects: any[]) {
        const result: FixAccountantRolesDTO[] = [];
        for (const teacherObject of teacherObjects) {
            const elementToBeInserted: FixAccountantRolesDTO = {
                personID: teacherObject.personID,
                firstName: teacherObject.firstName,
                middleName: teacherObject.middleName,
                lastName: teacherObject.lastName,
                birthDate: teacherObject.birthDate,
                azureID: teacherObject.azureID,
                userID: teacherObject.userID,
                institutionIDs: teacherObject.institutionIDs,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
