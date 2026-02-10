import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { LoginAuditEntity } from 'src/common/entities/login-audit.entity';
import { LoginAuditController } from './routing/audit-login.controller';
import { LoginAuditService } from './routing/audit-login.service';

@Module({
    imports: [TypeOrmModule.forFeature([LoginAuditEntity])],
    controllers: [LoginAuditController],
    providers: [LoginAuditService],
    exports: [LoginAuditService],
})
export class LoginAuditModule {}
