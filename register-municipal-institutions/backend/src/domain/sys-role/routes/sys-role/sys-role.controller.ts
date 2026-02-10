import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { SysRole } from '../../sys-role.entity';
import { SysRoleService } from './sys-role.service';

// @Crud({
//     model: {
//         type: SysRole
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/sys-role')
export class SysRoleController implements CrudController<SysRole> {
    get base(): CrudController<SysRole> {
        return this;
    }
    constructor(public service: SysRoleService) {}
}
