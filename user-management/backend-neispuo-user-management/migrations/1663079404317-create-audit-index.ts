import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateAuditIndex1663079404317 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE INDEX Audit_AuditModuleId_IDX ON logs.Audit (AuditModuleId, ObjectName);
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP INDEX logs.Audit.Audit_AuditModuleId_IDX
            `,
            undefined,
        );
    }
}
