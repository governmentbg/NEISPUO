import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncInstitutionsAzureIDsTaskRunner } from './sync-institutions-azure-ids.command';
import { SyncInstitutionsAzureIDsRepository } from './sync-institutions-azure-ids.repository';
import { SyncInstitutionsAzureIDsService } from './sync-institutions-azure-ids.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [
        SyncInstitutionsAzureIDsService,
        SyncInstitutionsAzureIDsRepository,
        SyncInstitutionsAzureIDsTaskRunner,
        SyncInstitutionsAzureIDsRepository,
    ],
})
export class SyncInstitutionsAzureIDsModule {}
