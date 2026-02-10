import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateOrganizationsIndexes1661763211418 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE NONCLUSTERED INDEX IX_Organizations_WorkflowType ON azure_temp.Organizations(WorkflowType)
            CREATE NONCLUSTERED INDEX IX_Organizations_Status ON azure_temp.Organizations(Status)
            CREATE NONCLUSTERED INDEX IX_Organizations_OrganizationID ON azure_temp.Organizations(OrganizationID)
            CREATE NONCLUSTERED INDEX IX_Organizations_InProcessing ON azure_temp.Organizations(InProcessing)
            CREATE NONCLUSTERED INDEX IX_Organizations_InProcessing_Status_RetryAttempts ON azure_temp.Organizations(InProcessing, Status,   RetryAttempts)
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP INDEX IX_Organizations_WorkflowType 
            DROP INDEX IX_Organizations_Status
            DROP INDEX IX_Organizations_InProcessing
            DROP INDEX IX_Organizations_OrganizationID
            DROP INDEX IX_Organizations_InProcessing_Status_RetryAttempts
            `,
            undefined,
        );
    }
}
