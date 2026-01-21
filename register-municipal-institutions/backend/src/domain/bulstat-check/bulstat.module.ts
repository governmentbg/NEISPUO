import { Module } from '@nestjs/common';
import winston from 'winston';
import { BulstatController } from './routes/bulstat/bulstat.controller';
import { BulstatService } from './routes/bulstat/bulstat.service';

@Module({
    imports: [],
    controllers: [BulstatController],
    exports: [],
    providers: [
        {
            provide: BulstatService,
            useFactory: async () => {
                const logger = winston.createLogger({
                    level: 'info',
                    format: winston.format.json(),
                    transports: [new winston.transports.Console()],
                });
                const service = new BulstatService(logger);
                await service.init();
                return service;
            },
        },
    ],
})
export class BulstatModule {}
