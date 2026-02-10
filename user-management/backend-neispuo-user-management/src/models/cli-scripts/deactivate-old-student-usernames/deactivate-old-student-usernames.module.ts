import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { GraphApiModule } from 'src/models/graph-api/graph-api.module';
import { DeactivateOldStudentUsernamesService } from './deactivate-old-student-usernames.service';
import { DeactivateOldStudentUsernamesRepository } from './deactivate-old-student-usernames.repository';
import { DeactivateOldStudentUsernamesTaskRunner } from './deactivate-old-student-usernames.command';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule, GraphApiModule],
    providers: [
        DeactivateOldStudentUsernamesService,
        DeactivateOldStudentUsernamesRepository,
        DeactivateOldStudentUsernamesTaskRunner,
    ],
})
export class DeactivateOldStudentUsernamesModule {}
