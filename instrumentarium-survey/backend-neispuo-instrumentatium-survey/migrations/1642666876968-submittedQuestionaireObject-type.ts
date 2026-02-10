import {MigrationInterface, QueryRunner} from "typeorm";

export class submittedQuestionaireObjectType1642666876968 implements MigrationInterface {
    name = 'submittedQuestionaireObjectType1642666876968'

    public async up(queryRunner: QueryRunner): Promise<void> {
       await queryRunner.query(`ALTER TABLE tools_assessment.submittedQuestionaire ALTER COLUMN submittedQuestionaireObject nvarchar(max);`);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
    }
}
