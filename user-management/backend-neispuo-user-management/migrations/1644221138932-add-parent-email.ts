// import { MigrationInterface, QueryRunner } from 'typeorm';

// export class AddParentEmail1644221138932 implements MigrationInterface {
//     public async up(queryRunner: QueryRunner): Promise<void> {
//         await queryRunner.query(
//             `ALTER TABLE core.Person ADD ParentEmail VARCHAR(255) CONSTRAINT CorePersonParentEmailDefault DEFAULT NULL`,
//             undefined,
//         );
//     }

//     public async down(queryRunner: QueryRunner): Promise<void> {
//         await queryRunner.query(`ALTER TABLE core.Person DROP CONSTRAINT CorePersonParentEmailDefault`, undefined);
//         await queryRunner.query(`ALTER TABLE core.Person DROP COLUMN ParentEmail`, undefined);
//     }
// }
