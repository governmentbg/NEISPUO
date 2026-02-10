import { Controller } from '@nestjs/common';
import { CrudController } from '@nestjsx/crud';
import { RIProcedure } from '../../ri-procedure.entity';
import { RIProcedureService } from './ri-procedure.service';

// @Crud({
//     model: {
//         type: RIProcedure
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/ri-procedure')
export class RIProcedureController implements CrudController<RIProcedure> {
    get base(): CrudController<RIProcedure> {
        return this;
    }

    constructor(public service: RIProcedureService) {}
}
