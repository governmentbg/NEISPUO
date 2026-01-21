import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureUsersService } from 'src/models/azure/azure-users/routing/azure-users.service';

@Injectable()
export class SendAzureUsersUpdateJob {
    constructor(private azureUsersService: AzureUsersService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_SEND_AZURE_USERS_UPDATE, { name: CONSTANTS.JOB_NAME_SEND_AZURE_USERS_UPDATE })
    @RunOnDeployment({ names: [DeploymentGroup.SEND] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                await this.azureUsersService.sendAzureUsers(WorkflowType.USER_UPDATE);
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
