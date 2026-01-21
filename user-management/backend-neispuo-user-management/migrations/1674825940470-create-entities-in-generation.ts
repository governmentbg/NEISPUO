import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateEntitiesInGeneration1674825940470 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                CREATE TABLE azure_temp.EntitiesInGeneration (
                    Identifier varchar(100) NULL,
                    CreatedOn datetime2(0) DEFAULT GETUTCDATE() NULL
                );
            `,
        );
        await queryRunner.query(
            `
            INSERT INTO azure_temp.CronJobConfig
            (Name, Cron, IsRunning, IsActive, MarkedForRestart)
            VALUES( N'delete-entities-in-generation', N'0 0 0 * * *', 0, 1, 0);
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `        
                DROP TABLE azure_temp.EntitiesInGeneration;
            `,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'delete-entities-in-generation'
                    )
            `,
            undefined,
        );
    }
}
