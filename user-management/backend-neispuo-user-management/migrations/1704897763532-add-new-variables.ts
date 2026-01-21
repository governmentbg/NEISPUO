import { CONSTANTS } from "src/common/constants/constants";
import {MigrationInterface, QueryRunner} from "typeorm";

export class AddedNewVaribles1704897763532 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ORGANIZATIONS_CREATE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ORGANIZATIONS_CREATE_DEFAULT_TOP_VALUE}');
            `,
        );  
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ORGANIZATIONS_UPDATE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ORGANIZATIONS_UPDATE_DEFAULT_TOP_VALUE}');
            `,
        );
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ORGANIZATIONS_DELETE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ORGANIZATIONS_DELETE_DEFAULT_TOP_VALUE}');
            `,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'azure-organizations-create'
                    )
            `,
            undefined,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'send-azure-organizations-create', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'send-azure-organizations-update', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'send-azure-organizations-delete', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ORGANIZATIONS_DELETE_DEFAULT_TOP_NAME}';
            `,
        ); await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ORGANIZATIONS_UPDATE_DEFAULT_TOP_NAME}';
            `,
        ); await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ORGANIZATIONS_CREATE_DEFAULT_TOP_NAME}';
            `,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'send-azure-organizations-create',
                        'send-azure-organizations-update',
                        'send-azure-organizations-delete'
                    )
            `,
            undefined,
        );
        
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-organizations-create', N'*/55 * 1-23 * * *', 0, 1, 0);
                `,
        );
    }

}
