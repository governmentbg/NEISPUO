import { MigrationInterface, QueryRunner } from 'typeorm';

export class TurnOnClassesArchive1673362321769 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
                UPDATE 
                    azure_temp.CronJobConfig
                SET 
                    IsActive=1,
                    Cron = '0 */5 * * * *'
                WHERE 
                    Name=N'azure-classes-archive'
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
                UPDATE 
                    azure_temp.CronJobConfig
                SET 
                    IsActive=0,
                    Cron = '0 0 0 * * *'
                WHERE 
                    Name=N'azure-classes-archive'
            `,
        );
    }
}
