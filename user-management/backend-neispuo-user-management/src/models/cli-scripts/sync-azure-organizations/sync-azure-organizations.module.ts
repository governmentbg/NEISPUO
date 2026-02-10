import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncAzureOrganizationsTaskRunner } from './sync-azure-organizations.command';
import { SyncAzureOrganizationsRepository } from './sync-azure-organizations.repository';
import { SyncAzureOrganizationsService } from './sync-azure-organizations.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [SyncAzureOrganizationsService, SyncAzureOrganizationsRepository, SyncAzureOrganizationsTaskRunner],
})
export class SyncAzureOrganizationsModule {}
