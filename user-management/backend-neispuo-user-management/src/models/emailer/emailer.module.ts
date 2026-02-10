import { MailerModule } from '@nestjs-modules/mailer';
import { Module } from '@nestjs/common';
import { HandlebarsAdapter } from '@nestjs-modules/mailer/dist/adapters/handlebars.adapter';
import { EmailerService } from './routing/emailer.service';

@Module({
    imports: [
        MailerModule.forRootAsync({
            imports: [EmailerModule],
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
                },
                template: {
                    dir: 'src/common/templates',
                    adapter: new HandlebarsAdapter(),
                    options: {
                        strict: true,
                    },
                },
            }),
        }),
    ],
    controllers: [],
    providers: [EmailerService],
    exports: [EmailerService],
})
export class EmailerModule {}
