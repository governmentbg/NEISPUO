import { Injectable } from '@nestjs/common';
import { Interval } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { JobsService } from 'src/models/jobs/routing/jobs.service';

@Injectable()
export class UpdateJobInterval {
    constructor(private jobsService: JobsService) {}

    @Interval(CONSTANTS.JOB_INTERVAL_NAME_UPDATE_JOB, CONSTANTS.JOB_INTERVAL_JOB_UPDATE)
    async handleCron() {
        /**
         * Always execute this job as it's responsible for the update of the cron and active status of all jobs from the database.
         */
        await this.jobsService.updateJobs();
    }
}
