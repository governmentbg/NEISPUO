import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncExistingPersonsWithAzureIDService } from './sync-existing-persons-with-azureid.service';
import { SyncExistingPersonsWithAzureIDRepository } from './sync-existing-persons-with-azureid.repository';
import { SyncExistingPersonsWithAzureIDTaskRunner } from './sync-existing-persons-with-azureid.command';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [
        SyncExistingPersonsWithAzureIDService,
        SyncExistingPersonsWithAzureIDRepository,
        SyncExistingPersonsWithAzureIDTaskRunner,
        SyncExistingPersonsWithAzureIDRepository,
    ],
})
export class SyncExistingPersonsWithAzureIDModule {}
