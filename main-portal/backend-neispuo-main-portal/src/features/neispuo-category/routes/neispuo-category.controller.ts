import { Controller, Get, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { NeispuoCategory } from '../neispuo-category.entity';
import { NesipuoCategoryService } from './neispuo-category.service';
import { RoleGuard } from './role.guard';

@UseGuards(JwksGuard, RoleGuard)
@Crud({
  model: {
    type: NeispuoCategory,
  },

  routes: {
    only: ['getManyBase', 'getOneBase'],
  },
  query: {
    join: {
      neispuoModules: { eager: true },
      'neispuoModules.roles': { eager: true },
      userGuides: { eager: true, exclude: ['fileContent'] },
    },
    sort: [
      {
        field: 'id',
        order: 'ASC',
      },
    ],
  },
})
@Controller('/v1/neispuo-category')
export class NesipuoCategoryController
  implements CrudController<NeispuoCategory> {
  constructor(public service: NesipuoCategoryService) {}
}
