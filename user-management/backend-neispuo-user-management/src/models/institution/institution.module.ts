import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { InstitutionsTableEntity } from 'src/common/entities/institutions-table-view.entity';
import { InstitutionController } from './routing/institution.controller';
import { InstitutionRepository } from './institution.repository';
import { InstitutionService } from './routing/institution.service';

@Module({
    imports: [TypeOrmModule.forFeature([InstitutionRepository, InstitutionsTableEntity])],
    providers: [InstitutionService],
    exports: [InstitutionService],
    controllers: [InstitutionController],
})
export class InstitutionModule {}
