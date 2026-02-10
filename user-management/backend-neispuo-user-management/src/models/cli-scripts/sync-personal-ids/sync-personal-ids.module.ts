import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncPersonalIDsTaskRunner } from './sync-personal-ids.command';
import { SyncPersonalIDsRepository } from './sync-personal-ids.repository';
import { SyncPersonalIDsService } from './sync-personal-ids.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [SyncPersonalIDsRepository, SyncPersonalIDsTaskRunner, SyncPersonalIDsService],
})
export class SyncPersonalIDsModule {}
