import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { StatusType } from '../../status-type.entity';
import { StatusTypeService } from './status-type.service';

// @Crud({
//     model: {
//         type: StatusType
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/status-type')
export class StatusTypeController implements CrudController<StatusType> {
    get base(): CrudController<StatusType> {
        return this;
    }
    constructor(public service: StatusTypeService) {}
}
