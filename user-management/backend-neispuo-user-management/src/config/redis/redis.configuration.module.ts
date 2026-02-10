import { Module } from '@nestjs/common';
import { RedisConfigService } from './redis.configuration.service';

@Module({
    imports: [],
    providers: [RedisConfigService],
    exports: [RedisConfigService],
})
export class RedisConfigModule {}
