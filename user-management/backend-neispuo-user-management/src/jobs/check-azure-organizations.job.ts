import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureOrganizationsService } from 'src/models/azure/azure-organizations/routing/azure-organizations.service';

@Injectable()
export class CheckAzureOrganizationsJob {
    constructor(private azureOrganizationsService: AzureOrganizationsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_CHECK_AZURE_ORGANIZATIONS, { name: CONSTANTS.JOB_NAME_CHECK_AZURE_ORGANIZATIONS })
    @RunOnDeployment({ names: [DeploymentGroup.CHECK] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                await this.azureOrganizationsService.checkAzureOrganizations();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
