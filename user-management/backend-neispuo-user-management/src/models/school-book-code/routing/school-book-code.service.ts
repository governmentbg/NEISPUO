import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { AuditActionEnum } from 'src/common/constants/enum/audit-log-action.enum';
import { AuditModuleEnum } from 'src/common/constants/enum/audit-module.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { SelectedRole } from 'src/common/dto/jwt.interface';
import { SchoolBookCodeAssignRequestDTO } from 'src/common/dto/requests/school-books-code-assign-request.dto';
import { SchoolBookCodeAssignResponseDTO } from 'src/common/dto/requests/school-books-code-assign-response.dto';
import { AuditEntity } from 'src/common/entities/audit.entity';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { StudentCodeGeneratorService } from 'src/common/services/student-code-generator/student-code-generator.service';
import { DateToUTCTransformService } from 'src/common/services/to-utc-date-transform/status-transform.service';
import { PersonService } from 'src/models/person/routing/person.service';
import { Connection, EntityManager } from 'typeorm';
import { SchoolBookCodeRepository } from '../school-book-code.repository';
import { AuditService } from 'src/models/logs/audit/routing/audit.service';

@Injectable()
export class SchoolBookCodeService {
    constructor(
        private schoolBookCodeRepository: SchoolBookCodeRepository,
        private personService: PersonService,
        private auditService: AuditService,
        private connection: Connection,
    ) {}

    async assignSchoolBookCodes(
        schoolBookCodeReAssignRequestDTO: SchoolBookCodeAssignRequestDTO,
        request?: AuthedRequest,
    ) {
        const { personIDs } = schoolBookCodeReAssignRequestDTO;
        const result = [];
        await this.connection.transaction(async (manager) => {
            for (const personId of personIDs) {
                const dto = await this.constructSchoolBookCodeDTO(personId);
                const savedCode = await this.assignSchoolBookCode(dto, manager);
                await this.deleteChildCodeByPersonID(dto, manager, request);
                result.push(savedCode);
            }
        });
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_RE_ASSIGN_CODE]: result } };
    }

    async assignSchoolBookCode(dto: SchoolBookCodeAssignResponseDTO, entityManager?: EntityManager) {
        const result = await this.schoolBookCodeRepository.assignSchoolBookCode(dto, entityManager);
        if (!result?.code || !result.personID) throw new EntityNotCreatedException();
        return result;
    }

    async deleteChildCodeByPersonID(
        dto: SchoolBookCodeAssignResponseDTO,
        entityManager: EntityManager,
        request?: AuthedRequest,
    ) {
        const selectedRole: SelectedRole = request?._authObject?.selectedRole || null;
        const { SysUserID, SysRoleID, Username, PersonID, InstitutionID } = selectedRole || {};

        const results = await this.schoolBookCodeRepository.deleteChildCodeByPersonID(dto, entityManager);
        for (const result of results) {
            const auditEntity: Partial<AuditEntity> = {
                SysUserId: SysUserID || null,
                SysRoleId: SysRoleID || null,
                Username: Username || null,
                PersonId: PersonID || null,
                DateUtc: DateToUTCTransformService.transform(new Date()),
                Action: AuditActionEnum.DELETE,
                InstId: InstitutionID || null,
                AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
                LoginSessionId: request?._authObject?.sessionID || null,
                RemoteIpAddress: request?.ip || `::ffff:0.0.0.0`,
                UserAgent: request?.get('User-Agent') || `NULL`,
                ObjectName: CONSTANTS.LOG_OBJECT_NAME_PARENT_CHILD_SCHOOL_BOOK_ACCESS,
                ObjectId: result.parentChildSchoolBookAccessID,
                Data: {
                    childID: result.childID,
                    parentID: result.parentID,
                },
            };

            await this.auditService.insertAudit(auditEntity);
        }
        return results;
    }

    async constructSchoolBookCodeDTO(personId: number): Promise<SchoolBookCodeAssignResponseDTO> {
        const person = await this.personService.getPersonByPersonID(personId);
        if (!person?.personID) throw new DataNotFoundException();
        const { personID } = person;
        const code = StudentCodeGeneratorService.getCode();
        return {
            code,
            personID,
        };
    }
}
