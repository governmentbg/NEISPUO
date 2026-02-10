import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { ErrorNotificationService } from 'src/models/error-notification/routing/error-notification.service';

@Injectable()
export class ErrorEmailNotificationJob {
    constructor(private errorNotificationService: ErrorNotificationService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_ERROR_EMAIL_NOTIFICATION, {
        name: CONSTANTS.JOB_NAME_ERROR_EMAIL_NOTIFICATION,
    })
    @RunOnDeployment({ names: [DeploymentGroup.OTHER] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                console.log(`sendEmail`);
                await this.errorNotificationService.sendEmail();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
