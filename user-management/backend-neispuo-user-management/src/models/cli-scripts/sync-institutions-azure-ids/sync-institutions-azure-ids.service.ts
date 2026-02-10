import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { Paging } from 'src/common/dto/paging.dto';
import { AzureStudentService } from 'src/models/azure/azure-student/routing/azure-student.service';
import { AzureTeacherService } from 'src/models/azure/azure-teacher/routing/azure-teacher.service';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { SyncInstitutionsAzureIDsRepository } from './sync-institutions-azure-ids.repository';

@Injectable()
export class SyncInstitutionsAzureIDsService {
    constructor(
        private syncInstitutionsAzureIDsRepository: SyncInstitutionsAzureIDsRepository,
        private graphApiService: GraphApiService,
        private azureTeacherService: AzureTeacherService,
        private azureStudentService: AzureStudentService,
    ) {}

    async getAllInstitutionsAccountsWithoutAzureIDsCount() {
        const result = await this.syncInstitutionsAzureIDsRepository.getAllInstitutionsAccountsWithoutAzureIDsCount();
        return result;
    }

    async getAllInstitutionsAccountsWithoutAzureIDs(paging: Paging) {
        const result = await this.syncInstitutionsAzureIDsRepository.getAllInstitutionsAccountsWithoutAzureIDs(paging);
        return result;
    }

    async updateAzureID(azureID: string, personID: number) {
        return this.syncInstitutionsAzureIDsRepository.updateAzureID(azureID, personID);
    }

    async syncInstitutionAzureIDs() {
        console.log(`Started: ${Date.now()}`);
        // eslint-disable-next-line prettier/prettier
        const institutionsCount = await this.getAllInstitutionsAccountsWithoutAzureIDsCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalCount = institutionsCount;
        let currentTotalCount = institutionsCount;
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;

        // estimated to run for 30 seconds for every 100 schools.
        while (currentTotalCount > 0) {
            let schools;
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                schools = await this.getAllInstitutionsAccountsWithoutAzureIDs({
                    from: failedTotalCount,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                console.log(e);
                console.log(`Exiting script due to faulure `);
                break;
            }
            // take small portions of data to avoid blocking the app.
            for (const currentElement of schools) {
                let result, result2;
                const institutionID = currentElement.institutionID;
                const personID = currentElement.personID;
                currentArrayIndex = syncedTotalCount + failedTotalCount;
                currentTotalCount--;
                try {
                    result = await this.graphApiService.getSchoolInfo(institutionID);
                } catch (e) {
                    console.log(e);
                    continue;
                }
                if (!result?.response) {
                    console.log(
                        `${currentArrayIndex}. Skipping. InstitutionID: ${institutionID} doesnt exist in AZURE.`,
                    );
                    failedTotalCount++;
                    continue;
                }
                const { id } = result.response;
                const azureID = id;
                try {
                    result2 = await this.updateAzureID(azureID, personID);
                } catch (e) {
                    console.log(e);
                    continue;
                }
                // if there is no graph api result dont update DB
                if (!result2?.personID) {
                    console.log(`AzureID not updated for : ${personID}`);
                    failedTotalCount++;
                    continue;
                }
                syncedTotalCount++;
                console.log(`${currentArrayIndex}. Inserted Person: ${personID}`);
            }
        }
        console.log(`Ended: ${Date.now()}`);
    }
}
