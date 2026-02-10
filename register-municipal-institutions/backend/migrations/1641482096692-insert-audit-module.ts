import { MigrationInterface, QueryRunner } from 'typeorm';

export class InsertAuditModule1641482096692 implements MigrationInterface {
    name = 'insertAuditModule1641482096692';

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
        INSERT INTO logs.AuditModule 
            (AuditModuleId, Name) 
        VALUES 
            (501, 'REGISTER_MUNICIPAL_INSTITUTIONS');
        `,
        );
    }

public async down(queryRunner: QueryRunner): Promise<void> {
    }
}
