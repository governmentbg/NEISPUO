import { Module } from '@nestjs/common';
import { AzureClassesEntity } from 'src/common/entities/azure-classes.entity';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AzureClassesController } from './routing/azure-classes.controller';
import { AzureClassesRepository } from './azure-classes.repository';
import { AzureClassesService } from './routing/azure-classes.service';
import { AzureClassesCrudService } from './routing/azure-classes-crud.service';
import { AzureClassesCrudController } from './routing/azure-classes-crud.controller';
import { SyncClassesService } from './routing/sync-classes-service';
import { ClassesArchiveService } from './routing/classes-archive.service';
import { ClassesErrorService } from './routing/classes-error.service';
import { ClassesRestartService } from './routing/classes-restart.service';

@Module({
    imports: [TypeOrmModule.forFeature([AzureClassesEntity])],
    controllers: [AzureClassesController, AzureClassesCrudController],
    providers: [
        AzureClassesService,
        AzureClassesRepository,
        SyncClassesService,
        AzureClassesCrudService,
        ClassesArchiveService,
        ClassesRestartService,
        ClassesErrorService,
    ],
    exports: [
        AzureClassesService,
        AzureClassesCrudService,
        SyncClassesService,
        ClassesArchiveService,
        ClassesRestartService,
        ClassesErrorService,
    ],
})
export class AzureClassesModule {}
