import { Body, Controller, Logger, Post, Req, UseGuards } from '@nestjs/common';
import { ApiBadRequestResponse, ApiBearerAuth, ApiCreatedResponse, ApiOperation } from '@nestjs/swagger';
import { CONSTANTS } from 'src/common/constants/constants';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { EnrollmentStudentToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-student-to-class-create-request.dto';
import { EnrollmentStudentToClassDeleteRequestDTO } from 'src/common/dto/requests/enrollment-student-to-class-delete-request.dto';
import { EnrollmentStudentToSchoolDeleteRequestDTO } from 'src/common/dto/requests/enrollment-student-to-school-delete-request.dto';
import { StudentCreateRequestDTO } from 'src/common/dto/requests/student-create-request.dto';
import { StudentDeleteDisableRequestDTO } from 'src/common/dto/requests/student-delete-disable-request.dto';
import { StudentUpdateRequestDTO } from 'src/common/dto/requests/student-update-request.dto';
import { UserManagementErrorResponse } from 'src/common/dto/responses/user-management-error.response';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { AzureStudentService } from './azure-student.service';

@UseGuards(
    RoleGuard([
        RoleEnum.MON_ADMIN,
        RoleEnum.CIOO,
        RoleEnum.MON_EXPERT,
        RoleEnum.RUO,
        RoleEnum.RUO_EXPERT,
        RoleEnum.INSTITUTION,
        RoleEnum.TEACHER,
    ]),
)
@ApiBearerAuth()
@ApiBadRequestResponse({ type: UserManagementErrorResponse })
@Controller('v1/azure-integrations/student')
export class AzureStudentController {
    constructor(private readonly azureStudentsService: AzureStudentService) {}

    private readonly logger = new Logger(AzureStudentController.name);

    @Post('create')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_STUDENT_CREATE })
    async createAzureStudent(@Req() request: AuthedRequest, @Body() studentCreateRequestDTO: StudentCreateRequestDTO) {
        const result = await this.azureStudentsService.createAzureStudent(studentCreateRequestDTO, request);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('update')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_STUDENT_UPDATE })
    async updateAzureStudent(@Req() request: AuthedRequest, @Body() studentUpdateRequestDTO: StudentUpdateRequestDTO) {
        const result = await this.azureStudentsService.updateAzureStudent(studentUpdateRequestDTO, null, request);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('delete')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({ title: '', description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_STUDENT_DELETE })
    async deleteOrDisableStudent(
        @Req() request: AuthedRequest,
        @Body() studentDeleteDisableRequestDTO: StudentDeleteDisableRequestDTO,
    ) {
        const result = await this.azureStudentsService.deleteOrDisableAzureStudent(
            studentDeleteDisableRequestDTO,
            request,
        );
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('enrollment-school-create')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_STUDENT_ENROLLMENT_SCHOOL_CREATE,
    })
    async createAzureEnrollmentStudentToSchool(
        @Body()
        createEnrollmentTeacherDtoRequest: EnrollmentStudentToSchoolDeleteRequestDTO,
    ) {
        const result = await this.azureStudentsService.createAzureEnrollmentStudentToSchool(
            createEnrollmentTeacherDtoRequest,
        );
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('enrollment-school-delete')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_STUDENT_ENROLLMENT_SCHOOL_DELETE,
    })
    async deleteAzureEnrollmentStudentToSchool(
        @Body()
        enrollmentStudentToSchoolDeleteRequestDTO: EnrollmentStudentToSchoolDeleteRequestDTO,
    ) {
        const result = await this.azureStudentsService.deleteAzureEnrollmentStudentToSchool(
            enrollmentStudentToSchoolDeleteRequestDTO,
        );
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('enrollment-class-create')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_STUDENT_ENROLLMENT_CLASS_CREATE,
    })
    async createAzureEnrollmentStudentToClass(
        @Req() request: AuthedRequest,
        @Body() enrollmentRequestDTO: EnrollmentStudentToClassCreateRequestDTO,
    ) {
        const result = await this.azureStudentsService.createAzureEnrollmentStudentToClass(
            enrollmentRequestDTO,
            request,
        );
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('enrollment-class-delete')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_STUDENT_ENROLLMENT_CLASS_DELETE,
    })
    async deleteAzureEnrollmentStudentToClass(@Body() enrollmentRequestDTO: EnrollmentStudentToClassDeleteRequestDTO) {
        const result = await this.azureStudentsService.deleteAzureEnrollmentStudentToClass(enrollmentRequestDTO);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('azure-sync')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_USER_AZURE_SYNC,
    })
    async azureSyncStudent(@Req() request: AuthedRequest, @Body() dto: StudentCreateRequestDTO) {
        const result = await this.azureStudentsService.azureSyncStudent(dto, request);
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('convert')
    @ApiCreatedResponse({ type: UserManagementResponse })
    @ApiOperation({
        title: '',
        description: CONSTANTS.SWAGGER_PARAM_ENDPOINT_DESCRIPTION_USER_AZURE_SYNC,
    })
    async convertStudent(@Req() request: AuthedRequest, @Body() dto: StudentCreateRequestDTO) {
        const result = await this.azureStudentsService.convertAzureStudentToTeacher(dto, request);
        const response = new UserManagementResponse(result);
        return response;
    }
}
