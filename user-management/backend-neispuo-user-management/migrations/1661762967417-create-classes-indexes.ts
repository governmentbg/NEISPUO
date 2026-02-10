import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateClassesIndexes1661762967417 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE NONCLUSTERED INDEX IX_Classes_WorkflowType ON azure_temp.Classes(WorkflowType)
            CREATE NONCLUSTERED INDEX IX_Classes_Status ON azure_temp.Classes(Status)
            CREATE NONCLUSTERED INDEX IX_Classes_OrgID  ON azure_temp.Classes(OrgID)
            CREATE NONCLUSTERED INDEX IX_Classes_InProcessing ON azure_temp.Classes(InProcessing)
            CREATE NONCLUSTERED INDEX IX_Classes_InProcessing_Status_RetryAttempts ON azure_temp.Classes(InProcessing, Status,   RetryAttempts)
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP INDEX IX_Classes_WorkflowType 
            DROP INDEX IX_Classes_Status
            DROP INDEX IX_Classes_OrgID
            DROP INDEX IX_Classes_InProcessing
            DROP INDEX IX_Classes_InProcessing_Status_RetryAttempts
            `,
            undefined,
        );
    }
}
