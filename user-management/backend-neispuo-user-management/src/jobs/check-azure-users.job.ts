import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureUsersService } from 'src/models/azure/azure-users/routing/azure-users.service';

@Injectable()
export class CheckAzureUsersJob {
    constructor(private azureUsersService: AzureUsersService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_CHECK_AZURE_USERS, { name: CONSTANTS.JOB_NAME_CHECK_AZURE_USERS })
    @RunOnDeployment({ names: [DeploymentGroup.CHECK] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                await this.azureUsersService.checkAzureUsers();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
