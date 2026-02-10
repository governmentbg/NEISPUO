import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { GlobalModule } from 'src/models/global.module';
import { IntervalsModule } from 'src/models/intervals/intervals.module';
import { SyncCurriculumsWithoutAzureIDTaskRunner } from './sync-curriculums-without-azureid.command';
import { SyncCurriculumsWithoutAzureIDRepository } from './sync-curriculums-without-azureid.repository';
import { SyncCurriculumsWithoutAzureIDService } from './sync-curriculums-without-azureid.service';
require('dotenv').config({ path: `./.env` });

@Module({
    imports: [
        TypeOrmModule.forRoot(),
        TypeOrmModule.forRoot({
            name: 'mssql-read',
            type: 'mssql',
            host: process.env.DB_HOST,
            port: +process.env.DB_PORT,
            username: process.env.DB_USERNAME,
            password: process.env.DB_PASSWORD,
            database: process.env.DB,
            pool: {
                min: 10,
                max: 100,
            },
            options: {
                encrypt: false,
                enableArithAbort: true,
                readOnlyIntent: true,
            },
        }),
        TypeOrmModule.forRoot({
            name: 'mssql-write',
            type: 'mssql',
            host: process.env.DB_HOST,
            port: +process.env.DB_PORT,
            username: process.env.DB_USERNAME,
            password: process.env.DB_PASSWORD,
            database: process.env.DB,
            pool: {
                min: 10,
                max: 100,
            },
            options: {
                encrypt: false,
                enableArithAbort: true,
                readOnlyIntent: false,
            },
        }),
        IntervalsModule,
        GlobalModule,
    ],
    providers: [
        SyncCurriculumsWithoutAzureIDService,
        SyncCurriculumsWithoutAzureIDRepository,
        SyncCurriculumsWithoutAzureIDTaskRunner,
    ],
})
export class SyncCurriculumsWithoutAzureIDModule {}
