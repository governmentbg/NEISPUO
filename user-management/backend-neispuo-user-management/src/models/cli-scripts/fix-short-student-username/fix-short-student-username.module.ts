import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { FixShortStudentUsernameRepository } from './fix-short-student-username.repository';
import { FixShortStudentUsernameTaskRunner } from './fix-short-student-username.command';
import { FixShortStudentUsernameService } from './fix-short-student-username.service';
import { GraphApiModule } from 'src/models/graph-api/graph-api.module';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule, GraphApiModule],
    providers: [FixShortStudentUsernameService, FixShortStudentUsernameRepository, FixShortStudentUsernameTaskRunner],
})
export class FixShortStudentUsernameModule {}
