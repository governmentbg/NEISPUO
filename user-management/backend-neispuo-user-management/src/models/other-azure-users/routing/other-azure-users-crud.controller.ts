import { Controller, Logger, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { OtherAzureUsersGuard } from './other-azure-users.guard';
import { OtherAzureUsersCrudService } from './other-azure-users-crud.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(RoleGuard([RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.MON_EXPERT]), OtherAzureUsersGuard)
@Crud({
    model: {
        type: ExternalUserViewEntity,
    },
    params: {
        id: {
            field: 'sysUserID',
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
@Controller('v1/other-azure-users')
export class OtherAzureUsersCrudController implements CrudController<ExternalUserViewEntity> {
    private readonly logger = new Logger(OtherAzureUsersCrudController.name);

    constructor(public service: OtherAzureUsersCrudService) {}
}
