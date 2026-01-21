import { Injectable, OnModuleInit } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from '../constants';
import { LoggerService } from 'src/shared/services/logger/logger.service';
import { getManager } from 'typeorm';
import { AgreggatedResultsJobService } from '@domain/aggregated-results/aggregated-results-job/aggregated-results-job.service';


@Injectable()
export class SetCampaignsisActiveStatus implements OnModuleInit {

    constructor(
        private loggerService: LoggerService,
        private aggregatedResultsJob: AgreggatedResultsJobService
    ) {

    }

    onModuleInit() {
        this.loggerService.jobsLogger.info('[SetCampaignsIsActiveStatus] INIT');
    }

    @Cron(CONSTANTS.SET_CAMPAIGNS_IS_ACTIVE_STATUS, { name: CONSTANTS.JOB_NAME_SET_CAMPAIGNS_IS_ACTIVE_STATUS })
    async handleCron() {
        await this.aggregatedResultsJob.generateAggregatedDataForExpiredCampaigns();
        this.loggerService.jobsLogger.info('[AggregateResultsJobs] Called this.generateAggregatedDataForExpiredCampaigns()');
        this.setCampaignsToActive();
        this.loggerService.jobsLogger.info('[CampaignStatusJob] Called this.setCampaignsToActive()');
    }

    public async setCampaignsToActive() {
        const entityManager = getManager();
        await entityManager.query(`
            UPDATE tools_assessment.campaigns 
            SET isActive = 1
            WHERE endDate > GETDATE()
            AND startDate < GETDATE()
            AND isActive = 0
        `);
    }
}