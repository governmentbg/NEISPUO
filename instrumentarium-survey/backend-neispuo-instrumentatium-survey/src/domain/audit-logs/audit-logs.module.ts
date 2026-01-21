import { Module } from '@nestjs/common';
import { AuditLogsService } from './routes/audit-logs.service';

@Module({
    imports: [AuditLogsService],
    exports: [],
    controllers: [],
    providers: [AuditLogsService]
})
export class AuditLogsModule {}
