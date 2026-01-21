import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController, CrudOptions } from '@nestjsx/crud';
import { SchemaDefinitionService } from './schema-definition.service';
import { SchemaDefinitionGuard } from './schema-definition.guard';
import { SchemaDefinition } from './schema-definition.entity';

export const sdCrudOptions: CrudOptions = {
  model: {
    type: SchemaDefinition,
  },
  params: {
    SchemaDefinitionID: {
      field: 'SchemaDefinitionID',
      type: 'number',
      primary: true,
    },
  },
  routes: {
    only: ['getManyBase', 'createOneBase', 'replaceOneBase', 'deleteOneBase'],
  },
  query: {
    alwaysPaginate: false,
    sort: [
      {
        field: 'SchemaDefinitionID',
        order: 'DESC',
      },
    ],
    /**
        We need to add exclude on Primary Keys because otherwise the CRUD returns the 
        primary twice as an array which breaks update/delete services
    */

    exclude: ['SchemaDefinitionID'],
  },
};

@UseGuards(SchemaDefinitionGuard)
@Crud(sdCrudOptions)
@Controller('v1/schema-definition')
export class SchemaDefinitionController
  implements CrudController<SchemaDefinition>
{
  constructor(public service: SchemaDefinitionService) {}

  get base(): CrudController<SchemaDefinition> {
    return this;
  }
}
