import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SchemaDefinitionController } from './schema-definition.controller';
import { SchemaDefinition } from './schema-definition.entity';
import { SchemaDefinitionService } from './schema-definition.service';

@Module({
  imports: [TypeOrmModule.forFeature([SchemaDefinition])],
  controllers: [SchemaDefinitionController],
  providers: [SchemaDefinitionService],
  exports: [SchemaDefinitionService],
})
export class SchemaDefinitionModule {}
