import { Controller } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { ProcedureType } from '../../procedure-type.entity';
import { ProcedureTypeService } from './procedure-type.service';

@Crud({
    model: {
        type: ProcedureType,
    },
    routes: {
        only: ['getManyBase'],
    },
})
@Controller('v1/procedure-type')
export class ProcedureTypeController implements CrudController<ProcedureType> {
    get base(): CrudController<ProcedureType> {
        return this;
    }

    constructor(public service: ProcedureTypeService) {}
}
