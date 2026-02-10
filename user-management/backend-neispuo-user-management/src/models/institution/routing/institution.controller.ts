import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { InstitutionsTableEntity } from 'src/common/entities/institutions-table-view.entity';
import { InstitutionGuard } from './institution.guard';
import { InstitutionService } from './institution.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(InstitutionGuard)
@Crud({
    model: {
        type: InstitutionsTableEntity,
    },
    params: {
        id: {
            field: 'institutionID',
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
@Controller('v1/institutions')
export class InstitutionController implements CrudController<InstitutionsTableEntity> {
    constructor(public service: InstitutionService) {}
}
