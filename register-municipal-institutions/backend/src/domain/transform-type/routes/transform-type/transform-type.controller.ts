import {
 Controller,
} from '@nestjs/common';
import {
    Crud,
    CrudController,
} from '@nestjsx/crud';
import { TransformType } from '../../transform-type.entity';
import { TransformTypeService } from './transform-type.service';

@Crud({
    model: {
        type: TransformType,
    },
    routes: {
        only: ['getManyBase'],
    },
    query: {
        filter: {
            IsValid: true,
        },
    },
})
@Controller('v1/transform-type')
export class TransformTypeController implements CrudController<TransformType> {
    get base(): CrudController<TransformType> {
        return this;
    }

    constructor(public service: TransformTypeService) {}
}
