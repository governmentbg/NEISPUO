import { Inject, Injectable, Logger, forwardRef } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { ActionTypeEnum } from 'src/common/constants/enum/action-type.enum';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { TelelinkCheckStatusResponseEnum } from 'src/common/constants/enum/telelink-check-status-response.enum';
import { TelelinkCreateEventResponseEnum } from 'src/common/constants/enum/telelink-create-event-response.enum';
import { TelelinkStatusCodeEnum } from 'src/common/constants/enum/telelink-status-code.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { EnrollmentUserToSchoolCreateRequestDTO } from 'src/common/dto/requests/enrollment-user-to-school-create-request.dto';
import { OrganizationCreateRequestDTO } from 'src/common/dto/requests/organization-create-request.dto';
import { OrganizationDeleteRequestDTO } from 'src/common/dto/requests/organization-delete-request.dto';
import { OrganizationUpdateRequestDTO } from 'src/common/dto/requests/organization-update-request.dto';
import { AzureOrganizationsResponseDTO } from 'src/common/dto/responses/azure-organizations-response.dto';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { AzureOrganizationResponseFactory } from 'src/common/factories/azure-organization-response-dto.factory';
import { EventDtoFactory } from 'src/common/factories/event-dto.factory';
import { PrintWorkflowStatisticsService } from 'src/common/services/print-workflow-statistics/print-workflow-statistics.service';
import { AccountantService } from 'src/models/accountant/routing/accountant.service';
import { EducationalStateService } from 'src/models/educational-state/routing/educational-state.service';
import { InstitutionService } from 'src/models/institution/routing/institution.service';
import { PersonService } from 'src/models/person/routing/person.service';
import { TelelinkService } from 'src/models/telelink/routing/telelink.service';
import { UserService } from 'src/models/user/routing/user.service';
import { Connection } from 'typeorm';
import { AzureEnrollmentsService } from '../../azure-enrollments/routing/azure-enrollments.service';
import { AzureOrganizationsRepository } from '../azure-organizations.repository';
import { OrganizationsArchiveService } from './organizations-archive.service';
import { OrganizationsErrorService } from './organizations-error.service';
import { OrganizationsRestartService } from './organizations-restart.service';
import { ArchivedResourceQueryDTO } from 'src/common/dto/requests/archived-resource-query.dto';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: [
        'createAzureOrganization',
        'deleteAzureOrganization',
        'updateAzureOrganization',
        'getAzureOrganizationStatus',
        'syncUnsyncedInstitutions',
    ],
})
export class AzureOrganizationsService {
    constructor(
        private azureOrganizationsRepository: AzureOrganizationsRepository,
        private telelinkService: TelelinkService,
        private institutionService: InstitutionService,
        @Inject(forwardRef(() => UserService))
        private readonly userService: UserService,
        private readonly personService: PersonService,
        private readonly educationalStateService: EducationalStateService,
        @Inject(forwardRef(() => AzureEnrollmentsService))
        private readonly enrollmentsService: AzureEnrollmentsService,
        private accountantService: AccountantService,
        private organizationsArchiveService: OrganizationsArchiveService,
        private organizationsRestartService: OrganizationsRestartService,
        private organizationsErrorService: OrganizationsErrorService,
        private connection: Connection,
    ) {}

    async revertAzureOrganizations() {
        await this.azureOrganizationsRepository.revertAzureOrganizations();
    }

    async createAzureOrganization(createOrganizationRequestDTO: OrganizationCreateRequestDTO) {
        const institution = await this.institutionService.getInstitutionByInstitutionID(
            createOrganizationRequestDTO.institutionID,
        );
        if (!institution?.institutionID) throw new DataNotFoundException();
        const azureOrganizationDTO = AzureOrganizationResponseFactory.createFromInstitutionResponseDTO(institution);
        const result = await this.azureOrganizationsRepository.insertAzureOrganization(azureOrganizationDTO);
        if (!result?.rowID) throw new EntityNotCreatedException();
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_ORGANIZATION_CREATED]: result.rowID,
            },
        };
    }

    async deleteAzureOrganization(organizationDeleteRequestDTO: OrganizationDeleteRequestDTO) {
        // implement a check to see if the thing exists.
        const institution = await this.institutionService.getInstitutionByInstitutionID(
            organizationDeleteRequestDTO.institutionID,
        );
        if (!institution?.institutionID) throw new DataNotFoundException();
        const azureOrganizationDTO = AzureOrganizationResponseFactory.createFromInstitutionResponseDTO(institution);
        const result = await this.azureOrganizationsRepository.deleteAzureOrganization(azureOrganizationDTO);
        if (!result?.rowID) throw new EntityNotCreatedException();
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_ORGANIZATION_CREATED]: result.rowID,
            },
        };
    }

    async updateAzureOrganization(updateOrganizationRequestDTO: OrganizationUpdateRequestDTO) {
        // implement a check to see if the thing exists.
        const institution = await this.institutionService.getInstitutionByInstitutionID(
            updateOrganizationRequestDTO.institutionID,
        );
        if (!institution?.institutionID) throw new DataNotFoundException();
        const azureOrganizationDTO = AzureOrganizationResponseFactory.createFromInstitutionResponseDTO(institution);
        const result = await this.azureOrganizationsRepository.updateAzureOrganization(azureOrganizationDTO);
        if (!result) throw new EntityNotCreatedException();
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_ORGANIZATION_CREATED]: azureOrganizationDTO.rowID,
            },
        };
    }

    async restoreAzureOrganization(createOrganizationRequestDTO: OrganizationCreateRequestDTO) {
        const { institutionID } = createOrganizationRequestDTO;
        const institution = await this.institutionService.getDeletedInstitutionByInstitutionID(institutionID);
        if (!institution?.institutionID) throw new DataNotFoundException();
        const azureOrganizationDTO = AzureOrganizationResponseFactory.createFromInstitutionResponseDTO(institution);
        let result,
            result2,
            result3,
            // eslint-disable-next-line prefer-const
            result4 = [],
            result5;
        await this.connection.transaction(async (manager) => {
            result = await this.azureOrganizationsRepository.insertAzureOrganization(azureOrganizationDTO, manager);
            result2 = await this.educationalStateService.getUserEducationalStatesByInstitutionID({
                institutionID,
            });
            if (!result?.rowID) throw new EntityNotCreatedException();
            for (const educationalState of result2) {
                const { personID, institutionID, positionID } = educationalState;
                const userRole = positionID === 2 ? UserRoleType.TEACHER : UserRoleType.STUDENT;
                const dto: EnrollmentUserToSchoolCreateRequestDTO = { personID, institutionID, userRole };
                result3 = await this.enrollmentsService.createAzureEnrollmentUserToSchool(dto, manager);
                result4.push(result3.data);
            }
            result5 = await this.accountantService.getAccountantsByInstitutionID(institutionID);
            for (const accountant of result5) {
                const { personID, institutionID, positionID } = accountant;
                const userRole = positionID === 2 ? UserRoleType.TEACHER : UserRoleType.STUDENT;
                const dto: EnrollmentUserToSchoolCreateRequestDTO = { personID, institutionID, userRole };
                result3 = await this.enrollmentsService.createAzureEnrollmentUserToSchool(dto, manager);
                result4.push(result3.data);
            }
        });

        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_ORGANIZATION_CREATED]: result.rowID,
                [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: result4,
            },
        };
    }

    async getAllAzureOrganizationsForSending(workflowType: WorkflowType) {
        const result = await this.azureOrganizationsRepository.getAllAzureOrganizationsForSending(workflowType);
        return result;
    }

    async getAllAzureOrganizationsForChecking() {
        const result = await this.azureOrganizationsRepository.getAllAzureOrganizationsForChecking();
        return result;
    }

    async getAllAzureOrganizationsForCreating() {
        const result = await this.azureOrganizationsRepository.getAllAzureOrganizationsForCreating();
        return result;
    }

    async getAzureOrganizationStatus(id: number) {
        const result = await this.azureOrganizationsRepository.getAzureOrganizationStatus({ rowID: id });
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_EVENT_STATUS]: EventStatus[result.status],
            },
        };
    }

    async resendOrganizationWorkflow(dto: AzureOrganizationsResponseDTO) {
        const result = await this.azureOrganizationsRepository.resendWorkflow(dto);
        return result;
    }

    async setHasCompletedForCreate(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        for (const dto of dtos) {
            const createPrincipal = dto.telelinkResponseDto.response.data.actionsTriggered.filter((element) => {
                return element.action === ActionTypeEnum.CREATE_PRINCIPAL;
            })[0]?.data;
            const createSchool = dto.telelinkResponseDto.response.data.actionsTriggered.filter((element) => {
                return element.action === ActionTypeEnum.CREATE_SCHOOL;
            })[0]?.data;
            const { username, initialPassword } = createPrincipal;
            const { azureId } = createSchool;

            dto.azureID = azureId;
            dto.username = username;
            dto.password = initialPassword;
        }
        const result = await this.azureOrganizationsRepository.setHasCompleted(dtos);
        return result;
    }

    async setHasCompletedForDelete(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureOrganizationsRepository.setHasCompleted(dtos);
        return result;
    }

    async setHasCompletedForUpdate(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureOrganizationsRepository.setHasCompleted(dtos);
        return result;
    }

    async setHasFailed(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureOrganizationsRepository.setHasFailed(dtos);
        return result;
    }

    async setHasFailedCreation(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureOrganizationsRepository.setHasFailedCreation(dtos);
        return result;
    }

    async setHasFailedSyncronization(dtos: AzureOrganizationsResponseDTO[]) {
        const result = await this.azureOrganizationsRepository.setHasFailedSyncronization(dtos);
        return result;
    }

    async setInProcessing(dtos: AzureOrganizationsResponseDTO[]) {
        const result = await this.azureOrganizationsRepository.setInProcessing(dtos);
        return result;
    }

    async setInProgress(dtos: AzureOrganizationsResponseDTO[]) {
        const result = await this.azureOrganizationsRepository.setInProgress(dtos);
        return result;
    }

    async processSendCreated(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.setInProcessing(dtos);
    }

    async processSendNotCreated(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCreationErrorMessage(
            dtos,
            TelelinkCreateEventResponseEnum.NOT_CREATED,
        );
        await this.handleFailedCreation(dtos);
    }

    async processSendValidationError(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCreationErrorMessage(
            dtos,
            TelelinkCreateEventResponseEnum.VALIDATION_ERROR,
        );
        await this.handleFailedCreation(dtos);
    }

    async processSendException(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCreationErrorMessage(
            dtos,
            TelelinkCreateEventResponseEnum.EXCEPTION,
        );
        await this.handleFailedCreation(dtos);
    }

    async processSendError(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.ERROR);
        await this.handleFailedCreation(dtos);
    }

    async processCheckDone(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        const groupedWorkflows = await this.groupWorkflowsByType(dtos);
        await this.setHasCompletedForCreate(groupedWorkflows[WorkflowType.SCHOOL_CREATE]);
        await this.setHasCompletedForUpdate(groupedWorkflows[WorkflowType.SCHOOL_UPDATE]);
        await this.setHasCompletedForDelete(groupedWorkflows[WorkflowType.SCHOOL_DELETE]);
    }

    async processCheckDLQ(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.DLQ);
        await this.handleFailedCheck(dtos);
        for (const dto of dtos) {
            if (this.shouldWorkflowBeRestarted(dto?.telelinkResponseDto.statusCode)) {
                await this.resendOrganizationWorkflow(dto);
            }
        }
    }

    async processCheckInProgress(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsArchiveService.isForArchivation(dtos);
        await this.setInProgress(dtos);
    }

    async processCheckFailed(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.FAILED);
        await this.handleFailedCheck(dtos);
        for (const dto of dtos) {
            if (this.shouldWorkflowBeRestarted(dto?.telelinkResponseDto.statusCode)) {
                await this.resendOrganizationWorkflow(dto);
            }
        }
    }

    async processCheckNotFound(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.NOT_FOUND);
        await this.handleFailedCheck(dtos);
    }

    async processCheckOther(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.OTHER);
        await this.handleFailedCheck(dtos);
    }

    async processCheckException(dtos: AzureOrganizationsResponseDTO[]) {
        if (!dtos?.length) return;
        this.organizationsErrorService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.EXCEPTION);
        await this.handleFailedCheck(dtos);
    }

    async processSyncCreate(dtos: AzureOrganizationsResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.userService.syncCreateInstitutionUsers(dto);
            if (!result) {
                this.organizationsErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.SCHOOL_CREATE);
                await this.handleFailedSync([dto]);
            }
        }
    }

    async processSyncUpdate(dtos: AzureOrganizationsResponseDTO[] = []) {
        for (const dto of dtos) {
            const person = await this.personService.getPersonByInstitutionID(dto.organizationID);
            dto.personID = person?.personID;
            const result = await this.userService.syncUpdateInstitutionUsers(dto);
            if (!result) {
                this.organizationsErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.SCHOOL_UPDATE);
                await this.handleFailedSync([dto]);
            }
        }
    }

    async processSyncDelete(dtos: AzureOrganizationsResponseDTO[] = []) {
        for (const dto of dtos) {
            const person = await this.personService.getPersonByInstitutionID(dto.organizationID);
            dto.personID = person?.personID;
            const result = await this.userService.syncDeleteAzureInstitutionUser(dto);
            if (!result) {
                this.organizationsErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.SCHOOL_DELETE);
                await this.handleFailedSync([dto]);
            }
        }
    }

    async sendAzureOrganizations(workflowType: WorkflowType) {
        const workflows = await this.getAllAzureOrganizationsForSending(workflowType);
        const sendWorkflowResponse = await this.sendWorkflows(workflows);
        const groupedWorkflows = this.groupWorkflowsByStatus(sendWorkflowResponse);
        PrintWorkflowStatisticsService.print(workflowType, `sendAzureOrganizations`, null, groupedWorkflows);
        await this.processSendCreated(groupedWorkflows[TelelinkCreateEventResponseEnum.CREATED]);
        await this.processSendNotCreated(groupedWorkflows[TelelinkCreateEventResponseEnum.NOT_CREATED]);
        await this.processSendValidationError(groupedWorkflows[TelelinkCreateEventResponseEnum.VALIDATION_ERROR]);
        await this.processSendException(groupedWorkflows[TelelinkCreateEventResponseEnum.EXCEPTION]);
        await this.processSendError(groupedWorkflows[TelelinkCreateEventResponseEnum.ERROR]);
    }

    async checkAzureOrganizations() {
        const workflows = await this.getAllAzureOrganizationsForChecking();
        const checkWorkflowResponse = await this.checkWorkflows(workflows);
        const groupedWorkflows = await this.groupWorkflowsByStatus(checkWorkflowResponse);
        PrintWorkflowStatisticsService.print(null, `checkAzureOrganizations`, null, groupedWorkflows);
        await this.processCheckDone(groupedWorkflows[TelelinkCheckStatusResponseEnum.DONE]);
        await this.processCheckDLQ(groupedWorkflows[TelelinkCheckStatusResponseEnum.DLQ]);
        await this.processCheckInProgress(groupedWorkflows[TelelinkCheckStatusResponseEnum.IN_PROGRESS]);
        await this.processCheckFailed(groupedWorkflows[TelelinkCheckStatusResponseEnum.FAILED]);
        await this.processCheckNotFound(groupedWorkflows[TelelinkCheckStatusResponseEnum.NOT_FOUND]);
        await this.processCheckOther(groupedWorkflows[TelelinkCheckStatusResponseEnum.OTHER]);
        await this.processCheckException(groupedWorkflows[TelelinkCheckStatusResponseEnum.EXCEPTION]);
    }

    async syncInstitutions() {
        const azureClasses = await this.getAllAzureOrganizationsForCreating();
        const groupedWorkflows = await this.groupWorkflowsByType(azureClasses);
        await this.processSyncCreate(groupedWorkflows[WorkflowType.SCHOOL_CREATE]);
        await this.processSyncUpdate(groupedWorkflows[WorkflowType.SCHOOL_UPDATE]);
        await this.processSyncDelete(groupedWorkflows[WorkflowType.SCHOOL_DELETE]);
    }

    shouldWorkflowBeRestarted(statusCode: TelelinkStatusCodeEnum) {
        if (statusCode === TelelinkStatusCodeEnum.FAILURE) {
            return true;
        }
        return false;
    }

    async syncUnsyncedInstitutions() {
        const institutions = await this.institutionService.getUnsyncedInstitutions();
        for (const institution of institutions) {
            try {
                await this.createAzureOrganization({ institutionID: institution.institutionID });
            } catch (e) {
                Logger.error(`syncUnsyncedInstitutionsJOB FAILED FOR institutionID: ${institution.institutionID}`);
            }
        }
    }

    async handleFailedCreation(dtos: AzureOrganizationsResponseDTO[]) {
        const getDTOsForRestart = this.getDTOsForRestart(dtos);
        await this.organizationsRestartService.restartFailedWorkflows(getDTOsForRestart);
        dtos = this.removeDTOsForRestartFromOriginalArray(dtos);
        this.organizationsArchiveService.isForArchivation(dtos);
        await this.setHasFailedCreation(dtos);
    }

    async handleFailedCheck(dtos: AzureOrganizationsResponseDTO[]) {
        const getDTOsForRestart = this.getDTOsForRestart(dtos);
        await this.organizationsRestartService.restartFailedWorkflows(getDTOsForRestart);
        dtos = this.removeDTOsForRestartFromOriginalArray(dtos);
        this.organizationsArchiveService.isForArchivation(dtos);
        await this.setHasFailed(dtos);
    }

    getDTOsForRestart(dtos: AzureOrganizationsResponseDTO[]) {
        return dtos.filter((dto) => this.organizationsRestartService.isForRestart(dto));
    }

    async handleFailedSync(dtos: AzureOrganizationsResponseDTO[]) {
        await this.organizationsArchiveService.isForArchivation(dtos);
        await this.setHasFailedSyncronization(dtos);
    }

    removeDTOsForRestartFromOriginalArray(dtos: AzureOrganizationsResponseDTO[]) {
        dtos = dtos.filter((dto) => !this.organizationsRestartService.isForRestart(dto));
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

    groupWorkflowsByType(workflowResponses: any[] = []): {
        [key in WorkflowType]: any[];
    } {
        return workflowResponses.reduce((groups: any, response) => {
            const workflowType = response.workflowType;
            if (!groups[workflowType]) groups[workflowType] = [];
            groups[workflowType].push(response);
            return groups;
        }, {});
    }

    async sendWorkflows(workflows: AzureOrganizationsResponseDTO[]) {
        const results = [];
        for (const workflow of workflows) {
            const eventDTO = EventDtoFactory.createFromOrganizationsResponseDTO(workflow);
            const response = await this.telelinkService.createEvent(eventDTO);
            const status = response.status;
            workflow.telelinkResponseDto = response;
            results.push({ workflow, status });
        }
        return results;
    }

    async checkWorkflows(workflows: AzureOrganizationsResponseDTO[]) {
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
        return this.azureOrganizationsRepository.getArchivedPreviousYears(query);
    }
}
