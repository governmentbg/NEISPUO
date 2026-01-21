import { MigrationInterface, QueryRunner } from 'typeorm';

export class UpdatePersonIDs1645090914261 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            UPDATE
                der
            SET
                der.a = der.b
                OUTPUT INSERTED.a
            FROM
            (
                Select
                    u.PersonID as a, p.PersonID as b
                from
                    azure_temp.Users u
                join core.Person p  on
                    u.UserID = p.PersonalID
                    WHERE u.PersonID is null
            ) der
            `,
            undefined,
        );
    }

    // eslint-disable-next-line no-empty-function
    public async down(queryRunner: QueryRunner): Promise<void> {}
}
