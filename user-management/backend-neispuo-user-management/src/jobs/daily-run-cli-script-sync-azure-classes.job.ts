import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { SyncClassesService } from 'src/models/azure/azure-classes/routing/sync-classes-service';

@Injectable()
export class DailyRunCliScriptSyncAzureClassesJob {
    constructor(private syncClassesService: SyncClassesService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_DAILY_RUN_CLI_SCRIPT_SYNC_AZURE_CLASSES, {
        name: CONSTANTS.JOB_NAME_DAILY_RUN_CLI_SCRIPT_SYNC_AZURE_CLASSES,
    })
    @RunOnDeployment({ names: [DeploymentGroup.DAILY_RUN] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`syncUnsyncedClasses`);
                await this.syncClassesService.syncUnsyncedClasses();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
