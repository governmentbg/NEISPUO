import { Module } from '@nestjs/common';
import { MiddlewaresModule } from './common/middlewares/middlewares.module';
import { CorsModule } from './config/cors/cors.module';
import { DatabaseConfigModule } from './config/database/database.configuration.module';
import { GlobalModule } from './models/global.module';
import { IntervalsModule } from './models/intervals/intervals.module';
import { JobsModule } from './models/jobs/jobs.module';
import { siemLoggerOptions } from './models/siem-logger/siem-logger.config';
import { SIEMLoggerModule } from './models/siem-logger/siem-logger.module';

@Module({
    imports: [
        DatabaseConfigModule,
        JobsModule,
        IntervalsModule,
        CorsModule,
        GlobalModule,
        MiddlewaresModule,
        SIEMLoggerModule.forRoot(siemLoggerOptions),
    ],
    controllers: [],
    providers: [],
})
export class AppModule {}
