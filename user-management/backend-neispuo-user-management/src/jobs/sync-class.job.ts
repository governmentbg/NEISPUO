import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureClassesService } from 'src/models/azure/azure-classes/routing/azure-classes.service';

@Injectable()
export class SyncClassJob {
    constructor(private azureClassesService: AzureClassesService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_UPDATE_CLASSES, { name: CONSTANTS.JOB_NAME_UPDATE_CLASSES })
    @RunOnDeployment({ names: [DeploymentGroup.SYNC] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`syncClasses`);
                await this.azureClassesService.syncClasses();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
