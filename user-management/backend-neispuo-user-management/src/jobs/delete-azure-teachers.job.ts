import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { PM2Instances } from 'src/common/constants/enum/pm2-instances.enum';
import { AzureTeacherService } from 'src/models/azure/azure-teacher/routing/azure-teacher.service';

@Injectable()
export class DeleteAzureTeachersJob {
    constructor(private azureTeacherService: AzureTeacherService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_DELETE_UNEMPLOYED_TEACHERS, { name: CONSTANTS.JOB_NAME_DELETE_UNEMPLOYED_TEACHERS })
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
                await this.azureTeacherService.getUnemployedTeachersForDelete();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
