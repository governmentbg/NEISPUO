import { Controller, Get, Logger, Query, UseGuards } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { SchoolBooksService } from './school-books.service';
import { SchoolBooksGuard } from './school-books.guard';

@UseGuards(
    RoleGuard([RoleEnum.MON_ADMIN, RoleEnum.MON_EXPERT, RoleEnum.RUO, RoleEnum.INSTITUTION, RoleEnum.CIOO]),
    SchoolBooksGuard,
)
@Controller('v1/school-books')
export class SchoolBooksController {
    private readonly logger = new Logger(SchoolBooksController.name);

    constructor(public service: SchoolBooksService) {}

    @Get()
    async getSchoolBooksByInstitutionID(
        @Query('institutionID') institutionID: number,
        @Query('personID') personID: number,
    ): Promise<UserManagementResponse> {
        const result = await this.service.getSchoolBooksByInstitutionID(institutionID, personID);
        const response = new UserManagementResponse(result);
        return response;
    }
}
