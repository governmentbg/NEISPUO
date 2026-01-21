import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { SysUserSysRole } from '../../sysuser-sysrole.entity';
import { SysUserSysRoleService } from './sysuser-sysrole.service';

// @Crud({
//     model: {
//         type: SysUserSysRole
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/sys-role')
export class SysUserSysRoleController implements CrudController<SysUserSysRole> {
    get base(): CrudController<SysUserSysRole> {
        return this;
    }
    constructor(public service: SysUserSysRoleService) {}
}
