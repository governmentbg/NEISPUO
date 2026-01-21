import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { SysRole } from 'src/entities/sys-role.entity';
import { SysRoleService } from './sys-role.service';

@UseGuards(JwksGuard)
@Crud({
  model: {
    type: SysRole,
  },
  routes: {
    only: ['getManyBase', 'getOneBase'],
  },
})
@Controller('v1/sys-role')
export class SysRoleController implements CrudController<SysRole> {
  constructor(public service: SysRoleService) {}
}
