import { Controller, Logger, UseGuards } from '@nestjs/common';
import { Crud } from '@nestjsx/crud';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { MunicipalityAzureGuard } from './municipality-azure.guard';
import { MunicipalityAzureService } from './municipality-azure.service';

/**
 * General
 * The methods available and generated and exposed by the CRUD controller can be found at https://github.com/nestjsx/crud/wiki/Controllers#description
 *
 * Overriding methods
 * If you need to override any of the routes / methods you can refer to https://github.com/nestjsx/crud/wiki/Controllers#description
 */

@UseGuards(
    RoleGuard([
        RoleEnum.MON_ADMIN,
        RoleEnum.CIOO,
        RoleEnum.MON_EXPERT,
        RoleEnum.MON_OBGUM,
        RoleEnum.MON_OBGUM_FINANCES,
        RoleEnum.MON_CHRAO,
        RoleEnum.RUO_EXPERT,
        RoleEnum.RUO,
        RoleEnum.MUNICIPALITY,
    ]),
    MunicipalityAzureGuard,
)
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
@Controller('v1/municipality')
export class MunicipalityAzureController {
    private readonly logger = new Logger(MunicipalityAzureController.name);

    constructor(public service: MunicipalityAzureService) {}
}
