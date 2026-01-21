import { Module } from '@nestjs/common';
import { ScheduleModule } from '@nestjs/schedule';
import { UpdateRPersonalService } from './cron-jobs/update-rpersonal.service';
import { UpdateRInstitutionsService } from './cron-jobs/update-r-institutions.service';
import { UpdateRStudentsDetailsService } from './cron-jobs/update-rstudents-details.service';
import { UpdateRStudentsService } from './cron-jobs/update-rstudents.service';

@Module({
  controllers: [],
  imports: [ScheduleModule.forRoot()],
  providers: [
    UpdateRPersonalService,
    UpdateRInstitutionsService,
    UpdateRStudentsDetailsService,
    UpdateRStudentsService,
  ],
  exports: [],
})
export class CronModule {}
