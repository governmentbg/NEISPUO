import { Injectable } from '@nestjs/common';

@Injectable()
export class RedisConfigService {
    // could not get it to work with redis.
    // although i was supplying valid data it prefered the default values check it later

    get hostname(): string {
        return process.env.REDIS_HOSTNAME;
    }

    get port(): number {
        return Number(process.env.REDIS_PORT);
    }

    get pass(): string {
        return process.env.REDIS_PASSWORD;
    }
}
