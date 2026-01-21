// mappers are used to convert one object to another

import { CurriculumStudentResponseDTO } from '../dto/responses/curriculum-student-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class CurriculumStudentMapper {
    static transform(classesObjects: any[]) {
        const result: CurriculumStudentResponseDTO[] = [];
        for (const classObject of classesObjects) {
            const elementToBeInserted: CurriculumStudentResponseDTO = {
                curriculumID: classObject.curriculumID,
                personID: classObject.personID,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
