import { CacheModule, Module } from '@nestjs/common';
import { RedisService } from './redis.service';

@Module({
    imports: [CacheModule.register()],
    providers: [RedisService],
    exports: [RedisService],
})
export class RedisModule {}
