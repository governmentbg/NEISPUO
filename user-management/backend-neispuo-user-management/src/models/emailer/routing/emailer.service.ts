/* eslint-disable prettier/prettier */
import { ISendMailOptions, MailerService as MailService } from '@nestjs-modules/mailer';
import { Injectable, Logger } from '@nestjs/common';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['sendEmail'],
})
export class EmailerService {
    private readonly logger = new Logger(EmailerService.name);

    constructor(private readonly mailService: MailService) {}

    public async sendMail(sendMailOptions: ISendMailOptions) {
        if (process.env.STOP_EMAILS === 'true') {
            return;
        }
        if (!sendMailOptions?.to && process.env.DSS_SUPPORT_MAIL) {
            sendMailOptions.to = process.env.DSS_SUPPORT_MAIL;
        }
        return this.mailService.sendMail({
            ...sendMailOptions,
            from: process.env.EMAIL_FROM,
        });
    }
}
