import { Module } from '@nestjs/common';
import { LoggingService } from './logging.service';

@Module({
    exports: [LoggingService],
    providers: [LoggingService],
})
export class LoggerModule {}
