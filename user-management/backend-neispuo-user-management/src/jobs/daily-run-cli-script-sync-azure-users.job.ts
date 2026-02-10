import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { SyncAzureUsersService } from 'src/models/cli-scripts/sync-azure-users/sync-azure-users.service';

@Injectable()
export class DailyRunCliScriptSyncAzureUsersJob {
    constructor(private dailySyncUsersService: SyncAzureUsersService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_DAILY_RUN_CLI_SCRIPT_SYNC_AZURE_USERS, {
        name: CONSTANTS.JOB_NAME_DAILY_RUN_CLI_SCRIPT_SYNC_AZURE_USERS,
    })
    @RunOnDeployment({ names: [DeploymentGroup.DAILY_RUN] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`createTeachers createStudents`);
                await this.dailySyncUsersService.createTeachers();
                await this.dailySyncUsersService.createStudents();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
