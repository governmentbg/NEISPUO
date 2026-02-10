import { MigrationInterface, QueryRunner } from 'typeorm';

export class InsertAuditModules1638528082381 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
        INSERT INTO logs.AuditModule 
            (AuditModuleId, Name) 
        VALUES 
            (401, 'OIDC'),
            (402, 'MAIN_PORTAL'),
            (403, 'USER_MANAGEMENT');
        `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DELETE FROM logs.AuditModule
            WHERE AuditModuleId IN (401, 402, 403);
      `,
            undefined,
        );
    }
}
