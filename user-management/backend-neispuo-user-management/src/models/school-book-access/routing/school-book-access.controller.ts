import {
    Body,
    Controller,
    Delete,
    Get,
    Logger,
    ParseArrayPipe,
    Patch,
    Post,
    Query,
    Req,
    UseGuards,
} from '@nestjs/common';
import { ApiCreatedResponse } from '@nestjs/swagger';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { PersonnelSchoolBookAccessRequestDTO } from 'src/common/dto/requests/personnel-school-book-access.request.dto';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { SchoolBookAccessService } from './school-book-access.service';
import { SchoolBookAccessGuard } from './school-book-access.guard';
import { UpdatePersonnelSchoolBookAccessRequestDTO } from 'src/common/dto/requests/update-personnel-school-book-access-request.dto';

@UseGuards(
    RoleGuard([
        RoleEnum.MON_ADMIN,
        RoleEnum.CIOO,
        RoleEnum.MON_EXPERT,
        RoleEnum.RUO,
        RoleEnum.RUO_EXPERT,
        RoleEnum.INSTITUTION,
    ]),
    SchoolBookAccessGuard,
)
@Controller('v1/school-book-access')
export class SchoolBookAccessController {
    private readonly logger = new Logger(SchoolBookAccessController.name);

    constructor(public service: SchoolBookAccessService) {}

    @Get()
    async getSchoolBookAccessByPersonId(@Query('personID') personID: number): Promise<UserManagementResponse> {
        const result = await this.service.getSchoolBooksByPersonID(personID);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post()
    @ApiCreatedResponse({ type: UserManagementResponse })
    async giveSchoolBookAccessToPerson(
        @Req() request: AuthedRequest,
        @Body() dto: PersonnelSchoolBookAccessRequestDTO[],
    ) {
        const result = await this.service.giveSchoolBookAccessToPerson(request, dto);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Patch()
    async updateHasAdminAccess(
        @Req() request: AuthedRequest,
        @Body()
        updatePersonnelSchoolBookAccessDTO: UpdatePersonnelSchoolBookAccessRequestDTO,
    ) {
        const result = this.service.updateHasAdminAccess(request, updatePersonnelSchoolBookAccessDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Delete()
    async delete(
        @Req() request: AuthedRequest,
        @Query('rowIDs', new ParseArrayPipe({ items: Number, separator: ',' }))
        rowIDs: number[],
    ) {
        const result = await this.service.deleteSelectedAccesses(request, rowIDs);
        const response = new UserManagementResponse(result);
        return response;
    }
}
