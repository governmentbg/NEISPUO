import { BadRequestException, Controller, Get, Query, UseGuards } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { GetOrganizationInfoRequestDto } from '../dtos/requests/get-organization-info-request.dto';
import { GetParentInfoRequestDto } from '../dtos/requests/get-parent-info.dto-request';
import { GetUserInfoRequestDto } from '../dtos/requests/get-user-info.dto-request';
import { GraphApiService } from './graph-api.service';

@Controller('v1/graph-api')
@UseGuards(RoleGuard([RoleEnum.MON_ADMIN]))
export class GraphApiController {
    constructor(private readonly graphApiService: GraphApiService) {}

    @Get('organization')
    async getAzureOrganizationInfo(@Query() query: GetOrganizationInfoRequestDto) {
        return this.graphApiService.getAzureOrganizationInfo(query);
    }

    @Get('user')
    async getAzureUserInfo(@Query() query: GetUserInfoRequestDto) {
        if ((!query.publicEduNumber && !query.azureId) || (!!query.publicEduNumber && !!query.azureId)) {
            throw new BadRequestException('Exactly one of the two must be provided: publicEduNumber, azureId');
        }
        return this.graphApiService.getAzureUserInfo(query);
    }

    @Get('parent')
    async getAzureParentInfo(@Query() query: GetParentInfoRequestDto) {
        return this.graphApiService.getAzureParentInfo(query);
    }
}
