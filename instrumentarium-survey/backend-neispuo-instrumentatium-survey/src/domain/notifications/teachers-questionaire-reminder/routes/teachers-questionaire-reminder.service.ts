import { SubmittedQuestionaireStates } from '@domain/submitted-questionaire/submitted-questionaire-states.enum';
import { Injectable, Logger, Inject } from '@nestjs/common';
import { getManager } from 'typeorm';
import { DSSMailService } from '../../../../shared/services/dss-mail/dss-mail.service';

@Injectable()
export class TeachersQuestionaireReminderService {
    constructor(
        public dssMailer: DSSMailService,
        @Inject('winston') private readonly logger: Logger
    ) {}

    public async getAllTeachersWithoutSubmittedQuestionaireFromInstitution(campaignId: number) {
        const entityManager = getManager();
        const teachers = await entityManager.query(`
            select subm.userId, CONCAT(pers.FirstName, ' ', pers.LastName) as "fullName", sysuser.Username as "email"
            from [tools_assessment].[submittedQuestionaire] subm 
            left join [core].[SysUser] sysuser on subm.userId = sysuser.SysUserID
            left join [core].[Person] pers on pers.PersonID = sysuser.PersonID
            where subm.campaignId = ${campaignId}
            and subm.state = ${SubmittedQuestionaireStates.DRAFT}
            and sysuser.DeletedOn IS NULL
        `);

        return teachers;
    }

    async sendEmailsToTeachers(teachers: any) {
        const template = 'remind-teachers.hbs';

        for (const teacher of teachers) {
            try {
                await this.dssMailer.sendMail({
                    to: teacher.email,
                    subject: `Напомняне - Попълване на въпросник в платформа NEISPUO Survey`,
                    template,
                    context: {
                        fullName: teacher.fullName,
                        email: teacher.email,
                        appUrl: process.env.UI_BASE_URL
                    }
                });
                this.logger.warn(
                    `Successfully sent email: ${teacher.email}, uuid: ${teacher.SysUserID}.`
                );
            } catch (e) {
                this.logger.error(`Failed to send email for ${teacher.email}. Error: ${e}`);
            }
        }
    }
}
