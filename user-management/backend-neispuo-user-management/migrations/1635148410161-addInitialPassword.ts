import { MigrationInterface, QueryRunner } from 'typeorm';
//@TODO there has to be a a refactor of the migrations and to check if everything can be rollbacked to the begining
//right now there are problems with the removing a columns because we cannot remove the constraint that the column has
// eslint-disable-next-line @typescript-eslint/naming-convention
export class addInitialPassword1635148410161 implements MigrationInterface {
    name = 'addInitialPassword1635148410161';

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            'ALTER TABLE core.SysUser ADD InitialPassword nvarchar(255) CONSTRAINT InitialPasswordNull  DEFAULT NULL',
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE core.SysUser DROP CONSTRAINT InitialPasswordNull', undefined);
        await queryRunner.query(`ALTER TABLE core.SysUser DROP COLUMN InitialPassword;`, undefined);
    }
}
