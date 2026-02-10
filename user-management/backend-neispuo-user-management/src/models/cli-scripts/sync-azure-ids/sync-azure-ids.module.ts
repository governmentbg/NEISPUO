import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncAzureIDsTaskRunner } from './sync-azure-ids.command';
import { SyncAzureIDsRepository } from './sync-azure-ids.repository';
import { SyncAzureIDsService } from './sync-azure-ids.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [SyncAzureIDsService, SyncAzureIDsRepository, SyncAzureIDsTaskRunner],
})
export class SyncAzureIDsModule {}
