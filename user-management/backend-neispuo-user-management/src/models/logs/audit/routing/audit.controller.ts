import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { RoleAuditView } from 'src/common/entities/role-audit-view.entity';
import { AuditGuard } from './audit.guard';
import { AuditService } from './audit.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(AuditGuard)
@Crud({
    model: {
        type: RoleAuditView,
    },
    params: {
        id: {
            field: 'AuditID',
            primary: true,
        },
    },
    routes: {
        only: ['getManyBase'],
    },
    query: {
        alwaysPaginate: true,
        maxLimit: 99999,
    },
})
@Controller('v1/role-assignment')
export class AuditController implements CrudController<RoleAuditView> {
    constructor(public service: AuditService) {}
}
