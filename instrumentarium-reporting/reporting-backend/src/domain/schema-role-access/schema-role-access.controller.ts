import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController, CrudOptions } from '@nestjsx/crud';
import { SchemaRoleAccess } from './schema-role-access.entity';
import { SchemaRoleAccessService } from './schema-role-access.service';
import { SchemaRoleAccessGuard } from './schema-role-acess.guard';

export const sraCrudOptions: CrudOptions = {
  model: {
    type: SchemaRoleAccess,
  },
  params: {
    SchemaRoleAccessID: {
      field: 'SchemaRoleAccessID',
      type: 'number',
      primary: true,
    },
  },
  routes: {
    only: ['getManyBase', 'createOneBase', 'replaceOneBase', 'deleteOneBase'],
  },
  query: {
    alwaysPaginate: true,
    sort: [
      {
        field: 'SchemaRoleAccessID',
        order: 'DESC',
      },
    ],
    /**
        We need to add exclude on Primary Keys because otherwise the CRUD returns the 
        primary twice as an array which breaks update/delete services
    */
    join: {
      AllowedSysRole: {
        eager: true,
        allow: ['SysRoleID', 'Name'],
        exclude: ['SysRoleID'],
      },
    },
    exclude: ['SchemaRoleAccessID'],
  },
};

@UseGuards(SchemaRoleAccessGuard)
@Crud(sraCrudOptions)
@Controller('v1/schema-role-access')
export class SchemaRoleAccessController
  implements CrudController<SchemaRoleAccess>
{
  constructor(public service: SchemaRoleAccessService) {}

  get base(): CrudController<SchemaRoleAccess> {
    return this;
  }
}
