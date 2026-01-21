import { Injectable } from '@nestjs/common';
import { Connection, EntityManager, getManager } from 'typeorm';

export interface INewUsernameRecord {
    oldUsername?: string;
    personId?: string;
    azureId?: string;
    newUsername?: string;
    message?: string;
    newEdu?: string;
}

@Injectable()
export class DeactivateOldStudentUsernamesRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async getAllCompletedUsers() {
        const result: INewUsernameRecord[] = await this.entityManager.query(
            `SELECT
                "user"."old_username" as oldUsername,
                "user"."person_id" as personId,
                "user"."azure_id" as azureId,
                "user"."new_username" as newUsername,
                "user"."message" as message,
                "user"."new_edu" as newEdu
            FROM "azure_temp"."NewUsernames" "user" 
            WHERE "user"."message" = 'OK'`,
        );

        return result;
    }

    async setDeletedOn(dto: INewUsernameRecord, manager: EntityManager) {
        const entityManager = manager ? manager : this.entityManager;

        const result = await entityManager
            .query(
                `UPDATE core.SysUser
                SET DeletedOn = GETDATE() 
                FROM core.SysUser su
                WHERE su.Username = '${dto.oldUsername}'
                AND su.DeletedOn IS NULL
                `,
            )
            .catch(async (err) => {
                console.log(err);
            });

        return result;
    }

    async updatePerson(dto: INewUsernameRecord, manager: EntityManager) {
        const entityManager = manager ? manager : this.entityManager;
        const result = await entityManager
            .query(
                `UPDATE core.Person
                SET PublicEduNumber = '${dto.newEdu}'
                FROM core.Person p
                WHERE p.PersonID = '${dto.personId}'`,
            )
            .catch(async (err) => {
                await this.updateNewUsernameRecord(dto, 'Persons Edu number was not updated', manager);
            });

        return result;
    }

    async updateNewUsernameRecord(dto: INewUsernameRecord, message: string, manager: EntityManager) {
        const entityManager = manager ? manager : this.entityManager;

        const result = await this.entityManager
            .query(
                `UPDATE azure_temp.NewUsernames
            SET message = '${message}'
            FROM azure_temp.NewUsernames nu
            WHERE nu.old_username = '${dto.oldUsername}'`,
            )
            .catch(async (err) => {
                console.log(err);
            });

        return result;
    }
}
