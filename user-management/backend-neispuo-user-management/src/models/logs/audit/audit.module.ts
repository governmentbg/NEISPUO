import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AuditEntity } from 'src/common/entities/audit.entity';
import { RoleAuditView } from 'src/common/entities/role-audit-view.entity';
import { AuditRepository } from './audit.repository';
import { AuditController } from './routing/audit.controller';
import { AuditService } from './routing/audit.service';

@Module({
    imports: [TypeOrmModule.forFeature([AuditEntity, RoleAuditView])],
    controllers: [AuditController],
    providers: [AuditService, AuditRepository],
    exports: [AuditService],
})
export class AuditModule {}
