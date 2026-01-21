import { Injectable } from '@nestjs/common';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { AzureUsersMapper } from 'src/common/mappers/azure-users.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class FixShortStudentUsernameRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async getAllStudents() {
        const result = await this.entityManager.query(
            `SELECT
                "user".Username    as "username",
                "user".SysUserID   as "sysUserID",
                "user".IsAzureUser as "isAzureUser",
                "user".PersonID    as "personID",
                "person".PersonalID as "personalID",
                "person".FirstName as "firstName",
                "person".MiddleName  as "middleName",
                "person".LastName  as "surname",
                "person".AzureID as "azureID",
                "person".PublicEduNumber as "userID"
            FROM "core"."SysUser" "user" 
            LEFT JOIN "core"."Person" "person" ON
                    "person"."personID" = "user"."personID"
            WHERE 
            (
                "user"."username" LIKE '[A-Za-z][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%'
                OR 
                "user".Username LIKE '[A-Za-z][-._][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%'
            )
            AND "user"."username" LIKE '%@edu.mon.bg'
            AND "user"."DeletedOn" IS NULL`,
        );

        const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(result);

        return transformedResult;
    }

    async createNewUsernameRecord(
        dto: AzureUsersResponseDTO,
        newUsername: string,
        newEdu: string,
        message: string,
        manager?: EntityManager,
    ) {
        const { username, personID, azureID, userID } = dto;
        const entityManager = manager ? manager : this.entityManager;
        const result = await entityManager
            .query(
                `INSERT INTO azure_temp.NewUsernames (old_username, person_id, azure_id, new_username, new_edu, old_edu ,message) 
                VALUES ('${username}', '${personID}', '${azureID}', '${newUsername}', '${newEdu}', '${userID}','${message}');`,
            )
            .catch((err) => {
                console.error(err);
            });

        return result;
    }

    async createNewUser(dto: AzureUsersResponseDTO, newUsername: string, newEdu: string, manager: EntityManager) {
        const { personID } = dto;
        const entityManager = manager ? manager : this.entityManager;

        const usernameAlreadyExist = await entityManager
            .query(`SELECT Username FROM azure_temp.GeneratedUsers WHERE Username = '${newUsername}'`)
            .catch(async (err) => {
                console.error(err);
            });

        if (usernameAlreadyExist.length == 0) {
            const newUser = await entityManager
                .query(
                    `INSERT INTO core.SysUser (username, PersonId, IsAzureUser, isAzureSynced)
                    VALUES ('${newUsername}', '${personID}', 1, 1);`,
                )
                .catch(async (err) => {
                    console.error(err);
                });

            const generatedUser = await this.entityManager
                .query(
                    `INSERT INTO azure_temp.GeneratedUsers
                    (Username, Occurrences, CreatedOn)
                    VALUES('${newUsername}', '1', GETDATE());`,
                )
                .catch(async (err) => {
                    console.log(err);
                });

            return true;
        }

        return null;
    }

    async translateFromCyrilic(name: string): Promise<string> {
        const result = await this.entityManager
            .query(`SELECT newUsername = RESULT FROM [azure_temp].TRANSLATE_CYRILIC_TO_LATIN('${name}')`)
            .catch((err) => {
                console.log(err);
            });

        return result[0].newUsername;
    }
}
