import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateUsersIndexes1661763181284 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE NONCLUSTERED INDEX IX_Users_WorkflowType ON azure_temp.Users(WorkflowType)
            CREATE NONCLUSTERED INDEX IX_Users_Status ON azure_temp.Users(Status)
            CREATE NONCLUSTERED INDEX IX_Users_InProcessing ON azure_temp.Users(InProcessing)
            CREATE NONCLUSTERED INDEX IX_Users_InProcessing_Status_RetryAttempts ON azure_temp.Users(InProcessing, Status,   RetryAttempts)
                        `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP INDEX IX_Users_WorkflowType 
            DROP INDEX IX_Users_Status
            DROP INDEX IX_Users_OrgID
            DROP INDEX IX_Users_InProcessing
            DROP INDEX IX_Users_InProcessing_Status_RetryAttempts
            `,
            undefined,
        );
    }
}
