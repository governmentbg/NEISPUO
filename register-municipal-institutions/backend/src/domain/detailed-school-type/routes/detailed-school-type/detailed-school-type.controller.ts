import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { DetailedSchoolType } from '../../detailed-school-type.entity';
import { DetailedSchoolTypeService } from './detailed-school-type.service';

@Crud({
    model: {
        type: DetailedSchoolType,
    },
    routes: {
        only: ['getManyBase'],
    },
    query: {
        join: {
            BaseSchoolType: { eager: true },
        },
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/detailed-school-type')
export class DetailedSchoolTypeController implements CrudController<DetailedSchoolType> {
    get base(): CrudController<DetailedSchoolType> {
        return this;
    }

    constructor(public service: DetailedSchoolTypeService) {}
}
