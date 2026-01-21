import { Body, Controller, Logger, Post, Req, UseGuards } from '@nestjs/common';
import { ApiBadRequestResponse, ApiBearerAuth, ApiCreatedResponse, ApiOperation } from '@nestjs/swagger';
import { CONSTANTS } from 'src/common/constants/constants';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { EnrollmentTeacherToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-class-create-request.dto';
import { EnrollmentTeacherToClassDeleteRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-class-delete-request.dto';
import { EnrollmentTeacherToSchoolCreateRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-school-create-request.dto';
import { EnrollmentTeacherToSchoolDeleteRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-school-delete-request.dto';
import { TeacherCreateRequestDTO } from 'src/common/dto/requests/teacher-create-request.dto';
import { DeleteTeacherDtoRequest } from 'src/common/dto/requests/teacher-delete-request.dto';
import { TeacherUpdateRequestDTO } from 'src/common/dto/requests/teacher-update-request.dto';
import { UserManagementErrorResponse } from 'src/common/dto/responses/user-management-error.response';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { AzureTeacherService } from './azure-teacher.service';

@UseGuards(
    RoleGuard([
        RoleEnum.MON_ADMIN,
        RoleEnum.CIOO,
        RoleEnum.CONSORTIUM_HELPDESK,
        RoleEnum.MON_EXPERT,
        RoleEnum.RUO,
        RoleEnum.RUO_EXPERT,
        RoleEnum.INSTITUTION,
        RoleEnum.TEACHER,
    ]),
)
@ApiBearerAuth()
@ApiCreatedResponse({ type: UserManagementResponse })
@ApiBadRequestResponse({ type: UserManagementErrorResponse })
@Controller('v1/azure-integrations/teacher')
export class AzureTeacherController {
    constructor(private readonly azureTeachersService: AzureTeacherService) {}

    private readonly logger = new Logger(AzureTeacherController.name);

    @Post('create')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_TEACHER_CREATE })
    async createAzureTeacher(@Req() request: AuthedRequest, @Body() studentCreateRequestDTO: TeacherCreateRequestDTO) {
        const result = await this.azureTeachersService.createAzureTeacher(studentCreateRequestDTO, null, request);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('update')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_TEACHER_UPDATE })
    async updateAzureTeacher(@Req() request: AuthedRequest, @Body() teacherUpdateRequestDTO: TeacherUpdateRequestDTO) {
        const result = await this.azureTeachersService.updateAzureTeacher(teacherUpdateRequestDTO, request);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('delete')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_TEACHER_DELETE })
    async deleteAzureTeacher(@Req() request: AuthedRequest, @Body() deleteTeacherDtoRequest: DeleteTeacherDtoRequest) {
        const result = await this.azureTeachersService.deleteAzureTeacher(deleteTeacherDtoRequest, request);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('enrollment-school-create')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_TEACHER_ENROLLMENT_SCHOOL_CREATE,
    })
    async createAzureEnrollmentTeacherToSchool(
        @Body()
        enrollmentRequestDTO: EnrollmentTeacherToSchoolCreateRequestDTO,
    ) {
        const result = await this.azureTeachersService.createAzureEnrollmentTeacherToSchool(enrollmentRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('enrollment-school-delete')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_TEACHER_ENROLLMENT_SCHOOL_DELETE,
    })
    async deleteAzureEnrollmentTeacherToSchool(
        @Body() enrollmentRequestDTO: EnrollmentTeacherToSchoolDeleteRequestDTO,
    ) {
        const result = await this.azureTeachersService.deleteAzureEnrollmentTeacherToSchool(enrollmentRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('enrollment-class-create')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_TEACHER_ENROLLMENT_CLASS_CREATE,
    })
    async createAzureEnrollmentTeacherToClass(@Body() enrollmentRequestDTO: EnrollmentTeacherToClassCreateRequestDTO) {
        const result = await this.azureTeachersService.createAzureEnrollmentTeacherToClass(enrollmentRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('enrollment-class-delete')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_TEACHER_ENROLLMENT_CLASS_DELETE,
    })
    async deleteAzureEnrollmentTeacherToClass(@Body() enrollmentRequestDTO: EnrollmentTeacherToClassDeleteRequestDTO) {
        const result = await this.azureTeachersService.deleteAzureEnrollmentTeacherToClass(enrollmentRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('azure-sync')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_USER_AZURE_SYNC,
    })
    async azureSyncTeacher(@Body() dto: TeacherCreateRequestDTO) {
        const result = await this.azureTeachersService.azureSyncTeacher(dto);
        const response = new UserManagementResponse(result);
        return response;
    }
}
