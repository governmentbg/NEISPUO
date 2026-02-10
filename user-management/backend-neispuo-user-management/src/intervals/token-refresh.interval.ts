import { Injectable, OnModuleInit } from '@nestjs/common';
import { Interval } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { BearerTokenService } from 'src/models/bearer-token/routing/bearer-token.service';

@Injectable()
export class TokenRefreshInterval implements OnModuleInit {
    constructor(private readonly bearerTokenService: BearerTokenService) {}

    async onModuleInit() {
        await this.generateTokens();
    }

    @Interval(CONSTANTS.JOB_INTERVAL_NAME_BEARER_TOKEN, CONSTANTS.JOB_INTERVAL_BEARER_TOKEN)
    async handleCron() {
        /**
         * Always execute this job as it's responsible for the Auth Bearer token for Telelink integration
         */
        await this.generateTokens();
    }

    private async generateTokens() {
        await this.bearerTokenService.generateTelelinkBearerAccessToken();
        await this.bearerTokenService.generateGraphApiBearerAccessToken();
        await this.bearerTokenService.generateParentGraphApiBearerAccessToken();
    }
}
