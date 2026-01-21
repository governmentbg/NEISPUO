import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureOrganizationsService } from 'src/models/azure/azure-organizations/routing/azure-organizations.service';

@Injectable()
export class DailyRunCliScriptSyncAzureOrganizaionsJob {
    constructor(private azureOrganizationService: AzureOrganizationsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_DAILY_RUN_CLI_SCRIPT_SYNC_AZURE_SCHOOLS, {
        name: CONSTANTS.JOB_NAME_DAILY_RUN_CLI_SCRIPT_SYNC_AZURE_SCHOOLS,
    })
    @RunOnDeployment({ names: [DeploymentGroup.DAILY_RUN] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`syncUnsyncedInstitutions`);
                await this.azureOrganizationService.syncUnsyncedInstitutions();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
