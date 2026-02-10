// mappers are used to convert one object to another

import { EntitiesInGenerationDTO } from '../dto/entities-in-generation.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
export class EntitiesInGenerationMapper {
    static transform(entitiesInGeneration: any[]) {
        const result: EntitiesInGenerationDTO[] = [];
        for (const entitiesInGenerationObject of entitiesInGeneration) {
            result.push({
                identifier: entitiesInGenerationObject.identifier,
                createdOn: entitiesInGenerationObject.createdOn,
            });
        }
        return result;
    }
}
