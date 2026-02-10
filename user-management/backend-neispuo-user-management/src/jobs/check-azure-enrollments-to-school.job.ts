import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureEnrollmentsService } from 'src/models/azure/azure-enrollments/routing/azure-enrollments.service';

@Injectable()
export class CheckAzureEnrollmentsToSchoolJob {
    constructor(private azureEnrollmentsService: AzureEnrollmentsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_CHECK_AZURE_ENROLLMENTS_TO_SCHOOL, {
        name: CONSTANTS.JOB_NAME_CHECK_AZURE_ENROLLMENTS_TO_SCHOOL,
    })
    @RunOnDeployment({ names: [DeploymentGroup.CHECK_ENROLLMENTS] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                await this.azureEnrollmentsService.checkAzureEnrollmentsToSchool();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
