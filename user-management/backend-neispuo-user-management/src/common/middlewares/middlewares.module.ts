import { Global, MiddlewareConsumer, Module } from '@nestjs/common';
import { RequestContextModule } from 'nestjs-request-context';
import { AuthMiddleware } from './auth.middleware';
import { RequestIDMiddleware } from './request-id.middleware';

@Global()
@Module({
    imports: [RequestContextModule],
    providers: [],
    exports: [],
})
export class MiddlewaresModule {
    configure(consumer: MiddlewareConsumer): void {
        consumer.apply(RequestIDMiddleware).forRoutes('*');
        consumer.apply(AuthMiddleware).exclude('/v1/version').forRoutes('*');
    }
}
