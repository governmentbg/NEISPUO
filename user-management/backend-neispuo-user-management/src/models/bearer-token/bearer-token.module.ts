import { Global, Module } from '@nestjs/common';
import { RedisModule } from '../redis/redis.module';
import { BearerTokenService } from './routing/bearer-token.service';

@Global()
@Module({
    imports: [RedisModule],
    providers: [BearerTokenService],
    controllers: [],
    exports: [BearerTokenService],
})
export class BearerTokenModule {}
