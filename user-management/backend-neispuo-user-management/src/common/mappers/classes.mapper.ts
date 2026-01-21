// mappers are used to convert one object to another

import { ClassesResponseDTO } from '../dto/responses/classes-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class ClassesMapper {
    static transform(classesObjects: any[]) {
        const result: ClassesResponseDTO[] = [];
        for (const classObject of classesObjects) {
            const elementToBeInserted: ClassesResponseDTO = {
                curriculumID: classObject.curriculumID,
                institutionID: classObject.institutionID,
                institutionAzureID: classObject.institutionAzureID,
                className: classObject.className,
                subjectName: classObject.subjectName,
                azureID: classObject.azureID,
                startDate: classObject.startDate,
                endDate: classObject.endDate,
                subjectTypeName: classObject.subjectTypeName,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
