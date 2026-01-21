import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureOrganizationsService } from 'src/models/azure/azure-organizations/routing/azure-organizations.service';

@Injectable()
export class RevertAzureOrganizationsJob {
    constructor(private azureOrganizationsService: AzureOrganizationsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_REVERT_AZURE_ORGANIZATIONS, { name: CONSTANTS.JOB_NAME_REVERT_AZURE_ORGANIZATIONS })
    @RunOnDeployment({ names: [DeploymentGroup.REVERT] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`revertAzureOrganizations`);
                await this.azureOrganizationsService.revertAzureOrganizations();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
