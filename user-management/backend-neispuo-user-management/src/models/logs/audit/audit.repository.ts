import { Connection, DeepPartial } from 'typeorm';

import { AuditEntity } from 'src/common/entities/audit.entity';
import { Injectable } from '@nestjs/common';

@Injectable()
export class AuditRepository {
    constructor(private connection: Connection) {}

    async createAudits(auditDTOs: DeepPartial<AuditEntity>[]) {
        return this.connection.getRepository(AuditEntity).insert(auditDTOs);
    }

    async createAudit(auditDTOs: DeepPartial<AuditEntity>) {
        return this.connection.getRepository(AuditEntity).insert(auditDTOs);
    }
}
