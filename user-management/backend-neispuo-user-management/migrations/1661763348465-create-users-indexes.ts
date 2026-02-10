import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateUsersIndexes1661763348465 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE NONCLUSTERED INDEX IX_WorkflowType_UserID_Status
            ON azure_temp.Users(WorkflowType,UserID,Status)
            INCLUDE (CreatedOn)                        `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP INDEX IX_WorkflowType_UserID_Status 
            `,
            undefined,
        );
    }
}
