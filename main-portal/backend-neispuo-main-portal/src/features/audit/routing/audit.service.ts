import { Injectable } from '@nestjs/common';

import { Connection, DeepPartial } from 'typeorm';
import { AuditEntity } from 'src/entities/audit.entity';

@Injectable()
export class AuditService {
  constructor(private connection: Connection) {}

  async insertAudit(audit: DeepPartial<AuditEntity>[]) {
    return this.connection.getRepository(AuditEntity).insert(audit);
  }
}
