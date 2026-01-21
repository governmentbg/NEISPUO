import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { EnrollmentsEntity } from 'src/common/entities/azure-enrollments.entity';
import { AzureEnrollmentsRepository } from './azure-enrollments.repository';
import { AzureEnrollmentsCrudController } from './routing/azure-enrollments-crud.controller';
import { AzureEnrollmentsCrudService } from './routing/azure-enrollments-crud.service';
import { AzureEnrollmentsController } from './routing/azure-enrollments.controller';
import { AzureEnrollmentsService } from './routing/azure-enrollments.service';
import { EnrollmentsArchiveService } from './routing/enrollments-archive.service';
import { EnrollmentsErrorService } from './routing/enrollments-error.service';
import { EnrollmentsRestartService } from './routing/enrollments-restart.service';
import { SyncEnrollmentsService } from './routing/sync-enrollments-service';

@Module({
    imports: [TypeOrmModule.forFeature([EnrollmentsEntity])],
    controllers: [AzureEnrollmentsController, AzureEnrollmentsCrudController],
    providers: [
        AzureEnrollmentsService,
        AzureEnrollmentsRepository,
        AzureEnrollmentsCrudService,
        EnrollmentsArchiveService,
        EnrollmentsRestartService,
        EnrollmentsErrorService,
        SyncEnrollmentsService,
    ],
    exports: [
        AzureEnrollmentsService,
        AzureEnrollmentsCrudService,
        EnrollmentsArchiveService,
        EnrollmentsRestartService,
        EnrollmentsErrorService,
        SyncEnrollmentsService,
    ],
})
export class AzureEnrollmentsModule {}
