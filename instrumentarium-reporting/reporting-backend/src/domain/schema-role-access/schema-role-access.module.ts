import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SchemaRoleAccessController } from './schema-role-access.controller';
import { SchemaRoleAccess } from './schema-role-access.entity';
import { SchemaRoleAccessService } from './schema-role-access.service';

@Module({
  imports: [TypeOrmModule.forFeature([SchemaRoleAccess])],
  controllers: [SchemaRoleAccessController],
  providers: [SchemaRoleAccessService],
  exports: [SchemaRoleAccessService],
})
export class SchemaRoleAccessModule {}
