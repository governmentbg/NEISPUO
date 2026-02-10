import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import dbConfig from 'ormconfig';
import { HandlebarsAdapter, MailerModule } from '@nest-modules/mailer';
import { utilities as nestWinstonModuleUtilities, WinstonModule } from 'nest-winston';
import * as winston from 'winston';
import { ScheduleModule } from '@nestjs/schedule';
import { RegionModule } from '@domain/region/region.module';
import { MunicipalityModule } from '@domain/municipality/municipality.module';
import { InstitutionModule } from '@domain/institution/institution.module';
import { BaseSchoolTypeModule } from '@domain/base-school-type/base-school-type.module';
import { BudgetingInstitutionModule } from '@domain/budgeting-institution/budgeting-institution.module';
import { LocalAreaModule } from '@domain/local-area/local-area.module';
import { FinancialSchoolTypeModule } from '@domain/financial-school-type/financial-school-type.module';
import { DetailedSchoolTypeModule } from '@domain/detailed-school-type/detailed-school-type.module';
import { RIFlexFieldModule } from '@domain/ri-flex-field/ri-flex-field.module';
import { RIFlexFieldValueModule } from '@domain/ri-flex-field-value/ri-flex-field-value.module';
import { RIInstitutionModule } from '@domain/ri-institution/ri-institution.module';
import { RIProcedureModule } from '@domain/ri-procedure/ri-procedure.module';
import { RIPremInstitutionModule } from '@domain/ri-prem-institution/ri-prem-institution.module';
import { TransformTypeModule } from '@domain/transform-type/transform-type.module';
import { CPLRAreaTypeModule } from '@domain/cplr-area-type/cplr-area-type.module';
import { ProcedureTypeModule } from '@domain/procedure-type/procedure-type.module';
import { BlobTempModule } from '@domain/ri-document-temp/blob-temp.module';
import { VersionModule } from '@domain/version/version.module';
import { BulstatModule } from '@domain/bulstat-check/bulstat.module';
import { CurrentYearModule } from '@domain/current-year/current-year.module';
import { BlobModule } from './domain/blob/blob.module';
import { SysUserModule } from './domain/sys-user/sys-user.module';
import { SettingModule } from './domain/setting/setting.module';
import { ConfigService } from './shared/services/config/config.service';
import { SharedModule } from './shared/shared.module';
import { AppService } from './app.service';
import { AppController } from './app.controller';
import { TownModule } from './domain/town/town.module';

@Module({
    imports: [
        ScheduleModule.forRoot(),
        TypeOrmModule.forRoot({
            ...dbConfig.find((connection: any) => connection.name === 'default'), // any is necessary due to TS intermittently not resolving module on watch mode
            migrations: [], // .migrations is used by CLI only. Breaks compilation if not emptied.
        }),
        TypeOrmModule.forRoot({
            ...dbConfig.find(
                (connection: any) => connection.name === process.env.BLOBS_DB_CONNECTION,
            ), // any is necessary due to TS intermittently not resolving module on watch mode
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
                                  winston.format.simple(),
                              ),
                          }),
                      ]
                    : [
                          // log to console
                          new winston.transports.Console({
                              level: process.env.LOG_LEVEL,
                              format: winston.format.combine(
                                  winston.format.timestamp(),
                                  nestWinstonModuleUtilities.format.nestLike(),
                              ),
                          }),
                      ],
            }),
            inject: [ConfigService],
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
                        pass: process.env.EMAIL_PASSWORD,
                    },
                },
                defaults: {
                    from: '"nest-modules" <modules@nestjs.com>',
                },
                template: {
                    dir: 'src/shared/templates',
                    adapter: new HandlebarsAdapter(), // or new PugAdapter()
                    options: {
                        strict: true,
                    },
                },
            }),
            inject: [ConfigService],
        }),
        SettingModule,
        RegionModule,
        MunicipalityModule,
        TownModule,
        InstitutionModule,
        BaseSchoolTypeModule,
        BudgetingInstitutionModule,
        LocalAreaModule,
        FinancialSchoolTypeModule,
        DetailedSchoolTypeModule,
        RIFlexFieldModule,
        RIFlexFieldValueModule,
        RIInstitutionModule,
        RIProcedureModule,
        RIPremInstitutionModule,
        SysUserModule,
        RIPremInstitutionModule,
        TransformTypeModule,
        ProcedureTypeModule,
        CPLRAreaTypeModule,
        BlobModule,
        VersionModule,
        BulstatModule,
        BlobTempModule,
        CurrentYearModule,
    ],
    controllers: [AppController],
    providers: [AppService],
})
export class AppModule {}
