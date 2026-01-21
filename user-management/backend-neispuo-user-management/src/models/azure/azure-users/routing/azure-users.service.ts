/* eslint-disable prettier/prettier */
import { Inject, Injectable, Logger, forwardRef } from '@nestjs/common';
import { ActionTypeEnum } from 'src/common/constants/enum/action-type.enum';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { TelelinkCheckStatusResponseEnum } from 'src/common/constants/enum/telelink-check-status-response.enum';
import { TelelinkCreateEventResponseEnum } from 'src/common/constants/enum/telelink-create-event-response.enum';
import { TelelinkStatusCodeEnum } from 'src/common/constants/enum/telelink-status-code.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { EntitiesInGenerationDTO } from 'src/common/dto/entities-in-generation.dto';
import { ArchivedResourceQueryDTO } from 'src/common/dto/requests/archived-resource-query.dto';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { EventDtoFactory } from 'src/common/factories/event-dto.factory';
import { PrintWorkflowStatisticsService } from 'src/common/services/print-workflow-statistics/print-workflow-statistics.service';
import { EntitiesInGenerationService } from 'src/models/entities-in-generation/routing/entities-in-generation.service';
import { TelelinkService } from 'src/models/telelink/routing/telelink.service';
import { UserService } from 'src/models/user/routing/user.service';
import { EntityManager } from 'typeorm';
import { AzureUsersRepository } from '../azure-users.repository';
import { UsersArchiveService } from './users-archive.service';
import { UsersErrorService } from './users-error.service';
import { UsersRestartService } from './users-restart.service';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['getAzureUserStatus'],
})
export class AzureUsersService {
    private logger = new Logger(AzureUsersService.name);

    constructor(
        @Inject(forwardRef(() => AzureUsersRepository)) private azureUsersRepository: AzureUsersRepository,
        @Inject(forwardRef(() => TelelinkService)) private telelinkService: TelelinkService,
        @Inject(forwardRef(() => UserService)) private readonly userService: UserService,
        private entitiesInGenerationService: EntitiesInGenerationService,
        private usersArchiveService: UsersArchiveService,
        private usersErroService: UsersErrorService,
        private usersRestartService: UsersRestartService,
    ) {}

    async revertAzureUsers() {
        await this.azureUsersRepository.revertAzureUsers();
    }

    async getAllAzureUsersForSending(workflowType: WorkflowType) {
        const result = await this.azureUsersRepository.getAllAzureUsersForSending(workflowType);
        return result;
    }

    async getAllAzureUsersForChecking() {
        const result = await this.azureUsersRepository.getAllAzureUsersForChecking();
        return result;
    }

    async getAllAzureUsersForCreating() {
        const result = await this.azureUsersRepository.getAllAzureUsersForCreating();
        return result;
    }

    async getAzureUserStatus(id: number) {
        const result = await this.azureUsersRepository.getAzureUserStatus({ rowID: id });
        return result;
    }

    async resendUserWorkflow(dto: AzureUsersResponseDTO) {
        const result = await this.azureUsersRepository.resendWorkflow(dto);
        return result;
    }

    async stopUserWorkflow(dto: AzureUsersResponseDTO) {
        const result = await this.azureUsersRepository.stopWorkflow(dto);
        return result;
    }

    async setHasCompletedForCreate(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        for (const dto of dtos) {
            const createUser = dto.telelinkResponseDto.response.data.actionsTriggered.filter((element) => {
                return element.action === ActionTypeEnum.CREATE_USER;
            })[0]?.data;
            const { username, initialPassword, azureId } = createUser;
            dto.azureID = azureId;
            dto.username = username;
            dto.password = initialPassword;
        }
        const result = await this.azureUsersRepository.setHasCompleted(dtos);
        return result;
    }

    async setHasCompletedForUpdate(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        for (const dto of dtos) {
            dto.username = null;
            dto.password = null;
            dto.userRole = null;
        }
        const result = await this.azureUsersRepository.setHasCompleted(dtos);
        return result;
    }

    async setHasCompletedForDelete(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        for (const dto of dtos) {
            dto.username = null;
            dto.password = null;
            dto.userRole = null;
        }
        const result = await this.azureUsersRepository.setHasCompleted(dtos);
        return result;
    }

    async setHasFailed(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureUsersRepository.setHasFailed(dtos);
        return result;
    }

    async setHasFailedCreation(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureUsersRepository.setHasFailedCreation(dtos);
        return result;
    }

    async setHasFailedSyncronization(dtos: AzureUsersResponseDTO[]) {
        const result = await this.azureUsersRepository.setHasFailedSyncronization(dtos);
        return result;
    }

    async setInProcessing(dtos: AzureUsersResponseDTO[]) {
        const result = await this.azureUsersRepository.setInProcessing(dtos);
        return result;
    }

    async setInProgress(dtos: AzureUsersResponseDTO[]) {
        const result = await this.azureUsersRepository.setInProgress(dtos);
        return result;
    }

    async setSyncronized(dto: AzureUsersResponseDTO) {
        const result = await this.azureUsersRepository.setSyncronized(dto);
        return result;
    }

    async processSendCreated(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        const result = await this.azureUsersRepository.setInProcessing(dtos);
    }

    async processSendNotCreated(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.NOT_CREATED);
        await this.handleFailedCreation(dtos);
    }

    async processSendValidationError(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.VALIDATION_ERROR);
        await this.handleFailedCreation(dtos);
    }

    async processSendException(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.EXCEPTION);
        await this.handleFailedCreation(dtos);
    }

    async processSendError(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCreationErrorMessage(dtos, TelelinkCreateEventResponseEnum.ERROR);
        await this.handleFailedCreation(dtos);
    }

    async processCheckDone(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        const groupedWorkflows = await this.groupWorkflowsByType(dtos);
        await this.setHasCompletedForCreate(groupedWorkflows[WorkflowType.USER_CREATE]);
        await this.setHasCompletedForUpdate(groupedWorkflows[WorkflowType.USER_UPDATE]);
        await this.setHasCompletedForDelete(groupedWorkflows[WorkflowType.USER_DELETE]);
    }

    async processCheckDLQ(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.DLQ);
        await this.handleFailedCheck(dtos);
        for (const dto of dtos) {
            if (this.shouldWorkflowBeRestarted(dto?.telelinkResponseDto.statusCode)) {
                await this.resendUserWorkflow(dto);
            }
        }
    }

    async processCheckInProgress(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersArchiveService.isForArchivation(dtos);
        await this.setInProgress(dtos);
    }

    async processCheckFailed(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.FAILED);
        await this.handleFailedCheck(dtos);
        for (const dto of dtos) {
            if (this.shouldWorkflowBeStopped(dto?.telelinkResponseDto.statusCode)) {
                await this.stopUserWorkflow(dto);
            }
        }
    }

    async processCheckNotFound(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.NOT_FOUND);
        await this.handleFailedCheck(dtos);
    }

    async processCheckOther(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.OTHER);
        await this.handleFailedCheck(dtos);
    }

    async processCheckException(dtos: AzureUsersResponseDTO[]) {
        if (!dtos?.length) return;
        this.usersErroService.handleFailedCheckErrorMessage(dtos, TelelinkCheckStatusResponseEnum.EXCEPTION);
        await this.handleFailedCheck(dtos);
    }

    async processSyncCreate(dtos: AzureUsersResponseDTO[] = []) {
        await this.userService.logSysUserAudit(dtos);
        const groupedWorkflows = await this.groupWorkflowsByUserRole(dtos);
        await this.userService.syncCreateAzureStudentUsers(groupedWorkflows[UserRoleType.STUDENT]);
        await this.userService.syncCreateAzureTeacherUsers(groupedWorkflows[UserRoleType.TEACHER]);
        await this.userService.syncCreateAzureParentUsers(groupedWorkflows[UserRoleType.PARENT]);
    }

    async processSyncUpdate(dtos: AzureUsersResponseDTO[] = []) {
        const groupedWorkflows = await this.groupWorkflowsByUserRole(dtos);
        await this.userService.syncUpdateAzureStudentUsers(groupedWorkflows[UserRoleType.STUDENT]);
        await this.userService.syncUpdateAzureTeacherUsers(groupedWorkflows[UserRoleType.TEACHER]);
        await this.userService.syncUpdateAzureParentUsers(groupedWorkflows[UserRoleType.PARENT]);
    }

    async processSyncDelete(dtos: AzureUsersResponseDTO[] = []) {
        const groupedWorkflows = await this.groupWorkflowsByUserRole(dtos);
        await this.userService.syncDeleteAzureStudentUsers(groupedWorkflows[UserRoleType.STUDENT]);
        await this.userService.syncDeleteAzureTeacherUsers(groupedWorkflows[UserRoleType.TEACHER]);
        await this.userService.syncDeleteAzureParentUsers(groupedWorkflows[UserRoleType.PARENT]);
    }

    async sendAzureUsers(workflowType: WorkflowType) {
        const workflows = await this.getAllAzureUsersForSending(workflowType);
        const sendWorkflowResponse = await this.sendWorkflows(workflows);
        const groupedWorkflows = this.groupWorkflowsByStatus(sendWorkflowResponse);
        PrintWorkflowStatisticsService.print(workflowType, `sendAzureUsers`, null, groupedWorkflows);
        await this.processSendCreated(groupedWorkflows[TelelinkCreateEventResponseEnum.CREATED]);
        await this.processSendNotCreated(groupedWorkflows[TelelinkCreateEventResponseEnum.NOT_CREATED]);
        await this.processSendValidationError(groupedWorkflows[TelelinkCreateEventResponseEnum.VALIDATION_ERROR]);
        await this.processSendException(groupedWorkflows[TelelinkCreateEventResponseEnum.EXCEPTION]);
        await this.processSendError(groupedWorkflows[TelelinkCreateEventResponseEnum.ERROR]);
    }

    async checkAzureUsers() {
        const workflows = await this.getAllAzureUsersForChecking();
        const checkWorkflowResponse = await this.checkWorkflows(workflows);
        const groupedWorkflows = await this.groupWorkflowsByStatus(checkWorkflowResponse);
        PrintWorkflowStatisticsService.print(null, `checkAzureUsers`, null, groupedWorkflows);
        await this.processCheckDone(groupedWorkflows[TelelinkCheckStatusResponseEnum.DONE]);
        await this.processCheckDLQ(groupedWorkflows[TelelinkCheckStatusResponseEnum.DLQ]);
        await this.processCheckInProgress(groupedWorkflows[TelelinkCheckStatusResponseEnum.IN_PROGRESS]);
        await this.processCheckFailed(groupedWorkflows[TelelinkCheckStatusResponseEnum.FAILED]);
        await this.processCheckNotFound(groupedWorkflows[TelelinkCheckStatusResponseEnum.NOT_FOUND]);
        await this.processCheckOther(groupedWorkflows[TelelinkCheckStatusResponseEnum.OTHER]);
        await this.processCheckException(groupedWorkflows[TelelinkCheckStatusResponseEnum.EXCEPTION]);
    }

    async syncUsers() {
        const azureUsers = await this.getAllAzureUsersForCreating();
        const groupedWorkflows = this.groupWorkflowsByType(azureUsers);
        await this.processSyncCreate(groupedWorkflows[WorkflowType.USER_CREATE]);
        await this.processSyncUpdate(groupedWorkflows[WorkflowType.USER_UPDATE]);
        await this.processSyncDelete(groupedWorkflows[WorkflowType.USER_DELETE]);
    }

    shouldWorkflowBeRestarted(statusCode: TelelinkStatusCodeEnum) {
        if (statusCode === TelelinkStatusCodeEnum.FAILURE) {
            return true;
        }
        return false;
    }

    shouldWorkflowBeStopped(statusCode: TelelinkStatusCodeEnum) {
        if (statusCode === TelelinkStatusCodeEnum.USER_EXISTS) {
            return true;
        }
        return false;
    }

    isAzureUserAlreadyCreated(dto: AzureUsersResponseDTO) {
        //already created users in azure are returned to us as new users without password. this is why the check looks like this.
        const { password } = dto;
        if (password) {
            return false;
        }
        return true;
    }

    async generateUsername(dto: AzureUsersResponseDTO, manager?: EntityManager) {
        const result = await this.azureUsersRepository.generateUsername(dto, manager);
        return result;
    }

    userHasFailedUserNameGeneration(dto: AzureUsersResponseDTO) {
        if (dto?.status && dto.status === EventStatus[EventStatus.FAILED_USERNAME_GENERATION]) {
            return true;
        }
        return false;
    }

    async handleFailedCreation(dtos: AzureUsersResponseDTO[]) {
        await this.deleteEntitiesInGenerationByIdentifier(dtos);
        const getDTOsForRestart = this.getDTOsForRestart(dtos);
        await this.usersRestartService.restartFailedWorkflows(getDTOsForRestart);
        dtos = this.removeDTOsForRestartFromOriginalArray(dtos);
        this.usersArchiveService.isForArchivation(dtos);
        await this.setHasFailedCreation(dtos);
    }

    async handleFailedCheck(dtos: AzureUsersResponseDTO[]) {
        await this.deleteEntitiesInGenerationByIdentifier(dtos);
        const getDTOsForRestart = this.getDTOsForRestart(dtos);
        await this.usersRestartService.restartFailedWorkflows(getDTOsForRestart);
        dtos = this.removeDTOsForRestartFromOriginalArray(dtos);
        this.usersArchiveService.isForArchivation(dtos);
        await this.setHasFailed(dtos);
    }

    getDTOsForRestart(dtos: AzureUsersResponseDTO[]) {
        return dtos.filter((dto) => this.usersRestartService.isForRestart(dto));
    }

    async handleFailedSync(dtos: AzureUsersResponseDTO[]) {
        await this.deleteEntitiesInGenerationByIdentifier(dtos);
        await this.usersArchiveService.isForArchivation(dtos);
        await this.setHasFailedSyncronization(dtos);
    }

    removeDTOsForRestartFromOriginalArray(dtos: AzureUsersResponseDTO[]) {
        dtos = dtos.filter((dto) => !this.usersRestartService.isForRestart(dto));
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

    groupWorkflowsByUserRole(workflowResponses: any = []): {
        [key in UserRoleType]: any[];
    } {
        return workflowResponses.reduce((groups: any, response) => {
            const userRole = response?.userRole;
            if (!userRole) return groups;
            if (!groups[userRole]) groups[userRole] = [];
            groups[userRole].push(response);
            return groups;
        }, {});
    }

    async sendWorkflows(workflows: AzureUsersResponseDTO[]) {
        const results = [];
        for (const workflow of workflows) {
            const eventDTO = EventDtoFactory.createFromUserResponseDTO(workflow);
            const response = await this.telelinkService.createEvent(eventDTO);
            const status = response.status;
            workflow.telelinkResponseDto = response;
            results.push({ workflow, status });
        }
        return results;
    }

    async checkWorkflows(workflows: AzureUsersResponseDTO[]) {
        const results = [];
        for (const workflow of workflows) {
            const response = await this.telelinkService.checkEvent(workflow.guid);
            const status = response.status;
            workflow.telelinkResponseDto = response;
            results.push({ workflow, status });
        }
        return results;
    }

    async deleteEntitiesInGenerationByIdentifier(dtos: AzureUsersResponseDTO[]) {
        const identifiers = dtos.map((dto) => {
            return <EntitiesInGenerationDTO>{
                identifier: dto.personID.toString(), // spread the original element object
            };
        });
        await this.entitiesInGenerationService.deleteEntitiesInGenerationByIdentifiers(identifiers);
    }

    async generatedUserExist(dto: AzureUsersResponseDTO) {
        const result = await this.azureUsersRepository.getGeneratedUser(dto);
        return result ? true : false;
    }

    async getArchivedPreviousYears(query: ArchivedResourceQueryDTO) {
        return this.azureUsersRepository.getArchivedPreviousYears(query);
    }
}
