import { Injectable, Global } from '@nestjs/common';
import { MailerService, ISendMailOptions } from '@nest-modules/mailer';

@Global()
@Injectable()
export class DSSMailService {
    // DSS prepended to disambiguate from 'MailerService'
    constructor(private readonly mailerService: MailerService) {}

    public async sendMail(sendMailOptions: ISendMailOptions) {
        if (process.env.STOP_EMAILS === 'true') {
            return;
        }
        if (process.env.DEV_INBOX) {
            sendMailOptions.to = process.env.DEV_INBOX;
        }

        return this.mailerService.sendMail({
            ...sendMailOptions,
            from: process.env.EMAIL_FROM
        });
    }
}