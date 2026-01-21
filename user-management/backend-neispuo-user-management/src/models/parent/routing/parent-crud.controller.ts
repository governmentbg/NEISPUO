import { Body, Controller, Post, UseGuards } from '@nestjs/common';
import { ApiCreatedResponse, ApiOperation } from '@nestjs/swagger';
import { Crud, CrudController } from '@nestjsx/crud';
import { CONSTANTS } from 'src/common/constants/constants';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { ParentRequestDTO } from 'src/common/dto/requests/parent-request.dto';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { ParentViewEntity } from 'src/common/entities/parent-view.entity';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { ParentCrudService } from './parent-crud.service';

@UseGuards(RoleGuard([RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK]))
@Crud({
    model: {
        type: ParentViewEntity,
    },
    params: {},
    routes: {
        only: ['getManyBase', 'getOneBase'],
    },
    query: {
        alwaysPaginate: true,
        maxLimit: 99999,
    },
})
@Controller('v1/parent')
export class ParentCrudController implements CrudController<ParentViewEntity> {
    constructor(public service: ParentCrudService) {}

    @Post('azure-sync')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_USER_AZURE_SYNC,
    })
    async azureSyncParent(@Body() dto: ParentRequestDTO) {
        const result = await this.service.azureSyncParent(dto);
        const response = new UserManagementResponse(result);
        return response;
    }
}
