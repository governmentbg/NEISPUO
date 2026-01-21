import { Controller, Get, Logger, Param, Query, UseGuards } from '@nestjs/common';
import { ApiBadRequestResponse, ApiCreatedResponse, ApiOperation } from '@nestjs/swagger';
import { CONSTANTS } from 'src/common/constants/constants';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { ArchivedResourceQueryDTO } from 'src/common/dto/requests/archived-resource-query.dto';
import { UserManagementErrorResponse } from 'src/common/dto/responses/user-management-error.response';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { AzureEnrollmentsService } from './azure-enrollments.service';

@UseGuards(RoleGuard([RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK]))
@ApiBadRequestResponse({ type: UserManagementErrorResponse })
@Controller('v1/azure-integrations/enrollment')
export class AzureEnrollmentsController {
    constructor(private readonly azureEnrollmentsService: AzureEnrollmentsService) {}

    private readonly logger = new Logger(AzureEnrollmentsController.name);

    @Get('status/:id')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_ENROLLMENT_CHECK_STATUS })
    async getAzureEnrollmentStatus(@Param('id') id: number) {
        const result = await this.azureEnrollmentsService.getAzureEnrollmentStatus(id);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Get('archived-previous-years')
    async getArchivedEnrollments(@Query() query: ArchivedResourceQueryDTO) {
        return this.azureEnrollmentsService.getArchivedPreviousYears(query);
    }
}
