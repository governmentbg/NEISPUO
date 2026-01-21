import { Body, Controller, Delete, Get, Logger, Param, Post, Put, Query, Req, UseGuards } from '@nestjs/common';
import { ApiBadRequestResponse, ApiBearerAuth, ApiCreatedResponse } from '@nestjs/swagger';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { AccessUpsertRequestDTO } from 'src/common/dto/requests/access-upsert-request.dto';
import { ChildAccessRequestDTO } from 'src/common/dto/requests/child-access-request.dto';
import { ChildRevokeAccessRequestDTO } from 'src/common/dto/requests/child-revoke-access-request.dto';
import { ParentChildAccessResponseDTO } from 'src/common/dto/responses/parent-child-access-response.dto';
import { UserManagementErrorResponse } from 'src/common/dto/responses/user-management-error.response';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { ParentChildSchoolBookAccessesFindManyRequestDto } from '../../../common/dto/requests/pcsba-find-many-request.dto';
import { ParentAccessService } from './parent-access.service';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
@ApiBearerAuth()
@ApiCreatedResponse({ type: UserManagementResponse })
@ApiBadRequestResponse({ type: UserManagementErrorResponse })
@Controller('v1/parent-access')
export class ParentAccessController {
    private readonly logger = new Logger(ParentAccessController.name);

    constructor(private readonly parentAccessService: ParentAccessService) {}

    // @Post('/v1/child/access/change')
    // @ApiCreatedResponse({
    //     description: 'Access to all children is successfully changed for requested parentID',
    //     type: UserManagementResponse,
    // })
    // @ApiForbiddenResponse({
    //     description: 'User has no rights to change this parentID',
    // })
    // async changeAccess(@Body() parentRequestDTO: ParentRequestDTO) {
    //     const result = await this.parentAccessService.changeAccess( parentRequestDTO);
    //     const response = new UserManagementResponse(result);
    //     return response;
    // }

    @ApiCreatedResponse({ type: UserManagementResponse })
    @UseGuards(RoleGuard([RoleEnum.PARENT]))
    @Post('/v1/child/enroll')
    async enrollChildAndGiveAccess(@Body() childAccessRequestDTO: ChildAccessRequestDTO, @Req() req: AuthedRequest) {
        const result = await this.parentAccessService.enrollChildAndGiveAccess(childAccessRequestDTO, req);
        const response = new UserManagementResponse(result);
        return response;
    }

    @ApiCreatedResponse({ type: UserManagementResponse })
    @UseGuards(RoleGuard([RoleEnum.PARENT]))
    @Post('/v1/child/unenroll')
    async enrollChildAndTakeAccess(@Body() childAccessRequestDTO: ChildAccessRequestDTO, @Req() req: AuthedRequest) {
        const result = await this.parentAccessService.enrollChildAndTakeAccess(childAccessRequestDTO, req);
        const response = new UserManagementResponse(result);
        return response;
    }

    @ApiCreatedResponse({ type: UserManagementResponse })
    @UseGuards(
        RoleGuard([
            RoleEnum.TEACHER,
            RoleEnum.INSTITUTION,
            RoleEnum.MON_ADMIN,
            RoleEnum.CIOO,
            RoleEnum.CONSORTIUM_HELPDESK,
        ]),
    )
    @Post('/v1/child/revoke')
    async revokeAccessToChild(
        @Body() childRevokeAccessRequestDTO: ChildRevokeAccessRequestDTO,
        @Req() req: AuthedRequest,
    ) {
        const result = await this.parentAccessService.revokeAccessToChild(childRevokeAccessRequestDTO, req);
        const response = new UserManagementResponse(result);
        return response;
    }

    @UseGuards(RoleGuard([RoleEnum.MON_ADMIN]))
    @ApiCreatedResponse({ type: ParentChildAccessResponseDTO })
    @Put('/access-upsert')
    async accessUpsert(@Body() accessUpsertRequestDTO: AccessUpsertRequestDTO, @Req() req: AuthedRequest) {
        return this.parentAccessService.accessUpsert(accessUpsertRequestDTO, req);
    }

    @ApiCreatedResponse({ type: ParentChildAccessResponseDTO })
    @UseGuards(RoleGuard([RoleEnum.MON_ADMIN]))
    @Get('access-list')
    async getParentChildSchoolBookAccesses(@Query() query: ParentChildSchoolBookAccessesFindManyRequestDto) {
        return this.parentAccessService.getParentChildSchoolBookAccesses(query);
    }

    @UseGuards(RoleGuard([RoleEnum.MON_ADMIN]))
    @Delete('/:parentChildSchoolBookAccessID')
    async deleteParentChildSchoolBookAccess(
        @Param('parentChildSchoolBookAccessID') parentChildSchoolBookAccessID: number,
        @Req() req: AuthedRequest,
    ) {
        return this.parentAccessService.deleteParentChildSchoolBookAccess(parentChildSchoolBookAccessID, req);
    }
}
