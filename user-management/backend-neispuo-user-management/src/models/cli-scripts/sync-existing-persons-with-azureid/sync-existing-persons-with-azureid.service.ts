import { Injectable, Logger } from '@nestjs/common';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { SyncExistingPersonsWithAzureIDRepository } from './sync-existing-persons-with-azureid.repository';

@Injectable()
export class SyncExistingPersonsWithAzureIDService {
    constructor(
        private syncExistingPersonsWithAzureIDRepository: SyncExistingPersonsWithAzureIDRepository,
        private graphApiService: GraphApiService,
    ) {}

    async getAllUsersWithoutSysUserCount() {
        const result = await this.syncExistingPersonsWithAzureIDRepository.getAllUsersWithoutSysUserCount();
        return result;
    }

    async getAllUsersWithoutSysUser() {
        const result = await this.syncExistingPersonsWithAzureIDRepository.getAllUsersWithoutSysUser();
        return result;
    }

    async addSysUser(username: string, personID: number) {
        return this.syncExistingPersonsWithAzureIDRepository.addSysUser(username, personID);
    }

    async syncSysUsers() {
        console.log(`Started: ${Date.now()}`);
        // eslint-disable-next-line prettier/prettier
        const usersCount = await this.getAllUsersWithoutSysUserCount();

        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;

        const users = await this.getAllUsersWithoutSysUser();

        for (const currentElement of users) {
            let result, result2;

            const azureID = currentElement.azureID;
            const personID = currentElement.personID;
            currentArrayIndex = syncedTotalCount + failedTotalCount;
            try {
                result = await this.graphApiService.getUserInfoByAzureID(azureID);
            } catch (e) {
                console.log(e);
                continue;
            }
            if (!result?.response) {
                console.log(`${currentArrayIndex}. Skipping. AzureID: ${azureID} doesnt exist in AZURE.`);
                failedTotalCount++;
                continue;
            }
            const { mail } = result.response;
            try {
                result2 = await this.addSysUser(mail, personID);
            } catch (e) {
                console.log(e);
                continue;
            }
            // if there is no graph api result dont update DB
            if (!result2?.personID) {
                console.log(`SysUser not created for : ${personID}`);
                failedTotalCount++;
                continue;
            }
            syncedTotalCount++;
            console.log(`${currentArrayIndex}. Inserted SysUser: ${personID}`);
        }
        Logger.log(`Synced: ${syncedTotalCount} from Total: ${usersCount}`);
        Logger.log(`Ended: ${Date.now()}`);
    }
}
