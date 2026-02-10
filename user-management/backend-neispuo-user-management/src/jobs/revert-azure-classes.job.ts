import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureClassesService } from 'src/models/azure/azure-classes/routing/azure-classes.service';

@Injectable()
export class RevertAzureClassesJob {
    constructor(private azureClassesService: AzureClassesService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_REVERT_AZURE_CLASSES, { name: CONSTANTS.JOB_NAME_REVERT_AZURE_CLASSES })
    @RunOnDeployment({ names: [DeploymentGroup.REVERT] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`revertAzureClasses`);
                await this.azureClassesService.revertAzureClasses();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
