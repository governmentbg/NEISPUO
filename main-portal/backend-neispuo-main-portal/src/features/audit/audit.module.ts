import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AuditService } from './routing/audit.service';
import { AuditEntity } from 'src/entities/audit.entity';

@Module({
  imports: [TypeOrmModule.forFeature([AuditEntity])],
  controllers: [],
  providers: [AuditService],
  exports: [],
})
export class AuditModule {}
