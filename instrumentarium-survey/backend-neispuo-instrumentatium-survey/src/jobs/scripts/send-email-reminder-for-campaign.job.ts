import { Injectable, OnModuleInit } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { CONSTANTS } from '../constants';
import { LoggerService } from 'src/shared/services/logger/logger.service';
import { getManager } from 'typeorm';
import { TeachersQuestionaireReminderService } from '../../domain/notifications/teachers-questionaire-reminder/routes/teachers-questionaire-reminder.service';
import { SubmittedQuestionaireStates } from '@domain/submitted-questionaire/submitted-questionaire-states.enum';

@Injectable()
export class SendEmailReminderForCampaign implements OnModuleInit {

    constructor(
        private readonly loggerService: LoggerService,
        private notificationService: TeachersQuestionaireReminderService
    ) { }

    onModuleInit() {
        this.loggerService.jobsLogger.info('[SendEmailReminderForCampaign] INIT');
    }

    @Cron(CONSTANTS.SEND_EMAIL_REMINDER_FOR_CAMPAIGN_INTERVAL, { name: CONSTANTS.JOB_NAME_SEND_EMAIL_REMINDER_FOR_CAMPAIGN })
    handleCron() {
        this.sendEmailRemindersToUnfilledUsers();
        this.loggerService.jobsLogger.info('[SendEmailReminderForCampaign] Called this.sendEmailReminders()');

        this.sendEmailRemindersForCampaignsStartingToday();
        this.loggerService.jobsLogger.info('[SendEmailReminderForCampaign] Called this.sendEmailRemindersForCampaignsStartingToday()');
    }

    public async sendEmailRemindersToUnfilledUsers() {
        let campaigns = await this.getCampaignsThreeDaysBeforeEndDate();

        for (let campaign of campaigns) {
            const unfilledUsers = await this.getUnfilledUsers(campaign.id);

            await this.notificationService.sendEmailsToTeachers(unfilledUsers);
        }
    }

    public async sendEmailRemindersForCampaignsStartingToday() {
        let campaigns = await this.getCampaignsStartingToday();

        for (let campaign of campaigns) {
            const assignedUsers = await this.getAllAssignedUsers(campaign.id);

            await this.notificationService.sendEmailsToTeachers(assignedUsers);
        }
    }

    public async getCampaignsStartingToday() {
        const entityManager = getManager();
        const campaigns = await entityManager.query(`
            SELECT c.*
            FROM tools_assessment.campaigns c
            WHERE CAST(c.startDate AS DATE) = CAST(GETDATE() AS DATE)
            AND c.isActive = 1
        `);

        return campaigns;
    }

    public async getCampaignsThreeDaysBeforeEndDate() {
        const entityManager = getManager();
        const campaigns = await entityManager.query(`
            SELECT c.*
            from tools_assessment.campaigns c
            where DATEDIFF(DAY, c.endDate, GETDATE()) = 3
            AND c.isActive = 1
        `);

        return campaigns;
    }

    public async getUnfilledUsers(campaignId: number) {
        const entityManager = getManager();
        const users = await entityManager.query(`
            SELECT su.Username as "email", su.SysUserID, CONCAT(pers.FirstName, ' ', pers.LastName) as "fullName"
            FROM tools_assessment.campaigns c
            LEFT JOIN tools_assessment.submittedQuestionaire sq ON sq.campaignId = c.id 
            LEFT JOIN core.SysUser su ON sq.userId = su.SysUserID 
            LEFT JOIN core.Person pers on pers.PersonID = su.PersonID
            WHERE c.id = ${campaignId}
            AND su.Username IS NOT NULL
            AND sq.state = ${SubmittedQuestionaireStates.DRAFT}
            AND su.DeletedOn IS NULL
        `);
        return users;
    }

    public async getAllAssignedUsers(campaignId: number) {
        const entityManager = getManager();
        const users = await entityManager.query(`
            SELECT su.Username as "email", su.SysUserID, CONCAT(pers.FirstName, ' ', pers.LastName) as "fullName"
            FROM tools_assessment.campaigns c 
            LEFT JOIN tools_assessment.submittedQuestionaire sq ON sq.campaignId = c.id 
            LEFT JOIN core.SysUser su ON sq.userId = su.SysUserID 
            LEFT JOIN core.Person pers on pers.PersonID = su.PersonID
            WHERE c.id = ${campaignId}
            AND su.Username IS NOT NULL
            AND su.DeletedOn IS NULL
        `);
        return users;
    }
}