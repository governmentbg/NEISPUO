import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { GenerateStudentCodesTaskRunner } from './generate-student-codes.command';
import { GenerateStudentCodesRepository } from './generate-student-codes.repository';
import { GenerateStudentCodesService } from './generate-student-codes.service';

@Module({
    imports: [DatabaseConfigModule, IntervalsModule, GlobalModule],
    providers: [GenerateStudentCodesService, GenerateStudentCodesRepository, GenerateStudentCodesTaskRunner],
})
export class GenerateStudentCodesModule {}
