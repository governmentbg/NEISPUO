import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { Paging } from 'src/common/dto/paging.dto';
import { AzureStudentService } from 'src/models/azure/azure-student/routing/azure-student.service';
import { AzureTeacherService } from 'src/models/azure/azure-teacher/routing/azure-teacher.service';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { SyncPersonalIDsRepository } from './sync-personal-ids.repository';

@Injectable()
export class SyncPersonalIDsService {
    constructor(
        private syncPersonalIDsRepository: SyncPersonalIDsRepository,
        private graphApiService: GraphApiService,
        private azureTeacherService: AzureTeacherService,
        private azureStudentService: AzureStudentService,
    ) {}

    async getAllUsersWithAzureIDs(paging: Paging) {
        return this.syncPersonalIDsRepository.getAllUsersWithAzureIDs(paging);
    }

    async getAllUsersWithAzureIDsCount() {
        return this.syncPersonalIDsRepository.getAllUsersWithAzureIDsCount();
    }

    async updatePersonalIDsInAzure() {
        console.log(`Started: ${Date.now()}`);
        const personalIDsCount = await this.getAllUsersWithAzureIDsCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalCount = personalIDsCount[0].count;
        let currentTotalCount = personalIDsCount[0].count;
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        while (currentTotalCount > 0) {
            let personIDs;
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                personIDs = await this.getAllUsersWithAzureIDs({
                    from: failedTotalCount,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                console.log(e);
                console.log(`Exiting script due to faulure `);
                break;
            }
            // take small portions of data to avoid blocking the app.
            for (const currentElement of personIDs) {
                let result;
                const { personID, sysRoleID } = currentElement;
                currentArrayIndex = syncedTotalCount + failedTotalCount;
                currentTotalCount--;
                try {
                    if (sysRoleID === 6) {
                        result = await this.azureStudentService.updateAzureStudent({ personID });
                    } else {
                        result = await this.azureTeacherService.updateAzureTeacher({ personID });
                    }
                } catch (e) {
                    console.log(e);
                    failedTotalCount++;
                    continue;
                }
                syncedTotalCount++;
                console.log(`Synced user: ${personID}`);
            }
        }
        console.log(`Ended: ${Date.now()}`);
    }
}
