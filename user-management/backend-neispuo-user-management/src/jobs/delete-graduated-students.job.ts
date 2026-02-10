import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { StudentService } from 'src/models/student/routing/student.service';

@Injectable()
export class DeleteGraduatedStudentsJob {
    constructor(private studentService: StudentService) {}

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_DELETE_GRADUATED_STUDENTS, {
        name: CONSTANTS.JOB_NAME_DELETE_GRADUATED_STUDENTS,
    })
    @RunOnDeployment({ names: [DeploymentGroup.OTHER] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                await this.studentService.deleteGraduatedStudents();
            } catch (e) {
                this.previousJobHasFinished = true;
                throw e;
            }
            this.previousJobHasFinished = true;
        }
    }
}
