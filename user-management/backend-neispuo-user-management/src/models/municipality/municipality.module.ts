import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { MunicipalityRepository } from './municipality.repository';
import { MunicipalityService } from './routing/municipality.service';
import { MunicipalityAzureService } from './routing/municipality-azure.service';
import { MunicipalityAzureController } from './routing/municipality-azure.controller';

@Module({
    imports: [TypeOrmModule.forFeature([MunicipalityRepository, ExternalUserViewEntity])],
    providers: [MunicipalityService, MunicipalityAzureService],
    exports: [MunicipalityService, MunicipalityAzureService],
    controllers: [MunicipalityAzureController],
})
export class MunicipalityModule {}
