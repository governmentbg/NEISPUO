import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncAzureAccountantsTaskRunner } from './sync-azure-accountants.command';
import { SyncAzureAccountantsRepository } from './sync-azure-accountants.repository';
import { SyncAzureAccountantsService } from './sync-azure-accountants.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [SyncAzureAccountantsService, SyncAzureAccountantsRepository, SyncAzureAccountantsTaskRunner],
})
export class SyncAzureAccountantsModule {}
