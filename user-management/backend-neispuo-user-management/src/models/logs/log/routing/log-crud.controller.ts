import { Controller } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { AuditModuleEnum } from 'src/common/constants/enum/audit-module.enum';
import { LogLevelEnum } from 'src/common/constants/enum/log-level.enum';
import { LogEntity } from 'src/common/entities/log.entity';
import { LogCrudService } from './log-crud.service';

@Crud({
    model: {
        type: LogEntity,
    },
    params: {
        id: {
            field: 'id',
            primary: true,
        },
    },
    routes: {
        only: ['getManyBase'],
    },
    query: {
        alwaysPaginate: true,
        maxLimit: 99999,
        filter: [
            {
                field: 'auditModuleId',
                operator: '$eq',
                value: AuditModuleEnum.USER_MANAGEMENT,
            },
            {
                field: 'level',
                operator: '$eq',
                value: LogLevelEnum.ERROR,
            },
        ],
        sort: [
            {
                field: 'id',
                order: 'DESC',
            },
        ],
    },
})
@Controller('v1/log')
export class LogCrudController implements CrudController<LogEntity> {
    constructor(public service: LogCrudService) {}
}
