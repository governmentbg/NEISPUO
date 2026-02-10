import { Module } from '@nestjs/common';
import { RequestContextService } from './request-context.service';

@Module({
    controllers: [],
    providers: [RequestContextService],
    exports: [RequestContextService],
})
export class RequestContextModule {}
