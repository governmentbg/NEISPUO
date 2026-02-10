import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { AzureEnrollmentsService } from 'src/models/azure/azure-enrollments/routing/azure-enrollments.service';

@Injectable()
export class SendAzureEnrollmentsStudentToClassCreateJob {
    constructor(private azureEnrollmentsService: AzureEnrollmentsService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_SEND_AZURE_ENROLLMENTS_STUDENT_TO_CLASS_CREATE, {
        name: CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_CLASS_CREATE,
    })
    @RunOnDeployment({ names: [DeploymentGroup.SEND] })
    async handleCron() {
        try {
            await this.azureEnrollmentsService.sendAzureEnrollmentsForSending(
                WorkflowType.ENROLLMENT_CLASS_CREATE,
                UserRoleType.STUDENT,
            );
        } catch (e) {
            this.previousJobHasFinished = true;
            throw e;
        }
    }
}
