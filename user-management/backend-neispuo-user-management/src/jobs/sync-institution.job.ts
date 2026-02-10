import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureOrganizationsService } from 'src/models/azure/azure-organizations/routing/azure-organizations.service';

@Injectable()
export class SyncInstitutionJob {
    constructor(private azureOrganizationsService: AzureOrganizationsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_UPDATE_INSTITUTIONS, { name: CONSTANTS.JOB_NAME_UPDATE_INSTITUTIONS })
    @RunOnDeployment({ names: [DeploymentGroup.SYNC] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`syncInstitutions`);
                await this.azureOrganizationsService.syncInstitutions();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
