import {
  ISendMailOptions,
  MailerService as MailService,
} from '@nestjs-modules/mailer';
import { Injectable, Logger } from '@nestjs/common';
import { SendEmailResponse } from 'src/shared/interfaces/send-email-response.interface';

@Injectable()
export class MailerWrapperService {
  private readonly logger = new Logger(MailerWrapperService.name);

  constructor(private readonly mailService: MailService) {}

  public async sendMail(
    sendMailOptions: ISendMailOptions,
  ): Promise<SendEmailResponse> {
    if (process.env.STOP_EMAILS === 'true') {
      const messageEN = 'Emails are stopped by environment variable.';
      const messageBG = 'Изпращането на имейли е спряно.';
      this.logger.warn(messageEN);
      return { success: false, messageBG };
    }

    if (!sendMailOptions?.to && process.env.DSS_SUPPORT_MAIL) {
      sendMailOptions.to = process.env.DSS_SUPPORT_MAIL;
    }
    try {
      const info: any = await this.mailService.sendMail({
        ...sendMailOptions,
        from: process.env.EMAIL_FROM,
      });

      const acceptedCount = Array.isArray(info.accepted) ? info.accepted.length : 0;
      const rejectedCount = Array.isArray(info.rejected) ? info.rejected.length : 0;
      const transportOk = acceptedCount > 0 && rejectedCount === 0;

      const messageBG = transportOk ? 'Имейлът е изпратен успешно.' : 'Някои получатели бяха отхвърлени.';

      return { success: transportOk, skipped: false, messageBG };
    } catch (err) {
      const messageBG = 'Неизвестна грешка при изпращане на имейл.';
      this.logger.error(`Failed to send mail: ${err?.message}`, err?.stack);
      return { success: false, messageBG };
    }
  }
}
