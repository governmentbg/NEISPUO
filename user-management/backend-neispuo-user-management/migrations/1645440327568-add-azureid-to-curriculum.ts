import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddAzureIDToCurriculum1645440327568 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        // await queryRunner.query(`ALTER TABLE inst_year.Curriculum ADD AzureID VARCHAR(100) NULL`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Classes ADD AzureID VARCHAR(100) NULL`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.ClassesArchived ADD AzureID VARCHAR(100) NULL`, undefined);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        // await queryRunner.query(`ALTER TABLE inst_year.Curriculum DROP COLUMN AzureID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.Classes DROP COLUMN AzureID;`, undefined);
        await queryRunner.query(`ALTER TABLE azure_temp.ClassesArchived DROP COLUMN AzureID;`, undefined);
    }
}
