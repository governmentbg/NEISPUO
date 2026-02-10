import { Module } from '@nestjs/common';
import { SyncAzureUsersRepository } from './sync-azure-users.repository';
import { SyncAzureUsersService } from './sync-azure-users.service';

@Module({
    providers: [SyncAzureUsersService, SyncAzureUsersRepository],
    exports: [SyncAzureUsersService, SyncAzureUsersRepository],
})
export class DailySyncAzureUsersModule {}
