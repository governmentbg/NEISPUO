import { Injectable } from '@nestjs/common';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { EntitiesInGenerationDTO } from 'src/common/dto/entities-in-generation.dto';
import { EntityManager } from 'typeorm';
import { EntitiesInGenerationRepository } from '../entities-in-generation.repository';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['deleteOldEntitiesInGeneration', 'insertEntitiesInGeneration'],
})
export class EntitiesInGenerationService {
    constructor(private entitiesInGenerationRepository: EntitiesInGenerationRepository) {}

    async deleteOldEntitiesInGeneration() {
        await this.entitiesInGenerationRepository.deleteOldEntitiesInGeneration();
    }

    insertEntitiesInGeneration(dto: EntitiesInGenerationDTO, entityManager?: EntityManager) {
        return this.entitiesInGenerationRepository.insertEntitiesInGeneration(dto, entityManager);
    }

    async deleteEntitiesInGenerationByIdentifier(dto: EntitiesInGenerationDTO) {
        await this.entitiesInGenerationRepository.deleteEntitiesInGenerationByIdentifiers([dto]);
    }

    async deleteEntitiesInGenerationByIdentifiers(dtos: EntitiesInGenerationDTO[]) {
        await this.entitiesInGenerationRepository.deleteEntitiesInGenerationByIdentifiers(dtos);
    }

    async entitiesInGenerationExists(dto: EntitiesInGenerationDTO) {
        const result = await this.entitiesInGenerationRepository.getEntitiesInGenerationByIdentifier(dto);
        return result && result?.length > 0 ? true : false;
    }
}
