/* eslint-disable prefer-const */
import { Inject, Injectable, forwardRef } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { AuditActionEnum } from 'src/common/constants/enum/audit-log-action.enum';
import { AuditModuleEnum } from 'src/common/constants/enum/audit-module.enum';
import { IsAzureUser } from 'src/common/constants/enum/is-azure-user.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { MessageLevel } from 'src/common/constants/enum/siem/logger-level.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { AzureOrganizationsResponseDTO } from 'src/common/dto/responses/azure-organizations-response.dto';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { SysUserCreateDTO } from 'src/common/dto/sys-user-create.dto';
import { SysUserSysRoleCreateDTO } from 'src/common/dto/sys-user-sys-role-create.dto';
import { DateToUTCTransformService } from 'src/common/services/to-utc-date-transform/status-transform.service';
import { AzureUsersService } from 'src/models/azure/azure-users/routing/azure-users.service';
import { UsersErrorService } from 'src/models/azure/azure-users/routing/users-error.service';
import { AuditService } from 'src/models/logs/audit/routing/audit.service';
import { SIEMLogEventType } from 'src/models/siem-logger/siem-log-event-type.enum';
import { SIEMLoggerService } from 'src/models/siem-logger/siem-logger.service';
import { SystemUserService } from 'src/models/system-user/routing/system-user.service';
import { EntityManager } from 'typeorm';
import { UserRepository } from '../user.repository';
import { UserFindManyRequestDto } from 'src/common/dto/requests/user-find-many-request.dto';

@Injectable()
export class UserService {
    constructor(
        private userRepository: UserRepository,
        @Inject(forwardRef(() => AzureUsersService)) private azureUsersService: AzureUsersService,
        private auditService: AuditService,
        private systemUserService: SystemUserService,
        private usersErrorService: UsersErrorService,
        private readonly siemLoggerService: SIEMLoggerService,
    ) {}

    async getSysUserBySysUserID(sysUserID: number) {
        return this.userRepository.getSysUserBySysUserID(sysUserID);
    }

    async getSysUserByPersonID(sysUserID: number) {
        return this.userRepository.getSysUserByPersonID(sysUserID);
    }

    async getSysUsersByPersonID(sysUserID: number) {
        return this.userRepository.getSysUsersByPersonID(sysUserID);
    }

    async getSysUserByUsername(username: string) {
        return this.userRepository.getSysUserByUsername(username);
    }

    async hasUserRole(role: number, roles: any) {
        let result = false;
        for (let i = 0; i < roles.length; i += 1) {
            if (!!roles && (roles[i] === role || roles[i].sysRoleID === role)) {
                result = true;
                break;
            }
        }
        return result;
    }

    async syncUpdateAzureStudentUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.userRepository.updateAzureUserStudentProcedure(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_UPDATE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
    }

    async syncUpdateAzureTeacherUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.userRepository.updateAzureUserTeacherProcedure(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_UPDATE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
    }

    async syncUpdateAzureParentUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.userRepository.updateAzureParentProcedure(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_UPDATE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
    }

    async syncCreateAzureStudentUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            if (this.azureUsersService.isAzureUserAlreadyCreated(dto)) {
                await this.azureUsersService.setSyncronized(dto);
            }
            const result = await this.userRepository.createAzureUserProcedure(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_CREATE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
        // an already existing user should not be created again. this leads to certain fails. duplicate in core.sysUser.
    }

    async syncCreateAzureTeacherUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            if (this.azureUsersService.isAzureUserAlreadyCreated(dto)) {
                await this.azureUsersService.setSyncronized(dto);
            }
            const result = await this.userRepository.createAzureUserProcedure(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_CREATE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
    }

    async syncCreateAzureParentUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            if (this.azureUsersService.isAzureUserAlreadyCreated(dto)) {
                await this.azureUsersService.setSyncronized(dto);
            }
            const result = this.createAzureParent(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_CREATE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
    }

    async createAzureParent(dto: AzureUsersResponseDTO) {
        const result = await this.userRepository.createAzureParentProcedure(dto);
        return result;
    }

    async syncDeleteAzureStudentUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.userRepository.deleteAzureUserStudentProcedure(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_DELETE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
    }

    async syncDeleteAzureTeacherUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.userRepository.deleteAzureUserTeacherProcedure(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_DELETE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
    }

    async syncDeleteAzureParentUsers(dtos: AzureUsersResponseDTO[] = []) {
        for (const dto of dtos) {
            const result = await this.userRepository.deleteAzureParentProcedure(dto);
            if (!result) {
                this.usersErrorService.handleFailedSyncErrorMessage([dto], WorkflowType.USER_DELETE);
                await this.azureUsersService.handleFailedSync([dto]);
            }
        }
    }

    async syncCreateInstitutionUsers(dto: AzureOrganizationsResponseDTO) {
        return this.userRepository.createAzureInstitutionUserProcedure(dto);
    }

    async syncUpdateInstitutionUsers(dto: AzureOrganizationsResponseDTO) {
        return this.userRepository.updateAzureInstitutionUserProcedure(dto);
    }

    async syncDeleteAzureInstitutionUser(dto: AzureOrganizationsResponseDTO) {
        return this.userRepository.deleteAzureInstitutionUserProcedure(dto);
    }

    async getNonSyncedUsersPersonalIDsCount() {
        return this.userRepository.getNonSyncedUsersPersonalIDsCount();
    }

    async getUnemployedTeachersForDelete() {
        return this.userRepository.getUnemployedTeachersForDelete();
    }

    async getUnattendingStudentsForDelete() {
        return this.userRepository.getUnattendingStudentsForDelete();
    }

    async logSysUserAudit(dtos: AzureUsersResponseDTO[]) {
        const systemUser = await this.systemUserService.getSystemUser();
        const auditDTOs = [];
        for (const dto of dtos) {
            const { workflowType, username, sysUserID, personID, password, userRole } = dto;
            auditDTOs.push({
                SysUserId: systemUser.sysUserID,
                SysRoleId: RoleEnum.MON_ADMIN,
                Username: CONSTANTS.SYS_USER_USERNAME,
                PersonId: systemUser.personID,
                DateUtc: DateToUTCTransformService.transform(new Date()),
                Action: workflowType === WorkflowType.USER_CREATE ? AuditActionEnum.INSERT : AuditActionEnum.UPDATE,
                InstId: null,
                AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
                LoginSessionId: null,
                RemoteIpAddress: `::ffff:0.0.0.0`,
                UserAgent: `NULL`,
                ObjectName: 'SysUser',
                ObjectId: sysUserID,
                Data: {
                    SysUserID: sysUserID,
                    Username: username,
                    Password: userRole === UserRoleType.PARENT ? password : null,
                    IsAzureUser: IsAzureUser.YES,
                    PersonID: personID,
                    InitialPassword: userRole === UserRoleType.PARENT ? null : password,
                },
            });
        }
        //changing this from array insertion to single insertion because if we insert them as a batch typeorm throws error
        for (const auditDTO of auditDTOs) {
            const result = await this.auditService.insertAudits(auditDTO);

            this.siemLoggerService.send(
                this.siemLoggerService.buildSIEMLogObject({
                    messageLevel: MessageLevel.WARN,
                    event: SIEMLogEventType.PRIVILEGE_PERMISSIONS_CHANGED,
                    AuditId: +result.identifiers?.[0],
                    ...auditDTO,
                }),
            );
        }
    }

    async deleteSysUserByPersonID(personID: number, manager?: EntityManager) {
        return this.userRepository.deleteSysUserByPersonID(personID, manager);
    }

    async deleteAzureIDByPersonID(personID: number, manager?: EntityManager) {
        return this.userRepository.deleteAzureIDByPersonID(personID, manager);
    }

    async userNameExists(dto: SysUserCreateDTO) {
        const { username } = dto;
        const result = await this.userRepository.getSysUserByUsername(username);
        if (result) {
            return true;
        }
        return false;
    }

    async createSysUser(dto: SysUserCreateDTO, entityManager?: EntityManager) {
        const result = await this.userRepository.createSysUser(dto, entityManager);
        return result;
    }

    async createSysUserSysRole(dto: SysUserSysRoleCreateDTO, entityManager?: EntityManager) {
        const result = await this.userRepository.createSysUserSysRole(dto, entityManager);
        return result;
    }

    async findUsersByUserRoleType(query: UserFindManyRequestDto) {
        const { email, personID, userRoleType } = query;

        switch (userRoleType) {
            case UserRoleType.PARENT:
                return this.userRepository.findParents(personID, email);
            case UserRoleType.STUDENT:
                return this.userRepository.findStudents(personID, email);
            default:
                throw new Error(`Invalid userRoleType supplied.`);
        }
    }
}
