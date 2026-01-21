import { Controller, Logger, UseGuards } from '@nestjs/common';
import { Crud } from '@nestjsx/crud';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { RuoAzureGuard } from './ruo-azure.guard';
import { RuoAzureService } from './ruo-azure.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(RuoAzureGuard)
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
@Controller('v1/ruo')
export class RuoAzureController {
    private readonly logger = new Logger(RuoAzureController.name);

    constructor(public service: RuoAzureService) {}
}
