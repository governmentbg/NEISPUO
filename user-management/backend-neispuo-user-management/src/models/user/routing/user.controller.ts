import { BadRequestException, Controller, Get, Logger, Query, UseGuards } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { UserService } from './user.service';
import { UserFindManyRequestDto } from 'src/common/dto/requests/user-find-many-request.dto';

@Controller('v1/user')
export class UserController {
    private readonly logger = new Logger(UserController.name);

    constructor(private readonly userService: UserService) {}

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
    @Get()
    async getRoleAssignmentsByUserID(@Query('personID') personID: number) {
        const result = await this.userService.getSysUserByPersonID(personID);
        const response = new UserManagementResponse(result);
        return response;
    }

    @UseGuards(RoleGuard([RoleEnum.MON_ADMIN]))
    @Get('get-all')
    async getSysUserByPersonID(@Query('personID') personID: number) {
        const result = await this.userService.getSysUsersByPersonID(personID);
        const response = new UserManagementResponse(result);
        return response;
    }

    @UseGuards(RoleGuard([RoleEnum.MON_ADMIN]))
    @Get('find-many')
    async findUsersByUserRoleType(@Query() query: UserFindManyRequestDto) {
        if (!query.personID && !query.email) {
            throw new BadRequestException(`At least one of personID or email query parameters must be provided`);
        }
        return this.userService.findUsersByUserRoleType(query);
    }
}
