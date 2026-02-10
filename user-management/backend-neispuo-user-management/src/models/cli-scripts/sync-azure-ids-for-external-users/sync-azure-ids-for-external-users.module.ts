import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncAzureIDsForExternalUsersTaskRunner } from './sync-azure-ids-for-external-users.command';
import { SyncAzureIDsForExternalUsersService } from './sync-azure-ids-for-external-users.service';
import { SyncAzureIDsForExternalUsersRepository } from './sync-azure-ids-for-external-users.repository';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [
        SyncAzureIDsForExternalUsersService,
        SyncAzureIDsForExternalUsersRepository,
        SyncAzureIDsForExternalUsersTaskRunner,
    ],
})
export class SyncAzureIDsForExternalUsersModule {}
