// mappers are used to convert one object to another

import { EducationalStateDTO } from '../dto/responses/educational-state.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class EducationalStateMapper {
    static transform(eduStateObjects: any[]) {
        const result: EducationalStateDTO[] = [];
        for (const eduStateObject of eduStateObjects) {
            result.push({
                institutionID: eduStateObject.institutionID,
                personID: eduStateObject.personID,
                positionID: eduStateObject.positionID,
            });
        }
        return result;
    }
}
