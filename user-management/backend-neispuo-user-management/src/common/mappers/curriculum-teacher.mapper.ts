// mappers are used to convert one object to another

import { CurriculumTeacherResponseDTO } from '../dto/responses/curriculum-teacher-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class CurriculumTeacherMapper {
    static transform(classesObjects: any[]) {
        const result: CurriculumTeacherResponseDTO[] = [];
        for (const classObject of classesObjects) {
            const elementToBeInserted: CurriculumTeacherResponseDTO = {
                curriculumID: classObject.curriculumID,
                personID: classObject.personID,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
