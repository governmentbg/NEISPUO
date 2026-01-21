import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureClassesService } from 'src/models/azure/azure-classes/routing/azure-classes.service';

@Injectable()
export class SendAzureClassesUpdateJob {
    constructor(private azureClassesService: AzureClassesService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_SEND_AZURE_CLASSES_UPDATE, { name: CONSTANTS.JOB_NAME_SEND_AZURE_CLASSES_UPDATE })
    @RunOnDeployment({ names: [DeploymentGroup.SEND] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                await this.azureClassesService.sendAzureClasses(WorkflowType.CLASS_UPDATE);
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
