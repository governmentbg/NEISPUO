import {
    Module,
    ClassSerializerInterceptor,
    NestModule,
    MiddlewareConsumer,
    ValidationPipe,
    Global,
} from '@nestjs/common';
import { APP_INTERCEPTOR, APP_PIPE } from '@nestjs/core';
import { ConfigService } from './services/config/config.service';
import { AuthMiddleware } from './middleware/auth.middleware';
// import { AuditInterceptor } from './interceptors/audit.interceptor';
import { RecaptchaService } from './services/recaptcha/recaptcha.service';
import { PasswordValidator } from './services/password-validator/password-validator.service';
import { BcryptService } from './services/bcrypt/bcrypt.service';
import { RandomStringService } from './services/random-string/random-string.service';
import { HashesService } from './services/hashes/hashes.service';

// https://docs.nestjs.com/fundamentals/custom-providers
// why not useGlobal: https://github.com/nestjs/nest/issues/1916#issuecomment-509053799
const CUSTOM_PROVIDERS = [
    {
        provide: ConfigService,
        useValue: new ConfigService('config/.env'),
    },
    // TODO: Investigate why app still validates with bellow commented out
    // Possibly default framework behavior - bellow code could be redundant.
    {
        provide: APP_PIPE,
        useClass: ValidationPipe,
    },
    {
        provide: APP_INTERCEPTOR,
        useClass: ClassSerializerInterceptor,
    },
    // {
    //     provide: APP_INTERCEPTOR,
    //     useClass: AuditInterceptor
    // }
];

@Global()
@Module({
    imports: [ClassSerializerInterceptor, ValidationPipe, PasswordValidator],
    providers: [
        ...CUSTOM_PROVIDERS,
        RecaptchaService,
        PasswordValidator,
        BcryptService,
        RandomStringService,
        HashesService,
    ],
    exports: [
        ClassSerializerInterceptor,
        ValidationPipe,
        ConfigService,
        RecaptchaService,
        PasswordValidator,
        BcryptService,
        RandomStringService,
        HashesService,
    ],
})
export class SharedModule implements NestModule {
    configure(consumer: MiddlewareConsumer) {
        consumer.apply(AuthMiddleware).forRoutes('*');
    }
}
