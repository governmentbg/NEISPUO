import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureEnrollmentsService } from 'src/models/azure/azure-enrollments/routing/azure-enrollments.service';

@Injectable()
export class CheckAzureEnrollmentsToClassJob {
    constructor(private azureEnrollmentsService: AzureEnrollmentsService) {}

    @Cron(CONSTANTS.JOB_CRON_CHECK_AZURE_ENROLLMENTS_TO_CLASS, {
        name: CONSTANTS.JOB_NAME_CHECK_AZURE_ENROLLMENTS_TO_CLASS,
    })
    @RunOnDeployment({ names: [DeploymentGroup.CHECK_ENROLLMENTS] })
    async handleCron() {
        try {
            await this.azureEnrollmentsService.checkAzureEnrollmentsToClass();
        } catch (e) {
            throw e;
        }
    }
}
