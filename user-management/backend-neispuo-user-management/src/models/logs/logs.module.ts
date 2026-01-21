import { Module } from '@nestjs/common';
import { AuditModule } from './audit/audit.module';
import { LoginAuditModule } from './audit-login/audit-login.module';
import { LogModule } from './log/log.module';

@Module({
    imports: [AuditModule, LoginAuditModule, LogModule],
    exports: [AuditModule, LoginAuditModule, LogModule],
})
export class LogsModule {}
