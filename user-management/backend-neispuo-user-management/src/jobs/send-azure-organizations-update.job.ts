import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureOrganizationsService } from 'src/models/azure/azure-organizations/routing/azure-organizations.service';

@Injectable()
export class SendAzureOrganizationsUpdateJob {
    constructor(private azureOrganizationsService: AzureOrganizationsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_SEND_AZURE_ORGANIZATIONS_UPDATE, {
        name: CONSTANTS.JOB_NAME_SEND_AZURE_ORGANIZATIONS_UPDATE,
    })
    @RunOnDeployment({ names: [DeploymentGroup.SEND] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                await this.azureOrganizationsService.sendAzureOrganizations(WorkflowType.SCHOOL_UPDATE);
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
