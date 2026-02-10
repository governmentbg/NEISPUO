import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureEnrollmentsService } from 'src/models/azure/azure-enrollments/routing/azure-enrollments.service';

@Injectable()
export class RevertAzureEnrollmentsJob {
    constructor(private azureEnrollmentsService: AzureEnrollmentsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_REVERT_AZURE_ENROLLMENTS, { name: CONSTANTS.JOB_NAME_REVERT_AZURE_ENROLLMENTS })
    @RunOnDeployment({ names: [DeploymentGroup.REVERT] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`revertAzureEnrollments`);
                await this.azureEnrollmentsService.revertAzureEnrollments();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
