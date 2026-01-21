import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateEnrollmentsIndexes1661763348463 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE NONCLUSTERED INDEX IX_Enrollments_WorkflowType ON azure_temp.Enrollments(WorkflowType)
            CREATE NONCLUSTERED INDEX IX_Enrollments_Status ON azure_temp.Enrollments(Status)
            CREATE NONCLUSTERED INDEX IX_Enrollments_InProcessing ON azure_temp.Enrollments(InProcessing)
            CREATE NONCLUSTERED INDEX IX_Enrollments_InProcessing_Status_RetryAttempts ON azure_temp.Enrollments(InProcessing, Status,   RetryAttempts)
            CREATE NONCLUSTERED INDEX IX_Enrollments_UserAzureID ON azure_temp.Enrollments(UserAzureID)
            CREATE NONCLUSTERED INDEX IX_Enrollments_ClassAzureID ON azure_temp.Enrollments(ClassAzureID)
            CREATE NONCLUSTERED INDEX IX_Enrollments_OrganizationAzureID ON azure_temp.Enrollments(OrganizationAzureID)
            CREATE NONCLUSTERED INDEX IX_Enrollments_OrganizationRowID ON azure_temp.Enrollments(OrganizationRowID)
            CREATE NONCLUSTERED INDEX IX_Enrollments_UserRowID ON azure_temp.Enrollments(UserRowID)
            CREATE NONCLUSTERED INDEX IX_Enrollments_ClassRowID ON azure_temp.Enrollments(ClassRowID)
        `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP INDEX IX_Enrollments_WorkflowType 
            DROP INDEX IX_Enrollments_Status
            DROP INDEX IX_Enrollments_InProcessing
            DROP INDEX IX_Enrollments_InProcessing_Status_RetryAttempts
            DROP INDEX IX_Enrollments_UserAzureID
            DROP INDEX IX_Enrollments_ClassAzureID
            DROP INDEX IX_Enrollments_OrganizationAzureID
            DROP INDEX IX_Enrollments_OrganizationRowID
            DROP INDEX IX_Enrollments_UserRowID
            DROP INDEX IX_Enrollments_ClassRowID
        `,
            undefined,
        );
    }
}
