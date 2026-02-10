// src/domain/audit/audit.module.ts

import { Module, MiddlewareConsumer, RequestMethod } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AuditEntity } from './audit.entity';
import { AuditLoggerService } from './audit.service';
import { AuditMiddleware } from './audit.middleware';

@Module({
  imports: [TypeOrmModule.forFeature([AuditEntity])],
  providers: [AuditLoggerService],
  exports: [AuditLoggerService], // in case it's used in other modules
})
export class AuditModule {
  configure(consumer: MiddlewareConsumer) {
    consumer
      .apply(AuditMiddleware)
      .forRoutes(
        { path: '/v1/cubejs/load', method: RequestMethod.ALL },
        { path: '/v1/cubejs/download-excel', method: RequestMethod.POST },
        { path: '/v1/saved-reports', method: RequestMethod.POST },
        { path: '/v1/saved-reports', method: RequestMethod.PUT },
        { path: '/v1/saved-reports', method: RequestMethod.DELETE },
      );
  }
}
