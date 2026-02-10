import { Global, Injectable } from '@nestjs/common';

@Global()
@Injectable()
export class CorsService {
    getCorsConfig() {
        return {
            origin: process.env.CORS_DOMAINS === 'any' ? '*' : process.env.CORS_DOMAINS.split(','),
            methods: process.env.CORS_METHODS,
            allowedHeaders: process.env.CORS_ALLOWED_HEADERS,
            exposedHeaders: process.env.CORS_EXPOSED_HEADERS,
            credentials: process.env.CORS_CREDENTIALS === 'true',
            maxAge: +process.env.CORS_MAX_AGE,
            preflightContinue: process.env.CORS_PREFLIGHT_CONTINUE === 'true',
            optionsSuccessStatus: +process.env.CORS_OPTIONS_SUCCESS_STATUS,
        };
    }
}
