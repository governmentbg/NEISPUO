import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { AuditActionEnum } from 'src/common/constants/enum/audit-log-action.enum';
import { AuditModuleEnum } from 'src/common/constants/enum/audit-module.enum';
import { ParentHasAccessEnum } from 'src/common/constants/enum/parent-has-access.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { SelectedRole } from 'src/common/dto/jwt.interface';
import { AccessUpsertRequestDTO } from 'src/common/dto/requests/access-upsert-request.dto';
import { ChildAccessRequestDTO } from 'src/common/dto/requests/child-access-request.dto';
import { ChildRevokeAccessRequestDTO } from 'src/common/dto/requests/child-revoke-access-request.dto';
import { SchoolBookCodeAssignRequestDTO } from 'src/common/dto/requests/school-books-code-assign-request.dto';
import { AuditEntity } from 'src/common/entities/audit.entity';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { DateToUTCTransformService } from 'src/common/services/to-utc-date-transform/status-transform.service';
import { AuditService } from 'src/models/logs/audit/routing/audit.service';
import { PersonService } from 'src/models/person/routing/person.service';
import { SchoolBookCodeService } from 'src/models/school-book-code/routing/school-book-code.service';
import { EntityManager } from 'typeorm';
import { ParentChildSchoolBookAccessesFindManyRequestDto } from '../../../common/dto/requests/pcsba-find-many-request.dto';
import { ParentAccessRepository } from '../parent-access.repository';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['changeAccess', 'enrollChildAndGiveAccess'],
})
export class ParentAccessService {
    constructor(
        private parentAccessRepository: ParentAccessRepository,
        private schoolBookCodesService: SchoolBookCodeService,
        private auditService: AuditService,
        private personService: PersonService,
    ) {}

    async enrollChildAndGiveAccess(childAccessRequestDTO: ChildAccessRequestDTO, request: AuthedRequest) {
        const { parentID, childPersonalID, childSchoolBookCode } = childAccessRequestDTO;
        const selectedRole: SelectedRole = request._authObject.selectedRole;
        const { SysUserID, SysRoleID, Username, PersonID, InstitutionID } = selectedRole;

        const parent = await this.parentAccessRepository.getPersonByParentID(parentID);
        if (!parent?.parentID) throw new DataNotFoundException();
        const child = await this.personService.getPersonByPersonalIDAndSchoolCode(childPersonalID, childSchoolBookCode);
        if (!child?.personID) throw new DataNotFoundException();
        const result = await this.parentAccessRepository.upsertParentAccess(
            parent.parentID,
            child.personID,
            ParentHasAccessEnum.YES,
        );
        const auditEntity: Partial<AuditEntity> = {
            SysUserId: SysUserID,
            SysRoleId: SysRoleID,
            Username: Username,
            PersonId: PersonID,
            DateUtc: DateToUTCTransformService.transform(new Date()),
            Action: AuditActionEnum.UPDATE,
            InstId: InstitutionID,
            AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
            LoginSessionId: request._authObject.sessionID,
            RemoteIpAddress: request.ip,
            UserAgent: request.get('User-Agent'),
            ObjectName: CONSTANTS.LOG_OBJECT_NAME_PARENT_CHILD_SCHOOL_BOOK_ACCESS,
            ObjectId: result.parentChildSchoolBookAccessID,
            Data: {
                parentID,
                childPersonalID,
                hasAccess: ParentHasAccessEnum.YES,
            },
        };

        await this.auditService.insertAudit(auditEntity);

        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_PARENT_CHILD_ACCESS]: result } };
    }

    async enrollChildAndTakeAccess(childAccessRequestDTO: ChildAccessRequestDTO, request: AuthedRequest) {
        const { parentID, childPersonalID, childSchoolBookCode } = childAccessRequestDTO;
        const selectedRole: SelectedRole = request._authObject.selectedRole;
        const { SysUserID, SysRoleID, Username, PersonID, InstitutionID } = selectedRole;

        const parent = await this.parentAccessRepository.getPersonByParentID(parentID);
        if (!parent?.parentID) throw new DataNotFoundException();
        const child = await this.personService.getPersonByPersonalIDAndSchoolCode(childPersonalID, childSchoolBookCode);
        if (!child?.personID) throw new DataNotFoundException();
        const result = await this.parentAccessRepository.upsertParentAccess(
            parent.parentID,
            child.personID,
            ParentHasAccessEnum.NO,
        );

        const auditEntity: Partial<AuditEntity> = {
            SysUserId: SysUserID,
            SysRoleId: SysRoleID,
            Username: Username,
            PersonId: PersonID,
            DateUtc: DateToUTCTransformService.transform(new Date()),
            Action: AuditActionEnum.UPDATE,
            InstId: InstitutionID,
            AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
            LoginSessionId: request._authObject.sessionID,
            RemoteIpAddress: request.ip,
            UserAgent: request.get('User-Agent'),
            ObjectName: CONSTANTS.LOG_OBJECT_NAME_PARENT_CHILD_SCHOOL_BOOK_ACCESS,
            ObjectId: result.parentChildSchoolBookAccessID,
            Data: {
                parentID,
                childPersonalID,
                hasAccess: ParentHasAccessEnum.NO,
            },
        };

        await this.auditService.insertAudit(auditEntity);

        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_PARENT_CHILD_ACCESS]: result } };
    }

    async revokeAccessToChild(childRevokeAccessRequestDTO: ChildRevokeAccessRequestDTO, request: AuthedRequest) {
        const { personID } = childRevokeAccessRequestDTO;
        const result = [];
        const selectedRole: SelectedRole = request._authObject.selectedRole;
        const { SysUserID, SysRoleID, Username, PersonID, InstitutionID } = selectedRole;

        const child = await this.personService.getPersonByPersonID(personID);
        if (!child?.personID) throw new DataNotFoundException();
        const childID = child.personID;
        const parents = await this.parentAccessRepository.getParentIDsByChildID(childID);
        for (const parent of parents) {
            const { parentID } = parent;
            const removeAccessResult = await this.parentAccessRepository.upsertParentAccess(
                parentID,
                childID,
                ParentHasAccessEnum.NO,
            );
            if (!removeAccessResult?.hasAccess) {
                const auditEntity: Partial<AuditEntity> = {
                    SysUserId: SysUserID,
                    SysRoleId: SysRoleID,
                    Username: Username,
                    PersonId: PersonID,
                    DateUtc: DateToUTCTransformService.transform(new Date()),
                    Action: AuditActionEnum.UPDATE,
                    InstId: InstitutionID,
                    AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
                    LoginSessionId: request._authObject.sessionID,
                    RemoteIpAddress: request.ip,
                    UserAgent: request.get('User-Agent'),
                    ObjectName: CONSTANTS.LOG_OBJECT_NAME_PARENT_CHILD_SCHOOL_BOOK_ACCESS,
                    ObjectId: removeAccessResult.parentChildSchoolBookAccessID,
                    Data: {
                        parentID,
                        childID,
                        hasAccess: ParentHasAccessEnum.NO,
                    },
                };

                await this.auditService.insertAudit(auditEntity);

                result.push(parentID);
            }
        }
        const schoolBookRequestDTO: SchoolBookCodeAssignRequestDTO = {
            personIDs: [child.personID],
        };
        await this.schoolBookCodesService.assignSchoolBookCodes(schoolBookRequestDTO, request);
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_PARENT_CHILD_ACCESS]: result } };
    }

    async createParentAccessToChild(
        parentID: number,
        childID: number,
        entityManager: EntityManager,
        request: AuthedRequest,
    ) {
        const selectedRole: SelectedRole = request._authObject.selectedRole;
        const { SysUserID, SysRoleID, Username, PersonID, InstitutionID } = selectedRole;

        const result = await this.parentAccessRepository.createParentAccess(parentID, childID, entityManager);

        const auditEntity: Partial<AuditEntity> = {
            SysUserId: SysUserID,
            SysRoleId: SysRoleID,
            Username: Username,
            PersonId: PersonID,
            DateUtc: DateToUTCTransformService.transform(new Date()),
            Action: AuditActionEnum.INSERT,
            InstId: InstitutionID,
            AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
            LoginSessionId: request._authObject.sessionID,
            RemoteIpAddress: request.ip,
            UserAgent: request.get('User-Agent'),
            ObjectName: CONSTANTS.LOG_OBJECT_NAME_PARENT_CHILD_SCHOOL_BOOK_ACCESS,
            ObjectId: result.parentChildSchoolBookAccessID,
            Data: {
                parentID,
                childID,
                hasAccess: ParentHasAccessEnum.YES,
            },
        };

        await this.auditService.insertAudit(auditEntity);
    }

    async getParentChildSchoolBookAccesses(query: ParentChildSchoolBookAccessesFindManyRequestDto) {
        return this.parentAccessRepository.getParentChildSchoolBookAccesses(query);
    }

    async accessUpsert(accessUpsertRequestDTO: AccessUpsertRequestDTO, request: AuthedRequest) {
        const { parentID, childID, hasAccess } = accessUpsertRequestDTO;
        const selectedRole: SelectedRole = request._authObject.selectedRole;
        const { SysUserID, SysRoleID, Username, PersonID, InstitutionID } = selectedRole;

        const hasParentChildSchoolBookAccessRecord =
            await this.parentAccessRepository.hasParentChildSchoolBookAccessRecord(parentID, childID);

        const result = await this.parentAccessRepository.upsertParentAccess(parentID, childID, hasAccess);

        const auditEntity: Partial<AuditEntity> = {
            SysUserId: SysUserID,
            SysRoleId: SysRoleID,
            Username: Username,
            PersonId: PersonID,
            DateUtc: DateToUTCTransformService.transform(new Date()),
            Action: hasParentChildSchoolBookAccessRecord ? AuditActionEnum.UPDATE : AuditActionEnum.INSERT,
            InstId: InstitutionID,
            AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
            LoginSessionId: request._authObject.sessionID,
            RemoteIpAddress: request.ip,
            UserAgent: request.get('User-Agent'),
            ObjectName: CONSTANTS.LOG_OBJECT_NAME_PARENT_CHILD_SCHOOL_BOOK_ACCESS,
            ObjectId: result.parentChildSchoolBookAccessID,
            Data: {
                parentID,
                childID,
                hasAccess,
            },
        };

        await this.auditService.insertAudit(auditEntity);

        return result;
    }

    async deleteParentChildSchoolBookAccess(parentChildSchoolBookAccessID: number, request: AuthedRequest) {
        const selectedRole: SelectedRole = request._authObject.selectedRole;
        const { SysUserID, SysRoleID, Username, PersonID, InstitutionID } = selectedRole;

        const result = await this.parentAccessRepository.deleteParentChildSchoolBookAccess(
            parentChildSchoolBookAccessID,
        );

        const auditEntity: Partial<AuditEntity> = {
            SysUserId: SysUserID,
            SysRoleId: SysRoleID,
            Username: Username,
            PersonId: PersonID,
            DateUtc: DateToUTCTransformService.transform(new Date()),
            Action: AuditActionEnum.DELETE,
            InstId: InstitutionID,
            AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
            LoginSessionId: request._authObject.sessionID,
            RemoteIpAddress: request.ip,
            UserAgent: request.get('User-Agent'),
            ObjectName: CONSTANTS.LOG_OBJECT_NAME_PARENT_CHILD_SCHOOL_BOOK_ACCESS,
            ObjectId: parentChildSchoolBookAccessID,
            Data: {
                parentChildSchoolBookAccessID,
            },
        };

        await this.auditService.insertAudit(auditEntity);

        return result;
    }
}
