import { Global, MiddlewareConsumer, Module, NestModule } from '@nestjs/common';
import { ConfigService } from './config/config.service';
import { AuthMiddleware } from '../middleware/auth.middleware';

const CUSTOM_PROVIDERS = [
    {
        provide: ConfigService,
        useValue: new ConfigService(`config/.env`)
    }
];

@Global()
@Module({
    imports: [],
    providers: [...CUSTOM_PROVIDERS],
    exports: [ConfigService]
})
export class SharedModule implements NestModule {
    configure(consumer: MiddlewareConsumer) {
        consumer.apply(AuthMiddleware).forRoutes('*');
    }
}
