import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { EnrollmentsEntity } from 'src/common/entities/azure-enrollments.entity';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { AzureEnrollmentsCrudService } from './azure-enrollments-crud.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(RoleGuard([RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK]))
@Crud({
    model: {
        type: EnrollmentsEntity,
    },
    params: {
        id: {
            field: 'RowID',
            type: 'number',
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
@Controller('v1/azure-enrollments')
export class AzureEnrollmentsCrudController implements CrudController<EnrollmentsEntity> {
    constructor(public service: AzureEnrollmentsCrudService) {}
}
