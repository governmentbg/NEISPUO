import { Injectable } from '@nestjs/common';
import { Interval } from '@nestjs/schedule';
import { JobsService } from 'src/features/jobs/jobs.service';
import { CONSTANTS } from 'src/shared/constants';

@Injectable()
export class UpdateJobInterval {
  constructor(private jobsService: JobsService) {}

  /**
   * This interval runs to synchronize the application's
   * job configurations (cron schedules and active flags) with the
   * latest data from the database.
   */
  @Interval(
    CONSTANTS.JOB_INTERVAL_NAME_UPDATE_JOB,
    CONSTANTS.JOB_INTERVAL_JOB_UPDATE,
  )
  async handleCron() {
    console.log(`Running job update interval at ${new Date(Date.now())}`);
    await this.jobsService.updateJobs();
  }
}
