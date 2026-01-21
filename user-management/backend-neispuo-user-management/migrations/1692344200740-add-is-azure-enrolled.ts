import {MigrationInterface, QueryRunner} from "typeorm";

export class AddIsAzureEnrolled1692344200740 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
                ALTER TABLE inst_year.CurriculumStudent ADD isAzureEnrolled INT NOT NULL CONSTRAINT CurriculumStudentIsAzureEnrolledNotNull DEFAULT 0 
            `,
            undefined,
        );  
        await queryRunner.query(
            `
                ALTER TABLE inst_year.CurriculumTeacher ADD isAzureEnrolled INT NOT NULL CONSTRAINT CurriculumTeacherIsAzureEnrolledNotNull DEFAULT 0 
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`ALTER TABLE inst_year.CurriculumStudent DROP CONSTRAINT CurriculumStudentIsAzureEnrolledNotNull`, undefined);
        await queryRunner.query(`ALTER TABLE inst_year.CurriculumStudent DROP COLUMN isAzureEnrolled;`, undefined);
        await queryRunner.query(`ALTER TABLE inst_year.CurriculumTeacher DROP CONSTRAINT CurriculumTeacherIsAzureEnrolledNotNull`, undefined);
        await queryRunner.query(`ALTER TABLE inst_year.CurriculumTeacher DROP COLUMN isAzureEnrolled`, undefined);
    }
}
