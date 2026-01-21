import { Module } from '@nestjs/common';
import { EntitiesInGenerationService } from './routing/entities-in-generation.service';
import { EntitiesInGenerationRepository } from './entities-in-generation.repository';

@Module({
    providers: [EntitiesInGenerationService, EntitiesInGenerationRepository],
    controllers: [],
    exports: [EntitiesInGenerationService],
})
export class EntitiesInGenerationModule {}
