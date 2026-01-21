import { Body, Controller, Logger, Post, Put, Query, Req, UseGuards } from '@nestjs/common';
import { ApiBadRequestResponse, ApiBearerAuth, ApiCreatedResponse, ApiImplicitBody } from '@nestjs/swagger';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { Paging } from 'src/common/dto/paging.dto';
import { RoleAssignmentRequestDTO } from 'src/common/dto/requests/role-assignment-create-request.dts';
import { UserManagementErrorResponse } from 'src/common/dto/responses/user-management-error.response';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { RoleManagementGuard } from './role-management.guard';
import { RoleManagementService } from './role-management.service';

@UseGuards(
    RoleGuard([
        RoleEnum.MON_ADMIN,
        RoleEnum.CIOO,
        RoleEnum.MON_EXPERT,
        RoleEnum.MON_OBGUM,
        RoleEnum.MON_OBGUM_FINANCES,
        RoleEnum.MON_CHRAO,
        RoleEnum.RUO,
        RoleEnum.RUO_EXPERT,
        RoleEnum.MUNICIPALITY,
        RoleEnum.INSTITUTION,
    ]),
)
@ApiBearerAuth()
@ApiCreatedResponse({ type: UserManagementResponse })
@ApiBadRequestResponse({ type: UserManagementErrorResponse })
@Controller('v1/role-management')
export class RoleManagementController {
    private readonly logger = new Logger(RoleManagementController.name);

    constructor(private readonly roleManagementService: RoleManagementService) {}

    @Put('/assignments/get-all')
    @ApiImplicitBody({ name: 'RoleAssignmentRequestDTO', type: [RoleAssignmentRequestDTO] })
    async getRoleAssignmentsByUserID(
        @Query() paging: Paging,
        @Body() roleAssignmentCreateRequestDTO: RoleAssignmentRequestDTO[],
    ) {
        const result = await this.roleManagementService.getRoleAssignmentsByUserID(
            paging,
            roleAssignmentCreateRequestDTO,
        );
        const response = new UserManagementResponse(result);
        return response;
    }

    @UseGuards(
        RoleGuard([
            RoleEnum.MON_ADMIN,
            RoleEnum.CIOO,
            RoleEnum.RUO,
            RoleEnum.RUO_EXPERT,
            RoleEnum.MON_EXPERT,
            RoleEnum.MON_OBGUM,
            RoleEnum.MON_OBGUM_FINANCES,
            RoleEnum.MON_CHRAO,
            RoleEnum.MON_USER_ADMIN,
            RoleEnum.INSTITUTION,
            RoleEnum.RUO,
            RoleEnum.RUO_EXPERT,
        ]),
        RoleManagementGuard,
    )
    @Post('/assignment/manage')
    @ApiImplicitBody({ name: 'RoleAssignmentRequestDTO', type: [RoleAssignmentRequestDTO] })
    async manageRoleAssignment(
        @Req() request: AuthedRequest,
        @Query() paging: Paging,
        @Body() roleAssignmentCreateRequestDTO: RoleAssignmentRequestDTO,
    ) {
        const result = await this.roleManagementService.manageRoleAssignment(
            request,
            paging,
            roleAssignmentCreateRequestDTO,
        );
        const response = new UserManagementResponse(result);
        return response;
    }
}
