import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { SyncEnrollmentsService } from 'src/models/azure/azure-enrollments/routing/sync-enrollments-service';

@Injectable()
export class DailyRunCliScriptSyncAzureSchoolEnrollmentsJob {
    constructor(private syncEnrollmentsService: SyncEnrollmentsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_DAILY_RUN_CLI_SCRIPT_SYNC_AZURE_SCHOOL_ENROLLMENTS, {
        name: CONSTANTS.JOB_NAME_DAILY_RUN_CLI_SCRIPT_SYNC_AZURE_SCHOOL_ENROLLMENTS,
    })
    @RunOnDeployment({ names: [DeploymentGroup.DAILY_RUN] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`syncSchoolEnrollments`);
                await this.syncEnrollmentsService.syncNotRecievedAzureSchoolEnrollments();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
