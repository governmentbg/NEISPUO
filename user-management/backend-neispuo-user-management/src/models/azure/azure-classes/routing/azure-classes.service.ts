import { Inject, Injectable, forwardRef } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { ActionTypeEnum } from 'src/common/constants/enum/action-type.enum';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { TelelinkCheckStatusResponseEnum } from 'src/common/constants/enum/telelink-check-status-response.enum';
import { TelelinkCreateEventResponseEnum } from 'src/common/constants/enum/telelink-create-event-response.enum';
import { TelelinkStatusCodeEnum } from 'src/common/constants/enum/telelink-status-code.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { ClassCreateRequestDTO } from 'src/common/dto/requests/class-create-request.dto';
import { ClassDeleteRequestDTO } from 'src/common/dto/requests/class-delete-request.dto';
import { ClassUpdateRequestDTO } from 'src/common/dto/requests/class-update-request.dto';
import { EnrollmentUserToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-user-to-class-create-request.dto';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { AzureClassesResponseFactory } from 'src/common/factories/azure-classes-response-dto.factory';
import { EventDtoFactory } from 'src/common/factories/event-dto.factory';
import { PrintWorkflowStatisticsService } from 'src/common/services/print-workflow-statistics/print-workflow-statistics.service';
import { ClassService } from 'src/models/class/routing/class.service';
import { EducationalStateService } from 'src/models/educational-state/routing/educational-state.service';
import { TelelinkService } from 'src/models/telelink/routing/telelink.service';
import { Connection } from 'typeorm';
import { AzureEnrollmentsService } from '../../azure-enrollments/routing/azure-enrollments.service';
import { AzureClassesRepository } from '../azure-classes.repository';
import { ClassesArchiveService } from './classes-archive.service';
import { ClassesErrorService } from './classes-error.service';
import { ClassesRestartService } from './classes-restart.service';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: [
        'createAzureClassesAndEnrollUsers',
        'deleteAzureClass',
        'updateAzureClass',
        'getAzureClassStatus',
    ],
})
export class AzureClassesService {
    constructor(
        private azureClassesRepository: AzureClassesRepository,
        private telelinkService: TelelinkService,
        private classesService: ClassService,
        @Inject(forwardRef(() => AzureEnrollmentsService))
        private azureEnrollmentService: AzureEnrollmentsService,
        private educationalStateService: EducationalStateService,
        private classesArchiveService: ClassesArchiveService,
        private classesRestartService: ClassesRestartService,
        private classesErrorService: ClassesErrorService,
        private connection: Connection,
    ) {}

    async revertAzureClasses() {
        await this.azureClassesRepository.revertAzureClasses();
    }

    async createBulkAzureClassesAndEnrollUsers(dtos: ClassCreateRequestDTO[]) {
        const result = [];
        /**
         * DEV COMMENTS(Ivelin Gorchovski) TypeORM Manager for transaction issue
         * There is an error when passing manager around and no reasonable solution comes to mind.
         * Ideally we should make a method which
         * does the same as  -> async createAzureClassesAndEnrollUsers but for an array
         */

        await this.connection.transaction(async (manager) => {
            for (const dto of dtos) {
                const { curriculumID, personIDs } = dto;
                const resultEnrollments = [];
                const curriculums = await this.classesService.getCurriculumsByCurriculumID(curriculumID);
                const generatedClassDTO = await this.classesService.generateClassesDTOFromSubjects(curriculums);
                const azureClassesDto = AzureClassesResponseFactory.createFromClassesResponseDTO(generatedClassDTO);
                const resultClasses = await this.azureClassesRepository.insertAzureClass(azureClassesDto, manager);
                if (!resultClasses?.rowID) throw new EntityNotCreatedException();
                for (const personID of personIDs) {
                    const enrollmentDTO: EnrollmentUserToClassCreateRequestDTO = {
                        personID,
                        curriculumID: generatedClassDTO.curriculumID,
                        userRole: UserRoleType.STUDENT,
                    };
                    const resultEnrollment = await this.azureEnrollmentService.createAzureEnrollmentUserToClass(
                        enrollmentDTO,
                        manager,
                    );
                    resultEnrollments.push(resultEnrollment.data);
                }
                result.push({
                    [CONSTANTS.RESPONSE_PARAM_NAME_CLASS_CREATED]: resultClasses.rowID,
                    [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: resultEnrollments,
                });
            }
        });
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_BULK_CLASS_CREATED]: result,
            },
        };
    }

    async createAzureClassesAndEnrollUsers(classCreateRequestDTO: ClassCreateRequestDTO) {
        const { curriculumID, personIDs } = classCreateRequestDTO;
        const resultEnrollments = [];
        let resultClasses: AzureClassesResponseDTO;
        const curriculums = await this.classesService.getCurriculumsByCurriculumID(curriculumID);
        const generatedClassDTO = await this.classesService.generateClassesDTOFromSubjects(curriculums);
        const azureClassesDto = AzureClassesResponseFactory.createFromClassesResponseDTO(generatedClassDTO);

        await this.connection.transaction(async (manager) => {
            resultClasses = await this.azureClassesRepository.insertAzureClass(azureClassesDto, manager);
            if (!resultClasses?.rowID) throw new EntityNotCreatedException();
            for (const personID of personIDs) {
                const userRole = (await this.isUserATeacher(personID)) ? UserRoleType.TEACHER : UserRoleType.STUDENT;
                const enrollmentDTO: EnrollmentUserToClassCreateRequestDTO = {
                    userRole,
                    personID,
                    curriculumID: generatedClassDTO.curriculumID,
                };
                const resultEnrollment = await this.azureEnrollmentService.createAzureEnrollmentUserToClass(
                    enrollmentDTO,
                    manager,
                );
                resultEnrollments.push(resultEnrollment.data);
            }
        });
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_CLASS_CREATED]: resultClasses.rowID,
                [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: resultEnrollments,
            },
        };
    }

    async isUserATeacher(personID: number) {
        const eduStates = await this.educationalStateService.getUserEducationalStatesByPersonID({
            personID,
        });
        const result = eduStates.some((es) => es?.positionID === 2);
        return result;
    }

    async deleteAzureClass(deleteClassesRequestDTO: ClassDeleteRequestDTO) {
        const { curriculumID } = deleteClassesRequestDTO;
        const curriculums = await this.classesService.getCurriculumsByCurriculumID(curriculumID);
        const generatedClassDTO = await this.classesService.generateClassesDTOFromSubjects(curriculums);
        const azureClassesDto = AzureClassesResponseFactory.createFromClassesResponseDTO(generatedClassDTO);
        azureClassesDto.isForArchivation = azureClassesDto?.azureID ? 0 : 1;
        const result = await this.azureClassesRepository.deleteAzureClass(azureClassesDto);
        if (!result?.rowID) throw new EntityNotCreatedException();
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_CLASS_CREATED]: result.rowID } };
    }

    async updateAzureClass(classUpdateRequestDTO: ClassUpdateRequestDTO) {
        const { curriculumID, personIDsToCreate, personIDsToDelete } = classUpdateRequestDTO;
        const resultEnrollments = [];
        let resultClasses;
        await this.connection.transaction(async (manager) => {
            const curriculums = await this.classesService.getCurriculumsByCurriculumID(curriculumID);
            const generatedClassDTO = await this.classesService.generateClassesDTOFromSubjects(curriculums);
            const azureClassesDto = AzureClassesResponseFactory.createFromClassesResponseDTO(generatedClassDTO);
            azureClassesDto.isForArchivation = azureClassesDto?.azureID ? 0 : 1;
            resultClasses = await this.azureClassesRepository.updateAzureClass(azureClassesDto, manager);
            if (!resultClasses?.rowID) throw new EntityNotCreatedException();
            for (const personID of personIDsToDelete) {
                const userRole = (await this.isUserATeacher(personID)) ? UserRoleType.TEACHER : UserRoleType.STUDENT;
                const enrollmentDTO: EnrollmentUserToClassCreateRequestDTO = {
                    userRole,
                    personID,
                    curriculumID: generatedClassDTO.curriculumID,
                };
                const resultEnrollment = await this.azureEnrollmentService.deleteAzureEnrollmentUserToClass(
                    enrollmentDTO,
                    manager,
                );
                resultEnrollments.push(resultEnrollment.data);
            }
            for (const personID of personIDsToCreate) {
                const userRole = (await this.isUserATeacher(personID)) ? UserRoleType.TEACHER : UserRoleType.STUDENT;
                const enrollmentDTO: EnrollmentUserToClassCreateRequestDTO = {
                    userRole,
                    personID,
                    curriculumID: generatedClassDTO.curriculumID,
                };
                const resultEnrollment = await this.azureEnrollmentService.createAzureEnrollmentUserToClass(
                    enrollmentDTO,
                    manager,
                );
                resultEnrollments.push(resultEnrollment.data);
            }
        });
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_CLASS_CREATED]: resultClasses.rowID,
                [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: resultEnrollments,
            },
        };
    }

    async getAllAzureClassesForSending(workflowType: WorkflowType) {
        const result = await this.azureClassesRepository.getAllAzureClassesForSending(workflowType);
        return result;
    }

    async getAllAzureClassesForChecking() {
        const result = await this.azureClassesRepository.getAllAzureClassesForChecking();
        return result;
    }

    async getAllAzureClassesForCreating() {
        const result = await this.azureClassesRepository.getAllAzureClassesForCreating();
        return result;
    }

    async getAzureClassStatus(id: number) {
        const result = await this.azureClassesRepository.getAzureClassStatus({ rowID: id });
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_EVENT_STATUS]: EventStatus[result.status] } };
    }

    async resendClassWorkflow(dto: AzureClassesResponseDTO) {
        const result = await this.azureClassesRepository.resendWorkflow(dto);
        return result;
    }

    async setHasCompleted(dtos: AzureClassesResponseDTO[]) {
        const result = await this.azureClassesRepository.setHasCompleted(dtos);
        return result;
    }

    async setHasFailed(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureClassesRepository.setHasFailed(dtos);
        return result;
    }

    async setHasFailedCreation(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureClassesRepository.setHasFailedCreation(dtos);
        return result;
    }

    async setHasFailedSyncronization(dtos: AzureClassesResponseDTO[]) {
        const result = await this.azureClassesRepository.setHasFailedSyncronization(dtos);
        return result;
    }

    async setInProcessing(dtos: AzureClassesResponseDTO[]) {
        const result = await this.azureClassesRepository.setInProcessing(dtos);
        return result;
    }

    async setInProgress(dtos: AzureClassesResponseDTO[]) {
        const result = await this.azureClassesRepository.setInProgress(dtos);
        return result;
    }

    async processSendCreated(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.setInProcessing(dtos);
    }

    async processSendNotCreated(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.NOT_CREATED);
        await this.handleFailedCreation(dtos);
    }

    async processSendValidationError(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCreationErrorMessage(
            dtos,
            TelelinkCreateEventResponseEnum.VALIDATION_ERROR,
        );
        await this.handleFailedCreation(dtos);
    }

    async processSendException(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.EXCEPTION);
        await this.handleFailedCreation(dtos);
    }

    async processSendError(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.ERROR);
        await this.handleFailedCreation(dtos);
    }

    async processCheckDone(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        const groupedWorkflows = await this.groupWorkflowsByType(dtos);
        await this.setHasCompletedForCreate(groupedWorkflows[WorkflowType.CLASS_CREATE]);
        await this.setHasCompletedForUpdate(groupedWorkflows[WorkflowType.CLASS_UPDATE]);
        await this.setHasCompletedForDelete(groupedWorkflows[WorkflowType.CLASS_DELETE]);
    }

    async setHasCompletedForCreate(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        for (const dto of dtos) {
            const createClass = dto.telelinkResponseDto.response.data.actionsTriggered.filter((element) => {
                return element.action === ActionTypeEnum.CREATE_CLASS;
            })[0]?.data;
            const { azureId } = createClass;
            dto.azureID = azureId;
        }
        await this.setHasCompleted(dtos);
    }

    async setHasCompletedForDelete(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.setHasCompleted(dtos);
        return result;
    }

    async setHasCompletedForUpdate(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.setHasCompleted(dtos);
        return result;
    }

    async processCheckDLQ(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.DLQ);
        await this.handleFailedCheck(dtos);
        for (const dto of dtos) {
            if (this.shouldWorkflowBeRestarted(dto?.telelinkResponseDto.statusCode)) {
                await this.resendClassWorkflow(dto);
            }
        }
    }

    async processCheckInProgress(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesArchiveService.isForArchivation(dtos);
        await this.setInProgress(dtos);
    }

    async processCheckFailed(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.FAILED);
        await this.handleFailedCheck(dtos);
    }

    async processCheckNotFound(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.NOT_FOUND);
        await this.handleFailedCheck(dtos);
    }

    async processCheckOther(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.OTHER);
        await this.handleFailedCheck(dtos);
    }

    async processCheckException(dtos: AzureClassesResponseDTO[]) {
        if (!dtos?.length) return;
        this.classesErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.EXCEPTION);
        await this.handleFailedCheck(dtos);
    }

    async processSyncCreate(dtos: AzureClassesResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.classesService.syncCreateAzureClass(dto);
            if (!result) {
                this.classesErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.CLASS_CREATE);
                await this.handleFailedSync([dto]);
            }
        }
    }

    async processSyncUpdate(dtos: AzureClassesResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.classesService.syncUpdateAzureClass(dto);
            if (!result) {
                this.classesErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.CLASS_UPDATE);
                await this.handleFailedSync([dto]);
            }
        }
    }

    async processSyncDelete(dtos: AzureClassesResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.classesService.syncDeleteAzureClass(dto);
            if (!result) {
                this.classesErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.CLASS_DELETE);
                await this.handleFailedSync([dto]);
            }
        }
    }

    async sendAzureClasses(workflowType: WorkflowType) {
        const workflows = await this.getAllAzureClassesForSending(workflowType);
        const sendWorkflowResponse = await this.sendWorkflows(workflows);
        const groupedWorkflows = this.groupWorkflowsByStatus(sendWorkflowResponse);
        PrintWorkflowStatisticsService.print(workflowType, `sendAzureClasses`, null, groupedWorkflows);
        await this.processSendCreated(groupedWorkflows[TelelinkCreateEventResponseEnum.CREATED]);
        await this.processSendNotCreated(groupedWorkflows[TelelinkCreateEventResponseEnum.NOT_CREATED]);
        await this.processSendValidationError(groupedWorkflows[TelelinkCreateEventResponseEnum.VALIDATION_ERROR]);
        await this.processSendException(groupedWorkflows[TelelinkCreateEventResponseEnum.EXCEPTION]);
        await this.processSendError(groupedWorkflows[TelelinkCreateEventResponseEnum.ERROR]);
    }

    async checkAzureClasses() {
        const workflows = await this.getAllAzureClassesForChecking();
        const checkWorkflowResponse = await this.checkWorkflows(workflows);
        const groupedWorkflows = await this.groupWorkflowsByStatus(checkWorkflowResponse);
        PrintWorkflowStatisticsService.print(null, `checkAzureClasses`, null, groupedWorkflows);
        await this.processCheckDone(groupedWorkflows[TelelinkCheckStatusResponseEnum.DONE]);
        await this.processCheckDLQ(groupedWorkflows[TelelinkCheckStatusResponseEnum.DLQ]);
        await this.processCheckInProgress(groupedWorkflows[TelelinkCheckStatusResponseEnum.IN_PROGRESS]);
        await this.processCheckFailed(groupedWorkflows[TelelinkCheckStatusResponseEnum.FAILED]);
        await this.processCheckNotFound(groupedWorkflows[TelelinkCheckStatusResponseEnum.NOT_FOUND]);
        await this.processCheckOther(groupedWorkflows[TelelinkCheckStatusResponseEnum.OTHER]);
        await this.processCheckException(groupedWorkflows[TelelinkCheckStatusResponseEnum.EXCEPTION]);
    }

    async syncClasses() {
        const azureClasses = await this.getAllAzureClassesForCreating();
        const groupedWorkflows = await this.groupWorkflowsByType(azureClasses);
        await this.processSyncCreate(groupedWorkflows[WorkflowType.CLASS_CREATE]);
        await this.processSyncUpdate(groupedWorkflows[WorkflowType.CLASS_UPDATE]);
        await this.processSyncDelete(groupedWorkflows[WorkflowType.CLASS_DELETE]);
    }

    shouldWorkflowBeRestarted(statusCode: TelelinkStatusCodeEnum) {
        if (statusCode === TelelinkStatusCodeEnum.FAILURE) {
            return true;
        }
        return false;
    }

    async handleFailedCreation(dtos: AzureClassesResponseDTO[]) {
        const getDTOsForRestart = this.getDTOsForRestart(dtos);
        await this.classesRestartService.restartFailedWorkflows(getDTOsForRestart);
        dtos = this.removeDTOsForRestartFromOriginalArray(dtos);
        this.classesArchiveService.isForArchivation(dtos);
        await this.setHasFailedCreation(dtos);
    }

    async handleFailedCheck(dtos: AzureClassesResponseDTO[]) {
        const getDTOsForRestart = this.getDTOsForRestart(dtos);
        await this.classesRestartService.restartFailedWorkflows(getDTOsForRestart);
        dtos = this.removeDTOsForRestartFromOriginalArray(dtos);
        this.classesArchiveService.isForArchivation(dtos);
        await this.setHasFailed(dtos);
    }

    getDTOsForRestart(dtos: AzureClassesResponseDTO[]) {
        return dtos.filter((dto) => this.classesRestartService.isForRestart(dto));
    }

    async handleFailedSync(dtos: AzureClassesResponseDTO[]) {
        await this.classesArchiveService.isForArchivation(dtos);
        await this.setHasFailedSyncronization(dtos);
    }

    removeDTOsForRestartFromOriginalArray(dtos: AzureClassesResponseDTO[]) {
        dtos = dtos.filter((dto) => !this.classesRestartService.isForRestart(dto));
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

    groupWorkflowsByType(workflowResponses: any = []): {
        [key in WorkflowType]: any[];
    } {
        return workflowResponses.reduce((groups: any, response) => {
            const workflowType = response.workflowType;
            if (!groups[workflowType]) groups[workflowType] = [];
            groups[workflowType].push(response);
            return groups;
        }, {});
    }

    async sendWorkflows(workflows: AzureClassesResponseDTO[]) {
        const results = [];
        for (const workflow of workflows) {
            const eventDTO = EventDtoFactory.createFromClassResponseDTO(workflow);
            const response = await this.telelinkService.createEvent(eventDTO);
            const status = response.status;
            workflow.telelinkResponseDto = response;
            results.push({ workflow, status });
        }
        return results;
    }

    async checkWorkflows(workflows: AzureClassesResponseDTO[]) {
        const results = [];
        for (const workflow of workflows) {
            const response = await this.telelinkService.checkEvent(workflow.guid);
            const status = response.status;
            workflow.telelinkResponseDto = response;
            results.push({ workflow, status });
        }
        return results;
    }
}
