import { Injectable } from '@nestjs/common';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncUsersFromAzureRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getPersonsForChecking() {
        //write your query to select persons to check in azure
        const result = await getManager().query(
            `
            Select * from azure_temp.MissingAzureUsers
                    
            `,
        );
        return result;
    }

    async updateUser(azureID: string, personID: string, publicEduNumber) {
        // write your query to where to update the results
        const result = await getManager().query(
            `
            UPDATE azure_temp.MissingAzureUsers SET
            AzureID = @0,
            Username = @2
            OUTPUT INSERTED.PersonID as personID
            WHERE
                PersonID = @1
            `,
            [azureID, personID, publicEduNumber],
        );
        return result;
    }
}
