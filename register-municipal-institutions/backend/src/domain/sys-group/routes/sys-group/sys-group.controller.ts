import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { SysGroup } from '../../sys-group.entity';
import { SysGroupService } from './sys-group.service';

// @Crud({
//     model: {
//         type: SysGroup
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/sys-group')
export class SysGroupController implements CrudController<SysGroup> {
    get base(): CrudController<SysGroup> {
        return this;
    }
    constructor(public service: SysGroupService) {}
}
