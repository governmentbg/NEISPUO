// mappers are used to convert one object to another

import { VariablesResponseDTO } from '../dto/responses/variables-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class VariablesMapper {
    static transform(variableObjects: any[]) {
        const result: VariablesResponseDTO[] = [];
        for (const variableObject of variableObjects) {
            const elementToBeInserted: VariablesResponseDTO = {
                name: variableObject.name,
                value: variableObject.value,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
