import { Inject, Injectable, forwardRef } from '@nestjs/common';
import { AuditActionEnum } from 'src/common/constants/enum/audit-log-action.enum';
import { AuditModuleEnum } from 'src/common/constants/enum/audit-module.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { MessageLevel } from 'src/common/constants/enum/siem/logger-level.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { Paging } from 'src/common/dto/paging.dto';
import { RoleAssignmentRequestDTO } from 'src/common/dto/requests/role-assignment-create-request.dts';
import { TeacherUpdateRoleRequestDTO } from 'src/common/dto/requests/teacher-update-role-request.dto';
import { AuditEntity } from 'src/common/entities/audit.entity';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { DateToUTCTransformService } from 'src/common/services/to-utc-date-transform/status-transform.service';
import { AzureTeacherService } from 'src/models/azure/azure-teacher/routing/azure-teacher.service';
import { InstitutionService } from 'src/models/institution/routing/institution.service';
import { AuditService } from 'src/models/logs/audit/routing/audit.service';
import { SIEMLogEventType } from 'src/models/siem-logger/siem-log-event-type.enum';
import { SIEMLoggerService } from 'src/models/siem-logger/siem-logger.service';
import { UserService } from 'src/models/user/routing/user.service';
import { RoleManagementRepository } from '../role-management.repository';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['getRoleAssignmentsByUserID', 'manageRoleAssignment'],
})
export class RoleManagementService {
    constructor(
        private readonly roleManagementRepository: RoleManagementRepository,
        private readonly institutionService: InstitutionService,
        private readonly auditService: AuditService,
        private readonly userService: UserService,
        private readonly siemLoggerService: SIEMLoggerService,
        @Inject(forwardRef(() => AzureTeacherService))
        private readonly azureTeacherService: AzureTeacherService,
    ) {}

    async getRoleAssignmentsByUserID(paging: Paging, roleAssignmentCreateRequestDTO: RoleAssignmentRequestDTO[]) {
        const result = [];
        for (const roleAssignment of roleAssignmentCreateRequestDTO) {
            result.push(await this.roleManagementRepository.getRoleAssignmentsByUserID(roleAssignment));
        }
        return { data: result };
    }

    async manageRoleAssignment(
        request: AuthedRequest,
        paging: Paging,
        roleAssignmentRequestDTO: RoleAssignmentRequestDTO,
    ) {
        if (!this.isUserAllowedToManageSchool(request, roleAssignmentRequestDTO)) {
            // we should think about the response { data: [] }; if it should be replaced by an exception in the future.
            return { data: [] };
        }
        const result = await this.manageRole(request, roleAssignmentRequestDTO);
        await this.logRoleManagement(request, roleAssignmentRequestDTO);

        return { data: result };
    }

    async assignRole(roleAssignmentRequestDTO: RoleAssignmentRequestDTO) {
        const result = await this.roleManagementRepository.insertRole(roleAssignmentRequestDTO);
        if (!result?.sysUserID) throw new EntityNotCreatedException();
        return result;
    }

    async removeRole(roleAssignmentRequestDTO: RoleAssignmentRequestDTO) {
        const result = await this.roleManagementRepository.deleteRole(roleAssignmentRequestDTO);
        if (!result?.sysUserID) throw new EntityNotCreatedException();
        return result;
    }

    async isUserAllowedToManageSchool(request, roleAssignmentRequestDTO: RoleAssignmentRequestDTO) {
        let initiatorSchools = [];
        const { institutionID } = roleAssignmentRequestDTO;
        if (request._authObject.isMon) {
            return true;
        }
        initiatorSchools = await this.institutionService.getInstitutionByUserName(
            request._authObject.selectedRole.Username,
        );
        // checks if the changed school is in the caller user's schools
        //if its mon then its there because we put it in the if above.
        if (initiatorSchools.filter((school) => school.institutionID === institutionID).length === 0) {
            return false;
        }
        return true;
    }

    async manageRole(request: AuthedRequest, roleAssignmentRequestDTO: RoleAssignmentRequestDTO) {
        let result = {};
        const { isDeleted, sysRoleID, sysUserID } = roleAssignmentRequestDTO;
        const teacherDtoRequest: TeacherUpdateRoleRequestDTO = {
            sysUserID: sysUserID,
        };
        if (isDeleted && sysRoleID === RoleEnum.ACCOUNTANT) {
            result = await this.removeRole(roleAssignmentRequestDTO);
            await this.azureTeacherService.adjustAccountantRole(teacherDtoRequest);
        } else if (sysRoleID === RoleEnum.ACCOUNTANT) {
            result = await this.assignRole(roleAssignmentRequestDTO);
            await this.azureTeacherService.adjustAccountantRole(teacherDtoRequest);
        } else if (isDeleted) {
            result = await this.removeRole(roleAssignmentRequestDTO);
        } else {
            result = await this.assignRole(roleAssignmentRequestDTO);
        }
        return result;
    }

    async logRoleManagement(request: AuthedRequest, roleAssignmentRequestDTO: RoleAssignmentRequestDTO) {
        const selectedRole = request._authObject.selectedRole;
        const { SysUserID, Username, PersonID, InstitutionID } = selectedRole;
        const { isDeleted, sysRoleID, sysUserID, institutionID } = roleAssignmentRequestDTO;

        const auditEntity: Partial<AuditEntity> = {
            SysUserId: SysUserID,
            SysRoleId: sysRoleID,
            Username: Username,
            PersonId: PersonID,
            DateUtc: DateToUTCTransformService.transform(new Date()),
            Action: isDeleted ? AuditActionEnum.DELETE : AuditActionEnum.INSERT,
            InstId: InstitutionID,
            AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
            LoginSessionId: request._authObject.sessionID,
            RemoteIpAddress: request.ip,
            UserAgent: request.get('User-Agent'),
            ObjectName: 'SysUserSysRole',
            ObjectId: sysUserID,
            Data: {
                AssignedToSysUserID: sysUserID,
                AssignedToSysUsername: (await this.userService.getSysUserBySysUserID(sysUserID)).username,
                AssignedSysRoleID: sysRoleID,
                AssignedSysRoleName: (await this.roleManagementRepository.getSysRoleBySysRoleID(sysRoleID)).sysRoleName,
                AssignedInstitutionID: institutionID,
            },
        };

        const result = await this.auditService.insertAudits([auditEntity]);

        this.siemLoggerService.send(
            this.siemLoggerService.buildSIEMLogObject({
                request,
                messageLevel: MessageLevel.WARN,
                event: SIEMLogEventType.PRIVILEGE_PERMISSIONS_CHANGED,
                auditEntity: { AuditId: +result.identifiers?.[0], ...auditEntity },
            }),
        );
    }
}
