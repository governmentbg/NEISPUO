import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AzureOrganizationsEntity } from 'src/common/entities/azure-organizations.entity';
import { AzureOrganizationsRepository } from './azure-organizations.repository';
import { AzureOrganizationsCrudController } from './routing/azure-organizations-crud.controller';
import { AzureOrganizationsCrudService } from './routing/azure-organizations-crud.service';
import { AzureOrganizationsController } from './routing/azure-organizations.controller';
import { AzureOrganizationsService } from './routing/azure-organizations.service';
import { OrganizationsArchiveService } from './routing/organizations-archive.service';
import { OrganizationsErrorService } from './routing/organizations-error.service';
import { OrganizationsRestartService } from './routing/organizations-restart.service';

@Module({
    imports: [TypeOrmModule.forFeature([AzureOrganizationsEntity])],
    controllers: [AzureOrganizationsController, AzureOrganizationsCrudController],
    providers: [
        AzureOrganizationsService,
        AzureOrganizationsRepository,
        AzureOrganizationsCrudService,
        OrganizationsRestartService,
        OrganizationsArchiveService,
        OrganizationsErrorService,
    ],
    exports: [
        AzureOrganizationsService,
        AzureOrganizationsCrudService,
        OrganizationsRestartService,
        OrganizationsArchiveService,
        OrganizationsErrorService,
    ],
})
export class AzureOrganizationsModule {}
