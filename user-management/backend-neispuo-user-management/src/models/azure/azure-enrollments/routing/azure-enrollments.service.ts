import { Inject, Injectable, Logger, forwardRef } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { TelelinkCheckStatusResponseEnum } from 'src/common/constants/enum/telelink-check-status-response.enum';
import { TelelinkCreateEventResponseEnum } from 'src/common/constants/enum/telelink-create-event-response.enum';
import { TelelinkStatusCodeEnum } from 'src/common/constants/enum/telelink-status-code.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { EnrollmentDTO } from 'src/common/dto/enrollment-dto';
import { EnrollmentGenerateDTO } from 'src/common/dto/requests/enrollment-generate.dto';
import { EnrollmentStudentToSchoolDeleteRequestDTO } from 'src/common/dto/requests/enrollment-student-to-school-delete-request.dto';
import { EnrollmentUserToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-user-to-class-create-request.dto';
import { EnrollmentUserToClassDeleteRequestDTO } from 'src/common/dto/requests/enrollment-user-to-class-delete-request.dto';
import { EnrollmentUserToSchoolCreateRequestDTO } from 'src/common/dto/requests/enrollment-user-to-school-create-request.dto';
import { AzureEnrollmentsResponseDTO } from 'src/common/dto/responses/azure-enrollments-response.dto';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { OrganizationNotFoundException } from 'src/common/exceptions/organization-not-found.exception';
import { UserNotFoundException } from 'src/common/exceptions/user-not-found.exception';
import { AzureEnrollmentsResponseFactory } from 'src/common/factories/azure-enrollments-response-dto.factory';
import { EventDtoFactory } from 'src/common/factories/event-dto.factory';
import { PrintWorkflowStatisticsService } from 'src/common/services/print-workflow-statistics/print-workflow-statistics.service';
import { ClassService } from 'src/models/class/routing/class.service';
import { InstitutionService } from 'src/models/institution/routing/institution.service';
import { PersonService } from 'src/models/person/routing/person.service';
import { TelelinkService } from 'src/models/telelink/routing/telelink.service';
import { EntityManager } from 'typeorm';
import { AzureUsersService } from '../../azure-users/routing/azure-users.service';
import { AzureEnrollmentsRepository } from '../azure-enrollments.repository';
import { EnrollmentsArchiveService } from './enrollments-archive.service';
import { EnrollmentsErrorService } from './enrollments-error.service';
import { EnrollmentsRestartService } from './enrollments-restart.service';
import { ArchivedResourceQueryDTO } from 'src/common/dto/requests/archived-resource-query.dto';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['getAzureEnrollmentStatus'],
})
export class AzureEnrollmentsService {
    constructor(
        private azureEnrollmentsRepository: AzureEnrollmentsRepository,
        @Inject(forwardRef(() => AzureUsersService)) private azureUsersService: AzureUsersService,
        private classesService: ClassService,
        private institutionService: InstitutionService,
        private personService: PersonService,
        private telelinkService: TelelinkService,
        private enrollmentsArchiveService: EnrollmentsArchiveService,
        private enrollmentsRestartService: EnrollmentsRestartService,
        private enrollmentsErrorService: EnrollmentsErrorService,
    ) {}

    async generateEnrollmentDTO(dto: EnrollmentGenerateDTO) {
        const { personID, curriculumID, institutionID } = dto;
        const result = new EnrollmentDTO();
        if (personID) {
            const person = await this.personService.getPersonByPersonID(personID);
            if (!person?.personID) throw new UserNotFoundException();
            result.userRole = dto?.userRole;
            result.userPersonID = person.personID;
        }
        if (curriculumID) {
            const curriculums = await this.classesService.getCurriculumsByCurriculumID(curriculumID);
            result.curriculumID = curriculumID;
        }
        if (institutionID) {
            const institution = await this.institutionService.getInstitutionByInstitutionID(institutionID);
            if (!institution?.personID) throw new OrganizationNotFoundException();
            result.organizationPersonID = institution.personID;
        }
        return result;
    }

    async generateDeleteEnrollmentDTO(dto: EnrollmentGenerateDTO) {
        const { personID, curriculumID, institutionID, userRole } = dto;
        const result = new EnrollmentDTO();
        if (personID) {
            const person = await this.personService.getPersonByPersonID(personID);
            if (!person?.personID) throw new UserNotFoundException();
            result.userPersonID = person.personID;
            result.userRole = userRole;
        }
        if (curriculumID) {
            result.curriculumID = curriculumID;
        }
        if (institutionID) {
            const institution = await this.institutionService.getInstitutionByInstitutionID(institutionID);
            if (!institution?.personID) throw new OrganizationNotFoundException();
            result.organizationPersonID = institution.personID;
        }
        return result;
    }

    async createAzureEnrollmentUserToClass(dto: EnrollmentUserToClassCreateRequestDTO, entityManager?: EntityManager) {
        const enrollmentDTO = await this.generateEnrollmentDTO(dto);
        const azureEnrollmentDTO = AzureEnrollmentsResponseFactory.createFromEnrollmentDTO(enrollmentDTO);
        const result = await this.azureEnrollmentsRepository.insertAzureClassEnrollment(
            azureEnrollmentDTO,
            entityManager,
        );
        if (!result?.rowID) throw new EntityNotCreatedException();
        return { data: result.rowID };
    }

    async createAzureEnrollmentUserToSchool(
        dto: EnrollmentUserToSchoolCreateRequestDTO,
        entityManager?: EntityManager,
    ) {
        const enrollmentDTO = await this.generateEnrollmentDTO(dto);
        const azureEnrollmentDTO = AzureEnrollmentsResponseFactory.createFromEnrollmentDTO(enrollmentDTO);
        const result = await this.azureEnrollmentsRepository.insertAzureSchoolEnrollment(
            azureEnrollmentDTO,
            entityManager,
        );
        if (!result?.rowID) throw new EntityNotCreatedException();
        return { data: result.rowID };
    }

    async deleteAzureEnrollmentUserToSchool(dto: EnrollmentStudentToSchoolDeleteRequestDTO) {
        const enrollmentDTO = await this.generateEnrollmentDTO(dto);
        const azureEnrollmentDTO = AzureEnrollmentsResponseFactory.createFromEnrollmentDTO(enrollmentDTO);
        const result = await this.azureEnrollmentsRepository.deleteAzureSchoolEnrollment(azureEnrollmentDTO);
        if (!result?.rowID) throw new EntityNotCreatedException();
        return { data: result.rowID };
    }

    async deleteAzureEnrollmentUserToClass(dto: EnrollmentUserToClassDeleteRequestDTO, entityManager?: EntityManager) {
        const enrollmentDTO = await this.generateDeleteEnrollmentDTO(dto);
        const azureEnrollmentDTO = AzureEnrollmentsResponseFactory.createFromEnrollmentDTO(enrollmentDTO);
        const result = await this.azureEnrollmentsRepository.deleteAzureClassEnrollment(
            azureEnrollmentDTO,
            entityManager,
        );
        if (!result?.rowID) throw new EntityNotCreatedException();
        return { data: result.rowID };
    }

    async revertAzureEnrollments() {
        await this.azureEnrollmentsRepository.revertAzureEnrollments();
    }

    async getAllAzureEnrollmentsForSending(workflowType: WorkflowType, userRole: UserRoleType) {
        const result = await this.azureEnrollmentsRepository.getAllAzureEnrollmentsForSending(workflowType, userRole);
        return result;
    }

    async getAllAzureEnrollmentsToSchoolForChecking() {
        const result = await this.azureEnrollmentsRepository.getAllAzureEnrollmentsToSchoolForChecking();
        return result;
    }

    async getAllAzureEnrollmentsToClassForChecking() {
        const result = await this.azureEnrollmentsRepository.getAllAzureEnrollmentsToClassForChecking();
        return result;
    }

    async resendEnrollmentWorkflow(dto: AzureEnrollmentsResponseDTO) {
        const result = await this.azureEnrollmentsRepository.resendWorkflow(dto);
        return result;
    }

    async getAzureEnrollmentStatus(id: number) {
        const result = await this.azureEnrollmentsRepository.getAzureEnrollmentStatus({ rowID: id });
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_EVENT_STATUS]: EventStatus[result.status] } };
    }

    async setHasCompleted(dtos: AzureEnrollmentsResponseDTO[]) {
        const result = await this.azureEnrollmentsRepository.setHasCompleted(dtos);
        return result;
    }

    async setHasFailed(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureEnrollmentsRepository.setHasFailed(dtos);
        return result;
    }

    async setHasFailedCreation(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureEnrollmentsRepository.setHasFailedCreation(dtos);
        return result;
    }

    async setInProcessing(dtos: AzureEnrollmentsResponseDTO[]) {
        const result = await this.azureEnrollmentsRepository.setInProcessing(dtos);
        return result;
    }

    async setInProgress(dtos: AzureEnrollmentsResponseDTO[]) {
        const result = await this.azureEnrollmentsRepository.setInProgress(dtos);
        return result;
    }

    async processSendCreated(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        await this.setInProcessing(dtos);
    }

    async processSendNotCreated(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCreationErrorMessage(
            dtos,
            TelelinkCreateEventResponseEnum.NOT_CREATED,
        );
        await this.handleFailedCreation(dtos);
    }

    async processSendValidationError(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCreationErrorMessage(
            dtos,
            TelelinkCreateEventResponseEnum.VALIDATION_ERROR,
        );
        await this.handleFailedCreation(dtos);
    }

    async processSendException(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.EXCEPTION);
        await this.handleFailedCreation(dtos);
    }

    async processSendError(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.ERROR);
        await this.handleFailedCreation(dtos);
    }

    async processCheckDone(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        await this.setHasCompleted(dtos);
        //unlike the other functions for faster development i avoided making a sync procedure for the enrollments.
        try {
            const groupedWorkflows = await this.azureUsersService.groupWorkflowsByUserRole(dtos);
            await this.classesService.setIsAzureEnrolledForStudent(groupedWorkflows[UserRoleType.STUDENT]);
            await this.classesService.setIsAzureEnrolledForTeacher(groupedWorkflows[UserRoleType.TEACHER]);
        } catch (e) {
            Logger.error(e);
        }
    }

    async processCheckDLQ(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.DLQ);
        await this.handleFailedCheck(dtos);
        // I WILL STOP THIS FOR NOW BECAUSE ENROLLMENTS ARE NON STOP FAILING WHEN org class or userid is not guid
        for (const dto of dtos) {
            if (this.shouldWorkflowBeRestarted(dto?.telelinkResponseDto.statusCode)) {
                await this.resendEnrollmentWorkflow(dto);
            }
        }
    }

    async processCheckInProgress(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsArchiveService.isForArchivation(dtos);
        await this.setInProgress(dtos);
    }

    async processCheckFailed(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.FAILED);
        await this.handleFailedCheck(dtos);
    }

    async processCheckNotFound(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.NOT_FOUND);
        await this.handleFailedCheck(dtos);
    }

    async processCheckOther(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.OTHER);
        await this.handleFailedCheck(dtos);
    }

    async processCheckException(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        this.enrollmentsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.EXCEPTION);
        await this.handleFailedCheck(dtos);
    }

    async sendAzureEnrollmentsForSending(workflowType: WorkflowType, userRole: UserRoleType) {
        const azureEnrollments = await this.getAllAzureEnrollmentsForSending(workflowType, userRole);
        const groupedWorkflows = await this.sendAzureEnrollments(azureEnrollments);
        PrintWorkflowStatisticsService.print(workflowType, `sendAzureEnrollments`, userRole, groupedWorkflows);
    }

    async sendAzureEnrollments(workflows: AzureEnrollmentsResponseDTO[]) {
        const sendWorkflowResponse = await this.sendWorkflows(workflows);
        const groupedWorkflows = this.groupWorkflowsByStatus(sendWorkflowResponse);
        await this.processSendCreated(groupedWorkflows[TelelinkCreateEventResponseEnum.CREATED]);
        await this.processSendNotCreated(groupedWorkflows[TelelinkCreateEventResponseEnum.NOT_CREATED]);
        await this.processSendValidationError(groupedWorkflows[TelelinkCreateEventResponseEnum.VALIDATION_ERROR]);
        await this.processSendException(groupedWorkflows[TelelinkCreateEventResponseEnum.EXCEPTION]);
        await this.processSendError(groupedWorkflows[TelelinkCreateEventResponseEnum.ERROR]);
        return groupedWorkflows;
    }

    async checkAzureEnrollmentsToSchool() {
        const azureEnrollments = await this.getAllAzureEnrollmentsToSchoolForChecking();
        const groupedWorkflows = await this.checkAzureEnrollments(azureEnrollments);
        PrintWorkflowStatisticsService.print(null, `checkAzureEnrollments`, null, groupedWorkflows);
    }

    async checkAzureEnrollmentsToClass() {
        const azureEnrollments = await this.getAllAzureEnrollmentsToClassForChecking();
        const groupedWorkflows = await this.checkAzureEnrollments(azureEnrollments);
        PrintWorkflowStatisticsService.print(null, `checkAzureEnrollments`, null, groupedWorkflows);
    }

    async checkAzureEnrollments(workflows: AzureEnrollmentsResponseDTO[]) {
        const checkWorkflowResponse = await this.checkWorkflows(workflows);
        const groupedWorkflows = this.groupWorkflowsByStatus(checkWorkflowResponse);
        await this.processCheckDone(groupedWorkflows[TelelinkCheckStatusResponseEnum.DONE]);
        await this.processCheckDLQ(groupedWorkflows[TelelinkCheckStatusResponseEnum.DLQ]);
        await this.processCheckInProgress(groupedWorkflows[TelelinkCheckStatusResponseEnum.IN_PROGRESS]);
        await this.processCheckFailed(groupedWorkflows[TelelinkCheckStatusResponseEnum.FAILED]);
        await this.processCheckNotFound(groupedWorkflows[TelelinkCheckStatusResponseEnum.NOT_FOUND]);
        await this.processCheckOther(groupedWorkflows[TelelinkCheckStatusResponseEnum.OTHER]);
        await this.processCheckException(groupedWorkflows[TelelinkCheckStatusResponseEnum.EXCEPTION]);
        return groupedWorkflows;
    }

    shouldWorkflowBeRestarted(statusCode: TelelinkStatusCodeEnum) {
        if (statusCode === TelelinkStatusCodeEnum.FAILURE) {
            return true;
        }
        return false;
    }

    async handleFailedCreation(dtos: AzureEnrollmentsResponseDTO[]) {
        const getDTOsForRestart = this.getDTOsForRestart(dtos);
        await this.enrollmentsRestartService.restartFailedWorkflows(getDTOsForRestart);
        dtos = this.removeDTOsForRestartFromOriginalArray(dtos);
        this.enrollmentsArchiveService.isForArchivation(dtos);
        await this.setHasFailedCreation(dtos);
    }

    async handleFailedCheck(dtos: AzureEnrollmentsResponseDTO[]) {
        const getDTOsForRestart = this.getDTOsForRestart(dtos);
        await this.enrollmentsRestartService.restartFailedWorkflows(getDTOsForRestart);
        dtos = this.removeDTOsForRestartFromOriginalArray(dtos);
        this.enrollmentsArchiveService.isForArchivation(dtos);
        await this.setHasFailed(dtos);
    }

    getDTOsForRestart(dtos: AzureEnrollmentsResponseDTO[]) {
        return dtos.filter((dto) => this.enrollmentsRestartService.isForRestart(dto));
    }

    removeDTOsForRestartFromOriginalArray(dtos: AzureEnrollmentsResponseDTO[]) {
        dtos = dtos.filter((dto) => !this.enrollmentsRestartService.isForRestart(dto));
        return dtos;
    }

    groupWorkflowsByStatus(workflowResponses: any): {
        [key in TelelinkCheckStatusResponseEnum | TelelinkCreateEventResponseEnum]: any[];
    } {
        return workflowResponses.reduce((groups: any, response) => {
            const status = response.status;
            if (!groups[status]) groups[status] = [];
            groups[status].push(response.workflow);
            return groups;
        }, {});
    }

    async sendWorkflows(workflows: AzureEnrollmentsResponseDTO[]) {
        const results = [];
        for (const workflow of workflows) {
            const eventDTO = EventDtoFactory.createFromEnrollmentResponseDTO(workflow);
            const response = await this.telelinkService.createEvent(eventDTO);
            const status = response.status;
            workflow.telelinkResponseDto = response;
            results.push({ workflow, status });
        }
        return results;
    }

    async checkWorkflows(workflows: AzureEnrollmentsResponseDTO[]) {
        const results = [];
        for (const workflow of workflows) {
            const response = await this.telelinkService.checkEvent(workflow.guid);
            const status = response.status;
            workflow.telelinkResponseDto = response;
            results.push({ workflow, status });
        }
        return results;
    }

    async getArchivedPreviousYears(query: ArchivedResourceQueryDTO) {
        return this.azureEnrollmentsRepository.getArchivedPreviousYears(query);
    }
}
