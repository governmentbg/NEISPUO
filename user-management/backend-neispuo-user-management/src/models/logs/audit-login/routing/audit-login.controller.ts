import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { LoginAuditEntity } from 'src/common/entities/login-audit.entity';
import { LoginAuditGuard } from './audit-login.guard';
import { LoginAuditService } from './audit-login.service';

@UseGuards(LoginAuditGuard)
@Crud({
    model: {
        type: LoginAuditEntity,
    },
    params: {
        id: {
            field: 'LoginAuditID',
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
@Controller('v1/login-audit')
export class LoginAuditController implements CrudController<LoginAuditEntity> {
    constructor(public service: LoginAuditService) {}
}
