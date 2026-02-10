import { Controller, Get } from '@nestjs/common';
import { ApiBadRequestResponse, ApiCreatedResponse } from '@nestjs/swagger';
import { UserManagementErrorResponse } from 'src/common/dto/responses/user-management-error.response';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { VersionService } from './version.service';

@ApiCreatedResponse({ type: UserManagementResponse })
@ApiBadRequestResponse({ type: UserManagementErrorResponse })
@Controller('v1/version')
export class VersionController {
    constructor(private readonly versionService: VersionService) {}

    @Get()
    getVersion(): UserManagementResponse {
        const result = this.versionService.getVersion();
        const response = new UserManagementResponse(result);
        return response;
    }
}
