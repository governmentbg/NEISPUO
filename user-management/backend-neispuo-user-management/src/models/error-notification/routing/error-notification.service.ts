/* eslint-disable prettier/prettier */
import { Inject, Injectable, Logger, forwardRef } from '@nestjs/common';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { LogService } from 'src/models/logs/log/routing/log.service';
import { EmailerService } from 'src/models/emailer/routing/emailer.service';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['sendEmail'],
})
export class ErrorNotificationService {
    private readonly logger = new Logger(ErrorNotificationService.name);

    constructor(
        @Inject(forwardRef(() => LogService)) private logService: LogService,
        private readonly emailerService: EmailerService,
    ) {}

    async sendEmail() {
        const result = await this.logService.areThereErrorsFromLastWeek();
        if (!result) return;
        const contextVariables = this.generateContextVariables();
        await this.emailerService
            .sendMail({
                subject: `error-report`,
                template: 'error-report-template.hbs',
                context: {
                    // Data to be sent to template engine.
                    FRONTEND_URL: contextVariables.FRONTEND_URL,
                    LAST_WEEK_UTC_DATE: contextVariables.LAST_WEEK_UTC_DATE,
                    TODAY_UTC_DATE: contextVariables.TODAY_UTC_DATE,
                },
            })
            .then((result) => {
                this.logger.warn(`Successfully sent email.`);
                result = true;
            })
            .catch((e) => {
                this.logger.error(`Failed to send email`);
            });
        return result;
    }

    generateContextVariables() {
        const todayDate = new Date();
        const lastWeekDate = new Date(todayDate.getTime() - 7 * 24 * 60 * 60 * 1000);
        const todayUTCDate = new Date(Date.UTC(todayDate.getFullYear(), todayDate.getMonth(), todayDate.getDate()));
        const lastWeekUTCDay = new Date(
            Date.UTC(lastWeekDate.getFullYear(), lastWeekDate.getMonth(), lastWeekDate.getDate()),
        );
        return {
            FRONTEND_URL: process.env.FRONTEND_URL,
            LAST_WEEK_UTC_DATE: lastWeekUTCDay.toISOString(),
            TODAY_UTC_DATE: todayUTCDate.toISOString(),
        };
    }
}
