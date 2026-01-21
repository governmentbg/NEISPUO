import { Body, Controller, Get, Logger, Param, Post, Query, UseGuards } from '@nestjs/common';
import { ApiBadRequestResponse, ApiCreatedResponse, ApiOperation } from '@nestjs/swagger';
import { CONSTANTS } from 'src/common/constants/constants';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { ArchivedResourceQueryDTO } from 'src/common/dto/requests/archived-resource-query.dto';
import { OrganizationCreateRequestDTO } from 'src/common/dto/requests/organization-create-request.dto';
import { OrganizationDeleteRequestDTO } from 'src/common/dto/requests/organization-delete-request.dto';
import { OrganizationUpdateRequestDTO } from 'src/common/dto/requests/organization-update-request.dto';
import { UserManagementErrorResponse } from 'src/common/dto/responses/user-management-error.response';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { AzureOrganizationsService } from './azure-organizations.service';

@UseGuards(RoleGuard([RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.RUO, RoleEnum.MUNICIPALITY]))
@ApiBadRequestResponse({ type: UserManagementErrorResponse })
@Controller('v1/azure-integrations/school')
export class AzureOrganizationsController {
    constructor(private readonly azureOrganizationsService: AzureOrganizationsService) {}

    private readonly logger = new Logger(AzureOrganizationsController.name);

    @Post('create')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_ORGANIZATION_CREATE })
    async createAzureOrganization(@Body() createOrganizationRequestDTO: OrganizationCreateRequestDTO) {
        const result = await this.azureOrganizationsService.createAzureOrganization(createOrganizationRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('delete')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_ORGANIZATION_DELETE })
    async deleteAzureOrganization(@Body() deleteOrganizationRequestDTO: OrganizationDeleteRequestDTO) {
        const result = await this.azureOrganizationsService.deleteAzureOrganization(deleteOrganizationRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('update')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_ORGANIZATION_UPDATE })
    async updateAzureOrganization(@Body() updateOrganizationRequestDTO: OrganizationUpdateRequestDTO) {
        const result = await this.azureOrganizationsService.updateAzureOrganization(updateOrganizationRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('restore')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_ORGANIZATION_RESTORE })
    async rstoreAzureOrganization(@Body() restoreOrganizationRequestDTO: OrganizationCreateRequestDTO) {
        const result = await this.azureOrganizationsService.restoreAzureOrganization(restoreOrganizationRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Get('status/:id')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_ORGANIZATION_CHECK_STATUS })
    async getAzureOrganizationStatus(@Param('id') id: number) {
        const result = await this.azureOrganizationsService.getAzureOrganizationStatus(id);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Get('archived-previous-years')
    async getArchivedOrganizations(@Query() query: ArchivedResourceQueryDTO) {
        return this.azureOrganizationsService.getArchivedPreviousYears(query);
    }
}
