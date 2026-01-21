import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureUsersService } from 'src/models/azure/azure-users/routing/azure-users.service';

@Injectable()
export class SyncUserJob {
    constructor(private azureUsersService: AzureUsersService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_CREATE_USERS, { name: CONSTANTS.JOB_NAME_CREATE_USERS })
    @RunOnDeployment({ names: [DeploymentGroup.SYNC] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`syncUsers`);
                await this.azureUsersService.syncUsers();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
