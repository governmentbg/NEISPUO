import { MigrationInterface, QueryRunner } from 'typeorm';

export class addSisAccessSecondaryRoleColumn1685361614030 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.users ADD sisAccessSecondaryRole INT DEFAULT NULL', undefined);

        await queryRunner.query(
            'ALTER TABLE azure_temp.UsersArchived ADD sisAccessSecondaryRole INT DEFAULT NULL',
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.UsersArchived DROP CONSTRAINT DF__UsersArch__sisAc__1E4BC1E4');

        await queryRunner.query('ALTER TABLE azure_temp.UsersArchived DROP COLUMN sisAccessSecondaryRole');
    }
}
