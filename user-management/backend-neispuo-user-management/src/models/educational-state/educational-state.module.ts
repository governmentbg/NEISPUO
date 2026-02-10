import { Module } from '@nestjs/common';
import { EducationalStateRepository } from './educational-state.repository';
import { EducationalStateService } from './routing/educational-state.service';

@Module({
    imports: [],
    providers: [EducationalStateService, EducationalStateRepository],
    exports: [EducationalStateService],
    controllers: [],
})
export class EducationalStateModule {}
