import { Body, Controller, Logger, Post, Req, UseGuards } from '@nestjs/common';
import { ApiBearerAuth, ApiCreatedResponse } from '@nestjs/swagger';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { SchoolBookCodeAssignRequestDTO } from 'src/common/dto/requests/school-books-code-assign-request.dto';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { SchoolBookCodeGuard } from './school-book-code.guard';
import { SchoolBookCodeService } from './school-book-code.service';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';

@ApiBearerAuth()
@UseGuards(
    RoleGuard([
        RoleEnum.INSTITUTION,
        RoleEnum.TEACHER,
        RoleEnum.CLASS_TEACHER,
        RoleEnum.MON_ADMIN,
        RoleEnum.CIOO,
        RoleEnum.RUO,
        RoleEnum.RUO_EXPERT,
    ]),
    SchoolBookCodeGuard,
)
@Controller('v1/school-book-code')
export class SchoolBookCodeController {
    private readonly logger = new Logger(SchoolBookCodeController.name);

    constructor(private readonly schoolBookCodeService: SchoolBookCodeService) {}

    @ApiCreatedResponse({
        type: UserManagementResponse,
    })
    @Post('assign')
    async assignSchoolBookCodes(
        @Body() schoolBookCodeReAssignRequestDTO: SchoolBookCodeAssignRequestDTO,
        @Req() req: AuthedRequest,
    ) {
        const result = await this.schoolBookCodeService.assignSchoolBookCodes(schoolBookCodeReAssignRequestDTO, req);
        const response = new UserManagementResponse(result);
        return response;
    }
}
