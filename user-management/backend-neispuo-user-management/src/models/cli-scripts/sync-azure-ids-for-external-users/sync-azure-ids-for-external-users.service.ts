import { Injectable } from '@nestjs/common';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { SyncAzureIDsForExternalUsersRepository } from './sync-azure-ids-for-external-users.repository';

@Injectable()
export class SyncAzureIDsForExternalUsersService {
    constructor(
        private syncAzureIDsForExternalUsersRepository: SyncAzureIDsForExternalUsersRepository,
        private graphApiService: GraphApiService,
    ) {}

    async getExternalUsersWithoutAzureIDsCount() {
        return this.syncAzureIDsForExternalUsersRepository.getExternalUsersWithoutAzureIDsCount();
    }

    async getExternalUsersWithoutAzureIDs() {
        return this.syncAzureIDsForExternalUsersRepository.getExternalUsersWithoutAzureIDs();
    }

    async updateAzureID(azureID: string, mail: string) {
        return this.syncAzureIDsForExternalUsersRepository.updateAzureID(azureID, mail);
    }

    async syncExternalUsersAzureIDsInNEISPUO() {
        console.log(`Started: ${Date.now()}`);
        /* 
            FYI THIS SCRIPT RUNS WITH A SPEED of 100 records per minute
            */
        const mailsCount = await this.getExternalUsersWithoutAzureIDsCount();
        const totalCount = mailsCount;
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        let usernames;
        try {
            console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
            usernames = await this.getExternalUsersWithoutAzureIDs();
        } catch (e) {
            console.log(e);
            console.log(`Exiting script due to failure `);
        }
        // take small portions of data to avoid blocking the app.
        for (const currentElement of usernames) {
            let result;
            const username = currentElement.Username;
            currentArrayIndex = syncedTotalCount + failedTotalCount;

            // if there is no graph api result dont update DB
            if (!result?.response) {
                try {
                    result = await this.graphApiService.callGetExternalUsersInfoByUsernameEndpoint(username);
                } catch (e) {
                    console.log(e);
                }
                if (!result?.data) {
                    console.log(`${currentArrayIndex}. Skipping. No Azure object for mail: ${username}`);
                    failedTotalCount++;
                    continue;
                }
            }

            const { userPrincipalName, id } = result.data;
            let result2;
            try {
                result2 = await this.updateAzureID(id, userPrincipalName);
            } catch (e) {
                console.log(e);
                failedTotalCount++;
                continue;
            }
            syncedTotalCount++;
            console.log(`${currentArrayIndex}. Synced user: ${userPrincipalName}`);
        }
        console.log(`Ended: ${Date.now()}`);
    }
}
