import { MigrationInterface, QueryRunner } from 'typeorm';

export class RemoveAzureEnrollmentTableConstraint1634735511224 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE azure_temp.Enrollments ALTER COLUMN ClassID VARCHAR(100) NULL`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments ALTER COLUMN OrganizationID VARCHAR(100) NULL`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`DELETE FROM azure_temp.Enrollments WHERE ClassID IS NULL;`, undefined);
        await queryRunner.query(`DELETE FROM azure_temp.Enrollments WHERE OrganizationID IS NULL;`, undefined);
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments ALTER COLUMN ClassID VARCHAR(100) NOT NULL;`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE azure_temp.Enrollments ALTER COLUMN OrganizationID VARCHAR(100) NOT NULL;`,
            undefined,
        );
    }
}
