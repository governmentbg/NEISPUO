import { ProcedureTypeEnum } from '@domain/procedure-type/enums/procedure-type.enum';
import { ProcedureType } from '@domain/procedure-type/procedure-type.entity';
import { RIFlexFieldValue } from '@domain/ri-flex-field-value/ri-flex-field-value.entity';
import { RIFlexFieldValueDto } from '@domain/ri-flex-field-value/routes/ri-flex-field-value/ri-flex-field-value.dto';
import { AuditService } from '@domain/ri-institution/audit/audit.service';
import { RIProcedureDto } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.dto';
import { RIProcedureService } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.service';
import { StatusTypeEnum } from '@domain/status-type/enums/status-type.enum';
import { StatusType } from '@domain/status-type/status-type.entity';
import { TransformTypeEnum } from '@domain/transform-type/enums/transform-type.enum';
import { TransformType } from '@domain/transform-type/transform-type.entity';
import { Body, Injectable, Req } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { AuditAction } from '@shared/enums/audit-action';
import { AuditObjectName } from '@shared/enums/audit-object-name';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { Connection, Repository } from 'typeorm';
import { RIInstitution } from '../../../ri-institution.entity';
import { RIInstitutionDTO } from '../ri-institution.dto';
import { RIInstitutionService } from '../ri-institution.service';

@Injectable()
export class RIInstitutionMergeService extends TypeOrmCrudService<RIInstitution> {
    constructor(
        public service: RIInstitutionService,
        public connection: Connection,
        public riProcedureService: RIProcedureService,
        public auditService: AuditService,
        @InjectRepository(RIInstitution) public repo: Repository<RIInstitution>,
    ) {
        super(repo);
    }

    public async mergeInstitutions(@Req() authedRequest: AuthedRequest, @Body() reqBody: any) {
        /**
         * Remove Valid From / Valid To as they are causing issues
         */
        delete reqBody.miToCreate.ValidFrom;
        delete reqBody.miToCreate.ValidTo;
        for (const mitd of reqBody.misToDelete) {
            delete mitd.ValidFrom;
            delete mitd.ValidTo;
        }

        return await this.connection.transaction(async (transactionManager) => {
            /**
             * ппреобразуване чрез сливане - преобразувано (закрито)
             */
            const joinedTransformType = await transactionManager
                .getRepository(TransformType)
                .findOne(TransformTypeEnum.MERGE);
            /**
             * вид на процедура - Закриване/ отписване на институция
             */
            const deleteProcedureType = await transactionManager
                .getRepository(ProcedureType)
                .findOne(ProcedureTypeEnum.DELETE);
            /**
             * Статус на процедура - приключена / потвърдена
             */
            const statusType = await transactionManager
                .getRepository(StatusType)
                .findOne(StatusTypeEnum.COMPLETED_AND_CONFIRMED);

            await this.service.deleteProcedures(
                reqBody,
                transactionManager,
                joinedTransformType,
                deleteProcedureType,
                statusType,
                authedRequest,
            );

            const procedureDto: RIProcedureDto = reqBody.miToCreate.RIProcedure;

            procedureDto.InstitutionID = await this.service.generateInstitutionID(
                reqBody.miToCreate.Town,
            );
            delete procedureDto?.ValidFrom;
            delete procedureDto?.ValidTo;

            const createTransformType = await transactionManager
                .getRepository(TransformType)
                .findOne(TransformTypeEnum.OPENING);
            /**
             *
             */
            const createProcedureType = await transactionManager
                .getRepository(ProcedureType)
                .findOne(ProcedureTypeEnum.CREATE);
            /**
             * Статус на процедура - приключена / потвърдена
             */
            const procedureStatusType = await transactionManager
                .getRepository(StatusType)
                .findOne(StatusTypeEnum.COMPLETED_AND_CONFIRMED);

            const order = await this.riProcedureService.generateOrderField(
                procedureDto,
                transactionManager,
            );

            let riProcedure = await this.riProcedureService.createProcedure(
                {
                    ...procedureDto,
                    // important to be NULL to prevent update of resource if ID exist in request
                    RIProcedureID: null,
                    TransformType: createTransformType,
                    ProcedureType: createProcedureType,
                    StatusType: procedureStatusType,
                    // last procedure should be IsActive = 1
                    IsActive: true,
                    Ord: order,
                },
                transactionManager,
            );

            const CPLRArea = reqBody?.miToCreate.CPLRAreaType?.CPLRAreaTypeID;
            if (CPLRArea) {
                const RICPLRDto = { ...CPLRArea, CPLRAreaType: CPLRArea };
                riProcedure = await this.riProcedureService.createProcedure(
                    {
                        ...riProcedure,
                        RICPLRArea: RICPLRDto,
                    },
                    transactionManager,
                );
            }

            const riInstitutionDto: RIInstitutionDTO = {
                ...reqBody.miToCreate,
                // important to be NULL to prevent update of resource if ID exist in request
                RIInstitutionID: null,
                InstitutionID: riProcedure.InstitutionID,
                RIProcedure: riProcedure,
            };
            const riInstitution = await transactionManager
                .getRepository(RIInstitution)
                .save(riInstitutionDto);

            for (const flexFieldValue of reqBody.miToCreate.RIFlexFieldValues) {
                const riFlexFieldValueDto: RIFlexFieldValueDto = {
                    ...flexFieldValue,
                    // important to be NULL to prevent update of resource if ID exist in request
                    RIFlexFieldValueID: null,
                    RIInstitution: riInstitution,
                };
                await transactionManager.getRepository(RIFlexFieldValue).save(riFlexFieldValueDto);
            }
            await this.auditService.insertAudit(
                {
                    authedRequest,
                    action: AuditAction.INSERT,
                    objectName: AuditObjectName.RI_INSTITUTION,
                    objectID: riInstitution.RIInstitutionID,
                    oldValue: null,
                    newValue: riInstitution,
                },
                transactionManager,
            );
            await this.service.azureSyncCreateSchool(
                riInstitution,
                authedRequest,
                transactionManager,
            );

            for (const mitd of reqBody.misToDelete) {
                await this.service.azureSyncDeleteSchool(mitd, authedRequest, transactionManager);
            }

            return riInstitution;
        });
    }
}
