// mappers are used to convert one object to another

import { SchoolBookCodeAssignResponseDTO } from '../dto/requests/school-books-code-assign-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class SchoolBookCodeMapper {
    static transform(codeObjects: any[]) {
        const result: SchoolBookCodeAssignResponseDTO[] = [];
        for (const codeObject of codeObjects) {
            result.push({
                code: codeObject.code,
                personID: codeObject.personID,
            });
        }
        return result;
    }
}
