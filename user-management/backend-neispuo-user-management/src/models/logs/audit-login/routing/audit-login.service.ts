import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { LoginAuditEntity } from 'src/common/entities/login-audit.entity';
import { DeepPartial } from 'typeorm';

@Injectable()
export class LoginAuditService extends TypeOrmCrudService<LoginAuditEntity> {
    constructor(@InjectRepository(LoginAuditEntity) repo) {
        super(repo);
    }

    async createLoginAudit(loginAuditDTO: DeepPartial<LoginAuditEntity>) {
        const auditRecord = await this.repo.save(loginAuditDTO);
        return auditRecord;
    }
}
