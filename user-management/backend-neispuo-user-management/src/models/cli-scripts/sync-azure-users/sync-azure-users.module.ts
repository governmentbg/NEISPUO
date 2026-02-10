import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncAzureUsersTaskRunner } from './sync-azure-users.command';
import { SyncUserQuestion } from './sync-azure-users.question';
import { SyncAzureUsersRepository } from './sync-azure-users.repository';
import { SyncAzureUsersService } from './sync-azure-users.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [SyncAzureUsersService, SyncAzureUsersRepository, SyncAzureUsersTaskRunner, SyncUserQuestion],
    exports: [SyncAzureUsersService, SyncAzureUsersRepository],
})
export class SyncAzureUsersModule {}
