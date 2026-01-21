import { Injectable, Logger } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { Token } from 'src/common/dto/token-dto';
import { RedisService } from 'src/models/redis/redis.service';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import axios from 'axios';
import qs from 'querystring';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['generateBearerAccessToken'],
})
export class BearerTokenService {
    constructor(private readonly redisService: RedisService) {}

    private readonly logger = new Logger(BearerTokenService.name);

    headerOptions = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
    };

    url = process.env.TENANT_URL;

    data = {
        client_id: process.env.TENANT_CLIENT_ID,
        client_secret: process.env.TENANT_CLIENT_SECRET,
        grant_type: process.env.TENANT_GRANT_TYPE,
        scope: '',
    };

    async generateTelelinkBearerAccessToken() {
        this.data.scope = process.env.TELELINK_SCOPE;
        await axios
            .post(this.url, qs.stringify(this.data), this.headerOptions)
            .then((result) => {
                this.setTelelinkBearerAccessToken(result.data);
            })
            .catch((err) => {
                this.logger.log(err);
                this.generateTelelinkBearerAccessToken();
            });
    }

    async setTelelinkBearerAccessToken(token: Token) {
        console.log(token.access_token);
        await this.redisService.set(CONSTANTS.CACHE_STORAGE_TELELINK_TOKEN, token);
    }

    async getTelelinkBearerAccessToken() {
        const token: Token = await this.redisService.get(CONSTANTS.CACHE_STORAGE_TELELINK_TOKEN);
        return token.access_token;
    }

    async generateGraphApiBearerAccessToken() {
        this.data.scope = process.env.GRAPH_SCOPE;
        await axios
            .post(this.url, qs.stringify(this.data), this.headerOptions)
            .then((result) => {
                this.setGraphApiBearerAccessToken(result.data);
            })
            .catch((err) => {
                this.logger.log(err);
                this.generateGraphApiBearerAccessToken();
            });
    }

    async setGraphApiBearerAccessToken(token: Token) {
        await this.redisService.set(CONSTANTS.CACHE_STORAGE_GRAPH_TOKEN, token);
    }

    async getGraphApiBearerAccessToken() {
        const token: Token = await this.redisService.get(CONSTANTS.CACHE_STORAGE_GRAPH_TOKEN);
        return token.access_token;
    }

    async generateParentGraphApiBearerAccessToken() {
        const data = {
            client_id: process.env.PARENT_TENANT_CLIENT_ID,
            client_secret: process.env.PARENT_TENANT_CLIENT_SECRET,
            grant_type: process.env.TENANT_GRANT_TYPE,
            scope: process.env.GRAPH_SCOPE,
        };
        await axios
            .post(process.env.PARENT_TENANT_URL, qs.stringify(data), this.headerOptions)
            .then((result) => {
                this.setParentGraphApiBearerAccessToken(result.data);
            })
            .catch((err) => {
                this.logger.log(err);
                this.generateParentGraphApiBearerAccessToken();
            });
    }

    async setParentGraphApiBearerAccessToken(token: Token) {
        await this.redisService.set(CONSTANTS.CACHE_STORAGE_GRAPH_TOKEN_PARENT, token);
    }

    async getParentGraphApiBearerAccessToken() {
        const token: Token = await this.redisService.get(CONSTANTS.CACHE_STORAGE_GRAPH_TOKEN_PARENT);
        return token.access_token;
    }
}
