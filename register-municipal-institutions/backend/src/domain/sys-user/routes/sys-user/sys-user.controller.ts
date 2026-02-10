import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { SysUser } from '../../sys-user.entity';
import { SysUserService } from './sys-user.service';

@Crud({
    model: {
        type: SysUser,
    },
    params: {
        SysUserID: {
            type: 'number',
            primary: true,
            field: 'SysUserID',
        },
    },
    routes: {
        only: ['getOneBase'],
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/sys-user')
export class SysUserController implements CrudController<SysUser> {
    get base(): CrudController<SysUser> {
        return this;
    }

    constructor(public service: SysUserService) {}
}
