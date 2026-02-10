import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { BasicAuthReadOnlyGuard } from '../../../../shared/guards/basic-auth-read-only.guard';
import { BudgetingInstitution } from '../../budgeting-institution.entity';
import { BudgetingInstitutionService } from './budgeting-institution.service';

@Crud({
    model: {
        type: BudgetingInstitution,
    },
    routes: {
        only: ['getManyBase'],
    },
})
@UseGuards(BasicAuthReadOnlyGuard)
@Controller('v1/budgeting-institution')
export class BudgetingInstitutionController implements CrudController<BudgetingInstitution> {
    get base(): CrudController<BudgetingInstitution> {
        return this;
    }

    constructor(public service: BudgetingInstitutionService) {}
}
