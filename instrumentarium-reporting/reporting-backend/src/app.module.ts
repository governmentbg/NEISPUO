import { MiddlewareConsumer, Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import AppDataSource from 'ormconfig';
import { CubeJSModule } from './cubejs/cubejs.module';
import { ReportModule } from './domain/report/report.module';
import { SchemaDefinitionModule } from './domain/schema-definition/schema-definition.module';
import { SchemaRoleAccessModule } from './domain/schema-role-access/schema-role-access.module';
import { AuthMiddleware } from './shared/middleware/auth.middleware';
import { VersionModule } from './domain/version/version.module';
import { CronModule } from './cron/cron.module';
import { AuditModule } from './domain/audit/audit.module';
import { RequestIdMiddleware } from './shared/middleware/requestId.middleware';
@Module({
  imports: [
    TypeOrmModule.forRoot(AppDataSource.options),
    ReportModule,
    SchemaRoleAccessModule,
    SchemaDefinitionModule,
    CubeJSModule,
    VersionModule,
    RequestIdMiddleware,
    AuditModule,
    // CronModule, // TODO: remove the cron feature in future releases since it's not used anymore
  ],
  controllers: [],
  providers: [],
})
export class AppModule {
  configure(consumer: MiddlewareConsumer) {
    consumer.apply(RequestIdMiddleware).forRoutes('*');
    consumer.apply(AuthMiddleware).forRoutes('*');
  }
}
