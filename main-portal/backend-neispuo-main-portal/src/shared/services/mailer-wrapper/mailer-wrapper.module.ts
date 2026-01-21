import { MailerModule  } from '@nestjs-modules/mailer';
import { Module } from '@nestjs/common';
import { HandlebarsAdapter } from '@nestjs-modules/mailer/dist/adapters/handlebars.adapter';
import { MailerWrapperService } from './mailer-wrapper.service';

@Module({
    imports: [
        MailerModule.forRootAsync({
            imports: [MailerWrapperModule],
            useFactory: async () => ({
                transport: {
                    host: process.env.EMAIL_HOST,
                    port: +process.env.EMAIL_PORT,
                    secure: process.env.EMAIL_SECURE === 'true',
                    auth: {
                        user: process.env.EMAIL_USER,
                        pass: process.env.EMAIL_PASSWORD,
                    },
                },
                defaults: {
                    from: '"nest-modules" <modules@nestjs.com>',
                }
            }),
        }),
    ],
    providers: [MailerWrapperService],
    exports: [MailerWrapperService],
})
export class MailerWrapperModule {}
