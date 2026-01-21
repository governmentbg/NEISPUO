import { Injectable } from '@nestjs/common';
import { GraphApiUserTypeEnum } from 'src/common/constants/enum/graph-api-user-type';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { SyncUsersFromAzureRepository } from './sync-users-from-azure.repository';

@Injectable()
export class SyncUsersFromAzureService {
    constructor(
        private syncUsersFromAzureRepository: SyncUsersFromAzureRepository,
        private graphApiService: GraphApiService,
    ) {}

    async getPersonsForChecking() {
        return this.syncUsersFromAzureRepository.getPersonsForChecking();
    }

    async updateAzureID(azureID: string, personID: string, publicEduNumber: string) {
        return this.syncUsersFromAzureRepository.updateUser(azureID, personID, publicEduNumber);
    }

    async syncUsersFromAzure() {
        console.log(`Started: ${Date.now()}`);
        /* 
            FYI THIS SCRIPT RUNS WITH A SPEED of 100 records per minute
            */
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        let users;
        try {
            users = await this.getPersonsForChecking();
        } catch (e) {
            console.log(e);
            console.log(`Exiting script due to faulure `);
        }
        // take small portions of data to avoid blocking the app.
        for (const currentElement of users) {
            currentArrayIndex++;
            let result;
            try {
                result = await this.graphApiService.callUserInfoByPersonalIDEndpoint(
                    currentElement?.UserID,
                    GraphApiUserTypeEnum.ALL,
                );
            } catch (e) {
                console.log(e);
            }
            if (!result?.response) {
                console.log(
                    `${currentArrayIndex}. Skipping. No Azure object for personalID: ${currentElement?.UserID}`,
                );
                failedTotalCount++;
                continue;
            }
            const { mailNickname, id } = result.response;
            const azureID = id;
            let result2;
            try {
                result2 = await this.updateAzureID(azureID, currentElement?.PersonID, mailNickname);
            } catch (e) {
                console.log(e);
                failedTotalCount++;
                continue;
            }
            syncedTotalCount++;
            console.log(`${currentArrayIndex}. Synced user: ${mailNickname}`);
        }
        console.log(`Synced: ${syncedTotalCount}`);
        console.log(`Failed: ${failedTotalCount}`);
        console.log(`Ended: ${Date.now()}`);
    }
}
