import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { EnrollmentsArchiveService } from 'src/models/azure/azure-enrollments/routing/enrollments-archive.service';

@Injectable()
export class ArchiveNotStartedWorkflowsJob {
    constructor(private enrollmentsArchiveService: EnrollmentsArchiveService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_ARCHIVE_NOT_STARTED_JOBS_WORKFLOWS, {
        name: CONSTANTS.JOB_NAME_ARCHIVE_NOT_STARTED_ENROLLMENTS,
    })
    @RunOnDeployment({ names: [DeploymentGroup.ARCHIVE] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`archiveNotStartedWorkflows`);
                await this.enrollmentsArchiveService.archiveNotStartedWorkflows();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
