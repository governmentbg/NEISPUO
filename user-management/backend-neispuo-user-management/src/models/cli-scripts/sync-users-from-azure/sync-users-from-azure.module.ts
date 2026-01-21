import { Module } from '@nestjs/common';
import { ScheduleModule } from '@nestjs/schedule';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { TokenRefreshInterval } from 'src/intervals/token-refresh.interval';
import { BearerTokenModule } from 'src/models/bearer-token/bearer-token.module';
import { BearerTokenService } from 'src/models/bearer-token/routing/bearer-token.service';
import { ClassRepository } from 'src/models/class/class.repository';
import { ClassService } from 'src/models/class/routing/class.service';
import { GraphApiModule } from 'src/models/graph-api/graph-api.module';
import { JobsRepository } from 'src/models/jobs/jobs.repository';
import { JobsService } from 'src/models/jobs/routing/jobs.service';
import { RedisModule } from 'src/models/redis/redis.module';
import { SyncOldUsersTaskRunner } from './sync-users-from-azure.command';
import { SyncUsersFromAzureRepository } from './sync-users-from-azure.repository';
import { SyncUsersFromAzureService } from './sync-users-from-azure.service';

@Module({
    imports: [DatabaseConfigModule, RedisModule, GraphApiModule, BearerTokenModule, ScheduleModule.forRoot()],
    providers: [
        SyncUsersFromAzureRepository,
        SyncUsersFromAzureService,
        SyncOldUsersTaskRunner,
        TokenRefreshInterval,
        JobsService,
        JobsRepository,
        BearerTokenService,
        ClassService,
        ClassRepository,
    ],
})
export class SyncOldUsersModule {}
