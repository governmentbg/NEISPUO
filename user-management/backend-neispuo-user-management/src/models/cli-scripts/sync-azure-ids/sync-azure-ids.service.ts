import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { GraphApiUserTypeEnum } from 'src/common/constants/enum/graph-api-user-type';
import { Paging } from 'src/common/dto/paging.dto';
import { StripZeroesService } from 'src/common/services/strip-zeroes/strip-zeroes.service';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { SyncAzureIDsRepository } from './sync-azure-ids.repository';

@Injectable()
export class SyncAzureIDsService {
    constructor(private syncAzureIDsRepository: SyncAzureIDsRepository, private graphApiService: GraphApiService) {}

    async getAllUsersWithoutAzureIDsCount() {
        return this.syncAzureIDsRepository.getAllUsersWithoutAzureIDsCount();
    }

    async getAllUsersWithoutAzureIDs(paging: Paging) {
        return this.syncAzureIDsRepository.getAllUsersWithoutAzureIDs(paging);
    }

    async updateAzureID(azureID: string, personID: string) {
        return this.syncAzureIDsRepository.updateAzureID(azureID, personID);
    }

    async syncAzureIDsInNEISPUO() {
        console.log(`Started: ${Date.now()}`);
        /* 
            FYI THIS SCRIPT RUNS WITH A SPEED of 100 records per minute
            */
        const personalIDsCount = await this.getAllUsersWithoutAzureIDsCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalCount = personalIDsCount[0].count;
        let currentTotalCount = personalIDsCount[0].count;
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        while (currentTotalCount > 0) {
            let personalIDs;
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                personalIDs = await this.getAllUsersWithoutAzureIDs({
                    from: failedTotalCount,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                console.log(e);
                console.log(`Exiting script due to faulure `);
                break;
            }
            // take small portions of data to avoid blocking the app.
            for (const currentElement of personalIDs) {
                let result;
                const personalID = currentElement.PersonalID;
                currentArrayIndex = syncedTotalCount + failedTotalCount;
                currentTotalCount--;
                const strippedPersonalID = StripZeroesService.transform(personalID);
                try {
                    result = await this.graphApiService.callUserInfoByPersonalIDEndpoint(
                        strippedPersonalID,
                        GraphApiUserTypeEnum.ALL,
                    );
                } catch (e) {
                    failedTotalCount++;
                    continue;
                }
                // if there is no graph api result dont update DB
                if (!result?.response) {
                    try {
                        result = await this.graphApiService.callUserInfoByPersonalIDEndpoint(
                            personalID,
                            GraphApiUserTypeEnum.ALL,
                        );
                    } catch (e) {
                        console.log(e);
                    }
                    if (!result?.response) {
                        console.log(`${currentArrayIndex}. Skipping. No Azure object for personalID: ${personalID}`);
                        failedTotalCount++;
                        continue;
                    }
                }
                const { mailNickname, id } = result.response;
                const azureID = id;
                let result2;
                try {
                    result2 = await this.updateAzureID(azureID, personalID);
                } catch (e) {
                    console.log(e);
                    failedTotalCount++;
                    continue;
                }
                syncedTotalCount++;
                console.log(`${currentArrayIndex}. Synced user: ${mailNickname}`);
            }
        }
        console.log(`Ended: ${Date.now()}`);
    }
}
