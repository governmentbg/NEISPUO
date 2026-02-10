import { CONSTANTS } from 'src/common/constants/constants';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddSystemUser1647527617645 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        const result = await queryRunner.query(
            `
            INSERT 
            INTO 
                core.Person 
                (
                    FirstName,
                    MiddleName,
                    LastName
                )
            OUTPUT INSERTED.PersonID
            VALUES
                (
                    N'ДСС-Системен',
                    NULL,
                    N'ДСС-Системен'
                )`,
            undefined,
        );
        await queryRunner.query(
            `
            INSERT 
            INTO 
                core.SysUser 
                (
                    Username,
                    Password,
                    IsAzureUser,
                    PersonID,
                    isAzureSynced,
                    InitialPassword,
                    DeletedOn
                ) 
                VALUES
                (
                    N'${CONSTANTS.SYS_USER_USERNAME}',
                    N'$2a$12$UD3aDy.VasW0i3TQs5xlXO/2QDbkfmppSh3xlcyBri4JEMCMWPSwK',
                    0,
                    @0,
                    0,
                    NULL,
                    NULL
                );
        `,
            [result[0].PersonID],
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        const result = await queryRunner.query(
            `
            DELETE
            FROM
                core.SysUser
            OUTPUT DELETED.PersonID
            WHERE
                Username = '${CONSTANTS.SYS_USER_USERNAME}'`,
            undefined,
        );
        await queryRunner.query(
            `	
            DELETE
            FROM
                core.Person
            WHERE
                PersonID = @0`,
            [result[0].PersonID],
        );
    }
}
