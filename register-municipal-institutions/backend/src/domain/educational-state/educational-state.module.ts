import { Module } from '@nestjs/common';
import { EducationalStateService } from './routes/educational-state/educational-state.service';
import { EducationalStateController } from './routes/educational-state/educational-state.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { EducationalState } from './educational-state.entity';

@Module({
    imports: [TypeOrmModule.forFeature([EducationalState])],
    exports: [TypeOrmModule],

    controllers: [EducationalStateController],
    providers: [EducationalStateService]
})
export class EducationalStateModule {}
