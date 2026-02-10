import { Controller } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { CPLRAreaType } from '../../cplr-area-type.entity';
import { CPLRAreaTypeService } from './cplr-area-type.service';

@Crud({
    model: {
        type: CPLRAreaType,
    },
    routes: {
        only: ['getManyBase'],
    },
})
@Controller('v1/cplr-area-type')
export class CPLRAreaTypeController implements CrudController<CPLRAreaType> {
    get base(): CrudController<CPLRAreaType> {
        return this;
    }

    constructor(public service: CPLRAreaTypeService) {}
}
