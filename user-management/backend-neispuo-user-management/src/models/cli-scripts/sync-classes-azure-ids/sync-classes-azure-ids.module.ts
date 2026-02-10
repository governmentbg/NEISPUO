import { Module } from '@nestjs/common';
import { ScheduleModule } from '@nestjs/schedule';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { BearerTokenModule } from 'src/models/bearer-token/bearer-token.module';
import { BearerTokenService } from 'src/models/bearer-token/routing/bearer-token.service';
import { ClassRepository } from 'src/models/class/class.repository';
import { ClassService } from 'src/models/class/routing/class.service';
import { JobsRepository } from 'src/models/jobs/jobs.repository';
import { SyncClassesAzureIDsTaskRunner } from './sync-classes-azure-ids.command';
import { SyncClassesAzureIDsRepository } from './sync-classes-azure-ids.repository';
import { SyncClassesAzureIDsService } from './sync-classes-azure-ids.service';
import { JobsService } from 'src/models/jobs/routing/jobs.service';
import { TokenRefreshInterval } from 'src/intervals/token-refresh.interval';
import { RedisModule } from 'src/models/redis/redis.module';
import { GraphApiModule } from 'src/models/graph-api/graph-api.module';

@Module({
    imports: [DatabaseConfigModule, RedisModule, GraphApiModule, BearerTokenModule, ScheduleModule.forRoot()],
    providers: [
        TokenRefreshInterval,
        JobsService,
        JobsRepository,
        BearerTokenService,
        ClassService,
        ClassRepository,
        SyncClassesAzureIDsRepository,
        SyncClassesAzureIDsTaskRunner,
        SyncClassesAzureIDsService,
    ],
})
export class SyncClassesAzureIDsModule {}
