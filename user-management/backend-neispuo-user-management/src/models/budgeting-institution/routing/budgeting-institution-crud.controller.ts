import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { BudgetingInstitutionCrudService } from './budgeting-institution-crud.service';
import { BudgetingInstitutionGuard } from './budgeting-institution.guard';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(BudgetingInstitutionGuard)
@Crud({
    model: {
        type: ExternalUserViewEntity,
    },
    params: {
        id: {
            field: 'sysUserID',
            primary: true,
        },
    },
    routes: {
        only: ['getManyBase', 'getOneBase'],
    },
    query: {
        alwaysPaginate: true,
        maxLimit: 99999,
    },
})
@Controller('v1/budgeting-institutions')
export class BudgetingInstitutionCrudController implements CrudController<ExternalUserViewEntity> {
    constructor(public service: BudgetingInstitutionCrudService) {}
}
