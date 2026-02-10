import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { Repository } from 'typeorm';
import { SchemaDefinition } from './schema-definition.entity';

@Injectable()
export class SchemaDefinitionService extends TypeOrmCrudService<SchemaDefinition> {
  constructor(
    @InjectRepository(SchemaDefinition)
    public readonly sdRepository: Repository<SchemaDefinition>,
  ) {
    super(sdRepository);
  }
}
