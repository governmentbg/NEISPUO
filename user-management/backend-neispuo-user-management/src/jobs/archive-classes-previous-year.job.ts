import { Injectable, Logger } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { DeploymentGroup } from 'src/common/constants/enum/deployment-group.enum';
import { RunOnDeployment } from 'src/common/decorators/run-on-deployment.decorator';
import { SchoolYearService } from 'src/common/services/school-year/school-year.service';
import { ArchivableEntity } from 'src/models/azure/azure-archive/enums/archivable-entity.enum';
import { ArchivationMode } from 'src/models/azure/azure-archive/enums/archivation-mode.enum';
import { AzureArchiveService } from 'src/models/azure/azure-archive/routing/azure-archive.service';

@Injectable()
export class ArchiveClassesPreviousYearJob {
    constructor(private azureArchiveService: AzureArchiveService) {}

    private readonly logger: any = new Logger(ArchiveClassesPreviousYearJob.name);

    private previousJobHasFinished = true;

    @Cron(CONSTANTS.JOB_CRON_ARCHIVE_CLASSES_PREVIOUS_YEARS, {
        name: CONSTANTS.JOB_NAME_ARCHIVE_CLASSES_PREVIOUS_YEARS,
    })
    @RunOnDeployment({ names: [DeploymentGroup.ARCHIVE] })
    async handleCron() {
        if (this.previousJobHasFinished) {
            this.previousJobHasFinished = false;
            try {
                this.logger.log(`Processing`);
                await this.azureArchiveService.archiveEntities({
                    entity: ArchivableEntity.CLASSES,
                    mode: ArchivationMode.ARCHIVED_TO_PREVIOUS_YEARS,
                    upToDate: SchoolYearService.getPreviousSchoolYearEndDate(),
                    logger: this.logger,
                });
            } finally {
                this.previousJobHasFinished = true;
            }
        }
    }
}
