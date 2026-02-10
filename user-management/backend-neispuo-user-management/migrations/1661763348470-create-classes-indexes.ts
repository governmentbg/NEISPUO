import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateClassesIndexes1661763348470 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE NONCLUSTERED INDEX IX_Classes_WorkflowType_ClassID_CreatedOn
            ON [azure_temp].[Classes] ([WorkflowType],[ClassID])
            INCLUDE ([CreatedOn])
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP INDEX IX_Classes_WorkflowType_ClassID_CreatedOn 
            `,
            undefined,
        );
    }
}
