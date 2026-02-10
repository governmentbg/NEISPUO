import { CACHE_MANAGER, Inject, Injectable } from '@nestjs/common';
import { Cache } from 'cache-manager';

@Injectable()
export class RedisService {
    constructor(@Inject(CACHE_MANAGER) private cacheManager: Cache) {}

    async get(key: string) {
        let value: any = await this.cacheManager.get(key);
        value = JSON.parse(value);
        return value;
    }

    // this ttl if not set causes the token to disapear from the cachemanager
    async set(key: string, value: any) {
        await this.cacheManager.set(key, JSON.stringify(value), { ttl: 10000000 });
    }
}
