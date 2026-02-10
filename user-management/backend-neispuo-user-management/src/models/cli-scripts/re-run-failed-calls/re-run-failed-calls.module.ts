import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { ReRunFailedCallsTaskRunner } from './re-run-failed-calls.command';
import { ReRunFailedCallsRepository } from './re-run-failed-calls.repository';
import { ReRunFailedCallsService } from './re-run-failed-calls.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [ReRunFailedCallsService, ReRunFailedCallsRepository, ReRunFailedCallsTaskRunner],
})
export class ReRunFailedCallsModule {}
