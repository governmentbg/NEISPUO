import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { TypeOrmModule } from '@nestjs/typeorm';
import dbConfig from 'ormconfig';
import { HandlebarsAdapter, MailerModule } from '@nest-modules/mailer';
import { utilities as nestWinstonModuleUtilities, WinstonModule } from 'nest-winston';
import * as winston from 'winston';
import { ScheduleModule } from '@nestjs/schedule';
import { CampaignModule } from './domain/campaigns/campaign.module';
import { JobsModule } from './jobs/jobs.module';
import { EsuiCampaignModule } from './esui/domain/esui-campaign/esui-campaign.module';
import { TeachersQuestionaireReminderModule } from './domain/notifications/teachers-questionaire-reminder/teachers-questionaire-reminder.module';
import { SubmittedQuestionaireModule } from './domain/submitted-questionaire/submitted-questionaire.module';
import { ConfigService } from '@shared/services/config/config.service';
import { SharedModule } from '@shared/services/shared.module';
import { AggregatedResultsModule } from '@domain/aggregated-results/aggregated-results.module';
import { QuestionaireModule } from '@domain/questionaire/questionaire.module';
import { VersionModule } from '@domain/version/version.module';

@Module({
    imports: [
        ScheduleModule.forRoot(),
        TypeOrmModule.forRoot({
            ...dbConfig.find((connection: any) => connection.name === 'default'), // any is necessary due to TS intermittently not resolving module on watch mode
            migrations: [] // .migrations is used by CLI only. Breaks compilation if not emptied.
        }),
        SharedModule,

        WinstonModule.forRootAsync({
            imports: [SharedModule],
            useFactory: (configService: ConfigService) => ({
                transports: process.env.LOG_FILE
                    ? [
                          // log to file
                          new winston.transports.File({
                              filename: process.env.LOG_FILE,
                              level: process.env.LOG_LEVEL,
                              format: winston.format.combine(
                                  winston.format.timestamp(),
                                  winston.format.simple()
                              )
                          })
                      ]
                    : [
                          // log to console
                          new winston.transports.Console({
                              level: process.env.LOG_LEVEL,
                              format: winston.format.combine(
                                  winston.format.timestamp(),
                                  nestWinstonModuleUtilities.format.nestLike()
                              )
                          })
                      ]
            }),
            inject: [ConfigService]
        }),

        MailerModule.forRootAsync({
            imports: [SharedModule],
            useFactory: async (configService: ConfigService) => ({
                transport: {
                    host: process.env.EMAIL_HOST,
                    port: +process.env.EMAIL_PORT,
                    secure: process.env.EMAIL_SECURE === 'true',
                    auth: {
                        user: process.env.EMAIL_USER,
                        pass: process.env.EMAIL_PASSWORD
                    }
                },
                defaults: {
                    from: '"nest-modules" <modules@nestjs.com>'
                },
                template: {
                    dir: 'src/shared/templates',
                    adapter: new HandlebarsAdapter(), // or new PugAdapter()
                    options: {
                        strict: true
                    }
                }
            }),
            inject: [ConfigService]
        }),
        CampaignModule,
        AggregatedResultsModule,
        QuestionaireModule,
        JobsModule,
        TeachersQuestionaireReminderModule,
        SubmittedQuestionaireModule,
        EsuiCampaignModule,
        VersionModule,
    ],
    controllers: [AppController],
    providers: [AppService]
})
export class AppModule {}
