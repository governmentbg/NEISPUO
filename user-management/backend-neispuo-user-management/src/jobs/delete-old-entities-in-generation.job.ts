import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { EntitiesInGenerationService } from 'src/models/entities-in-generation/routing/entities-in-generation.service';

@Injectable()
export class DeleteOldEntitiesInGenerationJob {
    constructor(private entitiesInGenerationService: EntitiesInGenerationService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_DELETE_ENTITIES_IN_GENERATION, {
        name: CONSTANTS.JOB_NAME_DELETE_ENTITIES_IN_GENERATION,
    })
    @RunOnDeployment({ names: [DeploymentGroup.DAILY_RUN] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`deleteOldEntitiesInGeneration`);
                await this.entitiesInGenerationService.deleteOldEntitiesInGeneration();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
