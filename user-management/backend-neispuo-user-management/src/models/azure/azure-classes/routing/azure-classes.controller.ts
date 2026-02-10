import { Body, Controller, Get, Logger, Param, Post, UseGuards } from '@nestjs/common';
import { ApiBadRequestResponse, ApiBearerAuth, ApiCreatedResponse, ApiOperation } from '@nestjs/swagger';
import { CONSTANTS } from 'src/common/constants/constants';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { ClassCreateRequestDTO } from 'src/common/dto/requests/class-create-request.dto';
import { ClassDeleteRequestDTO } from 'src/common/dto/requests/class-delete-request.dto';
import { ClassUpdateRequestDTO } from 'src/common/dto/requests/class-update-request.dto';
import { UserManagementErrorResponse } from 'src/common/dto/responses/user-management-error.response';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { AzureClassesService } from './azure-classes.service';

@ApiBearerAuth()
@UseGuards(
    RoleGuard([
        RoleEnum.MON_ADMIN,
        RoleEnum.CIOO,
        RoleEnum.CONSORTIUM_HELPDESK,
        RoleEnum.MON_EXPERT,
        RoleEnum.RUO,
        RoleEnum.RUO_EXPERT,
        RoleEnum.INSTITUTION,
    ]),
)
@ApiBadRequestResponse({ type: UserManagementErrorResponse })
@Controller('v1/azure-integrations/class')
export class AzureClassesController {
    constructor(private readonly azureClassesService: AzureClassesService) {}

    private readonly logger = new Logger(AzureClassesController.name);

    @Post('create')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_CLASS_CREATE })
    async createAzureClasses(@Body() classCreateRequestDTO: ClassCreateRequestDTO) {
        const result = await this.azureClassesService.createAzureClassesAndEnrollUsers(classCreateRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('create/bulk')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_CLASS_CREATE_BULK })
    async createBulkAzureClasses(@Body() dtos: ClassCreateRequestDTO[]) {
        const result = await this.azureClassesService.createBulkAzureClassesAndEnrollUsers(dtos);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('delete')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_CLASS_DELETE })
    async deleteAzureClass(@Body() deleteClassRequestDTO: ClassDeleteRequestDTO) {
        const result = await this.azureClassesService.deleteAzureClass(deleteClassRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('update')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_CLASS_UPDATE })
    async updateAzureClass(@Body() classUpdateRequestDTO: ClassUpdateRequestDTO) {
        const result = await this.azureClassesService.updateAzureClass(classUpdateRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Get('status/:id')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_CLASS_CHECK_STATUS })
    async getAzureClassStatus(@Param('id') id: number) {
        const result = await this.azureClassesService.getAzureClassStatus(id);
        const response = new UserManagementResponse(result);
        return response;
    }
}
