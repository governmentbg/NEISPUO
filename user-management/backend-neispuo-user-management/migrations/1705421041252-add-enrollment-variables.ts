import { CONSTANTS } from "src/common/constants/constants";
import {MigrationInterface, QueryRunner} from "typeorm";

export class AddEnrollmentVariables1705421041252 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_CLASS_CREATE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_CLASS_CREATE_DEFAULT_TOP_VALUE}');
            `,
        );
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_CLASS_DELETE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_CLASS_DELETE_DEFAULT_TOP_VALUE}');
            `,
        );  
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_SCHOOL_CREATE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_SCHOOL_CREATE_DEFAULT_TOP_VALUE}');
            `,
        );    
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_SCHOOL_DELETE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_SCHOOL_DELETE_DEFAULT_TOP_VALUE}');
            `,
        );    
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_SCHOOL_CREATE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_SCHOOL_CREATE_DEFAULT_TOP_VALUE}');
            `,
        );    
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_SCHOOL_DELETE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_SCHOOL_DELETE_DEFAULT_TOP_VALUE}');
            `,
        );      
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_CLASS_CREATE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_CLASS_CREATE_DEFAULT_TOP_VALUE}');
            `,
        );      
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_CLASS_DELETE_DEFAULT_TOP_NAME}',N'${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_CLASS_DELETE_DEFAULT_TOP_VALUE}');
            `,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        'azure-enrollments-to-school-create',
                        'azure-enrollments-student-to-class-create',
                        'azure-enrollments-teacher-to-class-create'
                    )
            `,
            undefined,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_CLASS_CREATE}', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_CLASS_DELETE}', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_SCHOOL_CREATE}', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_SCHOOL_DELETE}', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_TEACHER_TO_SCHOOL_CREATE}', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_TEACHER_TO_SCHOOL_DELETE}', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_TEACHER_TO_CLASS_CREATE}', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_TEACHER_TO_CLASS_DELETE}', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_CLASS_CREATE_DEFAULT_TOP_NAME}';
            `,
        );
        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_CLASS_DELETE_DEFAULT_TOP_NAME}';
            `,
        );
        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_SCHOOL_CREATE_DEFAULT_TOP_NAME}';
            `,
        );
        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ENROLLMENTS_STUDENT_TO_SCHOOL_DELETE_DEFAULT_TOP_NAME}';
            `,
        );

        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_SCHOOL_CREATE_DEFAULT_TOP_NAME}';
            `,
        );

        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_SCHOOL_DELETE_DEFAULT_TOP_NAME}';
            `,
        );

        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_CLASS_CREATE_DEFAULT_TOP_NAME}';
            `,
        );

        await queryRunner.query(
            `
            DELETE FROM  
                AZURE_TEMP.VARIABLES
            WHERE NAME = '${CONSTANTS.JOBS_ENROLLMENTS_TEACHER_TO_CLASS_DELETE_DEFAULT_TOP_NAME}';
            `,
        );
        await queryRunner.query(
            `
            DELETE 
                FROM 
                    azure_temp.CronJobConfig 
                WHERE 
                    Name IN (
                        '${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_CLASS_CREATE}',
                        '${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_CLASS_DELETE}',
                        '${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_SCHOOL_CREATE}',
                        '${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_STUDENT_TO_SCHOOL_DELETE}',
                        '${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_TEACHER_TO_SCHOOL_CREATE}',
                        '${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_TEACHER_TO_SCHOOL_DELETE}',
                        '${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_TEACHER_TO_CLASS_CREATE}',
                        '${CONSTANTS.JOB_NAME_SEND_AZURE_ENROLLMENTS_TEACHER_TO_CLASS_DELETE}'
                    )
            `,
            undefined,
        );
        
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-to-school-create', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-student-to-class-create', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        
        await queryRunner.query(
            `        
                INSERT INTO azure_temp.CronJobConfig
                (Name, Cron, IsRunning, IsActive, MarkedForRestart)
                VALUES( N'azure-enrollments-teacher-to-class-create', N'*/59 * 1-23 * * *', 0, 1, 0);
                `,
        );
        
    }

}
