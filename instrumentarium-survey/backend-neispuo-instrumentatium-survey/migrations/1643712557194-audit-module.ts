import {MigrationInterface, QueryRunner} from "typeorm";

export class auditModule1643712557194 implements MigrationInterface {
    name = 'auditModule1643712557194'

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
        INSERT INTO logs.AuditModule 
            (AuditModuleId, Name) 
        VALUES 
            (601, 'SURVEY');
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
    
    }

}
