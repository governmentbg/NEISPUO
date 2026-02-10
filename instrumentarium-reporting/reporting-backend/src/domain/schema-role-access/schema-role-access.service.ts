import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { Repository } from 'typeorm';
import { SchemaRoleAccess } from './schema-role-access.entity';

@Injectable()
export class SchemaRoleAccessService extends TypeOrmCrudService<SchemaRoleAccess> {
  constructor(
    @InjectRepository(SchemaRoleAccess)
    public readonly sraRepository: Repository<SchemaRoleAccess>,
  ) {
    super(sraRepository);
  }
}
