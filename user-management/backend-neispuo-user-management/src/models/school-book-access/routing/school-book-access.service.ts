import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { AuditActionEnum } from 'src/common/constants/enum/audit-log-action.enum';
import { AuditModuleEnum } from 'src/common/constants/enum/audit-module.enum';
import { MessageLevel } from 'src/common/constants/enum/siem/logger-level.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { PersonnelSchoolBookAccessRequestDTO } from 'src/common/dto/requests/personnel-school-book-access.request.dto';
import { UpdatePersonnelSchoolBookAccessRequestDTO } from 'src/common/dto/requests/update-personnel-school-book-access-request.dto';
import { AuditEntity } from 'src/common/entities/audit.entity';
import { PersonnelSchoolBookAccess } from 'src/common/entities/personnel-school-book-access.entity';
import { DateToUTCTransformService } from 'src/common/services/to-utc-date-transform/status-transform.service';
import { UserActionLogType } from 'src/common/types/siem/user-action-log.type';
import { SIEMLogEventType } from 'src/models/siem-logger/siem-log-event-type.enum';
import { SIEMLoggerService } from 'src/models/siem-logger/siem-logger.service';
import { TransactionEventHandlerService } from 'src/models/transaction-event-handler/transaction-event-handler.service';
import { UserService } from 'src/models/user/routing/user.service';
import { Connection, EntityManager, In, Repository } from 'typeorm';

@Injectable()
export class SchoolBookAccessService {
    constructor(
        @InjectRepository(PersonnelSchoolBookAccess) private readonly repo: Repository<PersonnelSchoolBookAccess>,
        private readonly userService: UserService,
        private connection: Connection,
        private readonly transactionEventHandlerService: TransactionEventHandlerService,
    ) {}

    //use QueryBuilder as current TypeOrm version in project do not allow ordering by relation field
    //it is supported in version 0.3.0 or hig
    async getSchoolBooksByPersonID(personID: number) {
        return this.connection
            .createQueryBuilder()
            .select('personnelSchoolBookAccess')
            .addSelect('classBook')
            .from('PersonnelSchoolBookAccess', 'personnelSchoolBookAccess')
            .leftJoin(
                'personnelSchoolBookAccess.classBook',
                'classBook',
                'classBook.ClassBookID = personnelSchoolBookAccess.ClassBookID',
            )
            .where('personnelSchoolBookAccess.personID= :personID', { personID: personID })
            .orderBy('personnelSchoolBookAccess.SchoolYear', 'DESC')
            .addOrderBy('classBook.fullBookName', 'ASC')
            .getMany();
    }

    async giveSchoolBookAccessToPerson(request: AuthedRequest, dtos: PersonnelSchoolBookAccessRequestDTO[]) {
        let requestedDtos = [...dtos];

        if (dtos?.length === 0) return;

        const existingSchoolBookAccesses = await this.repo.find({ where: [...requestedDtos] });

        /**
         * Skip the ones that already exist so we don't get the duplicate key error from the DB
         */
        if (existingSchoolBookAccesses.length > 0) {
            requestedDtos = dtos.filter((dto) => {
                // eslint-disable-next-line @typescript-eslint/no-unused-expressions
                !existingSchoolBookAccesses.some(
                    (esba) => esba.classBookID === dto.classBookID && esba.schoolYear === dto.schoolYear,
                );
            });
        }

        /** Check if we have anything for creation. */
        if (requestedDtos?.length === 0) return;

        return this.connection.transaction(async (transaction) => {
            await this.logSchoolBookAccessManagement(request, requestedDtos, false, transaction);
            return this.repo.save(dtos);
        });
    }

    async deleteSelectedAccesses(request: AuthedRequest, rowIDs: number[]) {
        return this.connection.transaction(async (transaction) => {
            const psbaRepo = transaction.getRepository(PersonnelSchoolBookAccess);
            const personnelSchoolBookAccessDTOs = await psbaRepo.find({
                where: { rowID: In(rowIDs) },
            });
            await this.logSchoolBookAccessManagement(request, personnelSchoolBookAccessDTOs, true, transaction);
            return psbaRepo.delete(rowIDs);
        });
    }

    async updateHasAdminAccess(request: AuthedRequest, dto: UpdatePersonnelSchoolBookAccessRequestDTO) {
        return this.connection.transaction(async (transaction) => {
            const psbaRepo = transaction.getRepository(PersonnelSchoolBookAccess);
            const personnelSchoolBookAccessDTO = await psbaRepo.findOne({
                where: { rowID: dto.rowID },
            });
            await this.logSchoolBookAccessManagement(request, [personnelSchoolBookAccessDTO], true, transaction);
            return psbaRepo.save(dto);
        });
    }

    async logSchoolBookAccessManagement(
        request,
        personnelSchoolBookAccessRequestDTOs: PersonnelSchoolBookAccessRequestDTO[],
        isDeleted: boolean,
        transaction: EntityManager,
    ) {
        const selectedRole = request._authObject.selectedRole;
        const { SysUserID, Username, PersonID, InstitutionID } = selectedRole;
        const auditRepo = transaction.getRepository(AuditEntity);

        const firstRow = personnelSchoolBookAccessRequestDTOs[0];
        const { sysUserID, username } = await this.userService.getSysUserByPersonID(firstRow.personID);
        const auditEntries = personnelSchoolBookAccessRequestDTOs.map((psba) => {
            const { schoolYear, classBookID, hasAdminAccess } = psba;
            const auditEntry = {
                SysUserId: SysUserID,
                Username: Username,
                PersonId: PersonID,
                DateUtc: DateToUTCTransformService.transform(new Date()),
                Action: isDeleted ? AuditActionEnum.DELETE : AuditActionEnum.INSERT,
                InstId: InstitutionID,
                AuditModuleId: AuditModuleEnum.USER_MANAGEMENT,
                LoginSessionId: request._authObject.sessionID,
                RemoteIpAddress: request.ip,
                UserAgent: request.get('User-Agent'),
                ObjectName: 'PersonnelSchoolBookAccess',
                ObjectId: sysUserID,
                Data: {
                    AssignedToSysUserID: sysUserID,
                    AssignedToSysUsername: username,
                    AssignedSchoolBookAccess: classBookID,
                    SchoolBookSchoolYear: schoolYear,
                    HasAdminAccess: hasAdminAccess,
                },
            };

            this.transactionEventHandlerService.registerCommitCallback(
                transaction,
                (dto: UserActionLogType, siemLoggerService: SIEMLoggerService) => {
                    siemLoggerService.send(siemLoggerService.buildSIEMLogObject(dto));
                },
                {
                    request,
                    messageLevel: MessageLevel.WARN,
                    event: SIEMLogEventType.SCHOOL_BOOK_ACCESS_CHANGE,
                    attributes: { ...psba },
                    auditEntity: auditEntry,
                },
            );

            return auditEntry;
        });

        return auditRepo.insert(auditEntries);
    }
}
