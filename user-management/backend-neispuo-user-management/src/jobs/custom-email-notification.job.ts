import { Injectable, Logger } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import axios from 'axios';
import * as https from 'https';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';

@Injectable()
export class CustomEmailNotificationJob {
    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_CUSTOM_EMAIL_NOTIFICATION, {
        name: CONSTANTS.JOB_NAME_CUSTOM_EMAIL_NOTIFICATION,
        timeZone: 'Europe/Sofia',
    })
    @RunOnDeployment({ names: [DeploymentGroup.OTHER] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                Logger.log('Sending custom emails');
                await this.sendCustomEmails();
                Logger.log('Custom emails sent');
            } catch (error) {
                Logger.error(`Error sending email: ${error?.message}`);
            } finally {
                this.previousJobHasFinished = true;
            }
        }
    }

    async sendCustomEmails() {
        const httpsAgent = new https.Agent({ rejectUnauthorized: false });

        const httpOptions = {
            headers: {
                'Content-Type': 'application/json',
                'x-api-key': process.env.INTERNAL_API_KEY,
            },
            httpsAgent: httpsAgent,
        };

        return axios.get(`${process.env.MAIN_PORTAL_SERVER_URL}${process.env.SEND_CUSTOM_EMAILS_URI}`, httpOptions);
    }
}
