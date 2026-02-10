import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { AuditEntity } from 'src/common/entities/audit.entity';
import { RoleAuditView } from 'src/common/entities/role-audit-view.entity';
import { DeepPartial } from 'typeorm';
import { AuditRepository } from '../audit.repository';

@Injectable()
export class AuditService extends TypeOrmCrudService<RoleAuditView> {
    constructor(@InjectRepository(RoleAuditView) repo, private auditRepo: AuditRepository) {
        super(repo);
    }

    async insertAudits(roleAssignmentDTOs: DeepPartial<AuditEntity>[]) {
        return this.auditRepo.createAudits(roleAssignmentDTOs);
    }

    async insertAudit(roleAssignmentDTOs: DeepPartial<AuditEntity>) {
        return this.auditRepo.createAudit(roleAssignmentDTOs);
    }
}
