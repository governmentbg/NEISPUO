import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { FinancialSchoolType } from '../../financial-school-type.entity';
import { FinancialSchoolTypeService } from './financial-school-type.service';

@Crud({
    model: {
        type: FinancialSchoolType,
    },
    routes: {
        only: ['getManyBase'],
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/financial-school-type')
export class FinancialSchoolTypeController implements CrudController<FinancialSchoolType> {
    get base(): CrudController<FinancialSchoolType> {
        return this;
    }

    constructor(public service: FinancialSchoolTypeService) {}
}
