import { Module } from '@nestjs/common';
import { ScheduleModule } from '@nestjs/schedule';
import { EmailTemplateModule } from '../email-template/email-template.module';
import { JobsRepository } from './jobs.repository';
import { JobsService } from './jobs.service';

@Module({
  imports: [ScheduleModule.forRoot(), EmailTemplateModule],
  providers: [JobsService, JobsRepository],
  exports: [JobsService],
})
export class JobsModule {}
