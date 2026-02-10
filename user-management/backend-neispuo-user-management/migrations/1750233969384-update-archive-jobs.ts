import { CONSTANTS } from 'src/common/constants/constants';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class UpdateArchiveJobs1750233969384 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            UPDATE
            	azure_temp.CronJobConfig
            SET
            	Name = CASE Name
                    WHEN 'yearly-run-archive-users-previous-year' THEN '${CONSTANTS.JOB_NAME_ARCHIVE_USERS_PREVIOUS_YEARS}'
                    WHEN 'yearly-run-archive-enrollments-previous-year' THEN '${CONSTANTS.JOB_NAME_ARCHIVE_ENROLLMENTS_PREVIOUS_YEARS}'
                    WHEN 'yearly-run-archive-organizations-previous-year' THEN '${CONSTANTS.JOB_NAME_ARCHIVE_ORGANIZATIONS_PREVIOUS_YEARS}'
                    WHEN 'yearly-run-archive-classes-previous-year' THEN '${CONSTANTS.JOB_NAME_ARCHIVE_CLASSES_PREVIOUS_YEARS}'
                END,
            	Cron = '0 */15 0 * * *',
            	IsActive = 1
            WHERE
            	Name IN ('yearly-run-archive-users-previous-year', 'yearly-run-archive-enrollments-previous-year', 'yearly-run-archive-organizations-previous-year', 'yearly-run-archive-classes-previous-year');
            `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            UPDATE
            	azure_temp.CronJobConfig
            SET
            	Name = CASE Name
                	WHEN '${CONSTANTS.JOB_NAME_ARCHIVE_USERS_PREVIOUS_YEARS}'  THEN 'yearly-run-archive-users-previous-year' 
                	WHEN '${CONSTANTS.JOB_NAME_ARCHIVE_ENROLLMENTS_PREVIOUS_YEARS}'  THEN 'yearly-run-archive-enrollments-previous-year' 
                	WHEN '${CONSTANTS.JOB_NAME_ARCHIVE_ORGANIZATIONS_PREVIOUS_YEARS}'  THEN 'yearly-run-archive-organizations-previous-year' 
                	WHEN '${CONSTANTS.JOB_NAME_ARCHIVE_CLASSES_PREVIOUS_YEARS}'  THEN 'yearly-run-archive-classes-previous-year' 
                END,
            	Cron = '0 10 0 1 8 *',
            	IsActive = 0
            WHERE
            	Name IN ('archive-users-previous-years', 'archive-enrollments-previous-years', 'archive-organizations-previous-years', 'archive-classes-previous-years');
            `,
        );
    }
}
