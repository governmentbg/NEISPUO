import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { PM2Instances } from 'src/common/constants/enum/pm2-instances.enum';
import { AzureStudentService } from 'src/models/azure/azure-student/routing/azure-student.service';

@Injectable()
export class DeleteAzureStudentUsersJob {
    constructor(private azureStudentService: AzureStudentService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_DELETE_UNATTENDING_STUDENT_USERS, {
        name: CONSTANTS.JOB_NAME_DELETE_UNATTENDING_STUDENT_USERS,
    })
    async handleCron() {
        // do nothing if nodejs is not first node of pm2 cluster
        const isThirdPm2Instance = process.env.NODE_APP_INSTANCE === PM2Instances.THIRD;
        const isProd = process.env.APP_ENV !== 'development';
        if (isProd && !isThirdPm2Instance) {
            return;
        }

        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                await this.azureStudentService.getUnattendingStudentsForDelete();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
