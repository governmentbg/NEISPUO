import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { BaseSchoolType } from '../../base-school-type.entity';
import { BaseSchoolTypeService } from './base-school-type.service';

@Crud({
    model: {
        type: BaseSchoolType,
    },
    routes: {
        only: ['getManyBase'],
    },
    query: {
        filter: {
            IsValid: 1,
        },
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/base-school-type')
export class BaseSchoolTypeController implements CrudController<BaseSchoolType> {
    get base(): CrudController<BaseSchoolType> {
        return this;
    }

    constructor(public service: BaseSchoolTypeService) {}
}
