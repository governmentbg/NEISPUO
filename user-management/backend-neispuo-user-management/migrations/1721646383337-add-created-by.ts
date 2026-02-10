import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddCreatedBy1721646383337 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.Users ADD CreatedBy varchar(255) DEFAULT NULL');
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('ALTER TABLE azure_temp.Users DROP COLUMN CreatedBy');
    }
}
