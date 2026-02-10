import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncAzureEnrollmentsTaskRunner } from './sync-azure-enrollments.command';
import { SyncAzureEnrollmentsRepository } from './sync-azure-enrollments.repository';
import { SyncAzureEnrollmentsService } from './sync-azure-enrollments.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [SyncAzureEnrollmentsService, SyncAzureEnrollmentsRepository, SyncAzureEnrollmentsTaskRunner],
})
export class SyncAzureEnrollmentsModule {}
