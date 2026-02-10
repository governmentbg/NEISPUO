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
import { RIInstitutionService } from '../ri-institution.service';
import { RIInstitutionDTO } from '../ri-institution.dto';

@Injectable()
export class RIInstitutionDetachService extends TypeOrmCrudService<RIInstitution> {
    constructor(
        public service: RIInstitutionService,
        public connection: Connection,
        public riProcedureService: RIProcedureService,
        public auditService: AuditService,
        @InjectRepository(RIInstitution) public repo: Repository<RIInstitution>,
    ) {
        super(repo);
    }

    public async detachInstitutions(@Req() authedRequest: AuthedRequest, @Body() body: any) {
        return await this.connection.transaction(async (transactionManager) => {
            /**
             * преобразуване чрез отделяне - преобразувано (действащо)
             */
            const divideTransformType = await transactionManager
                .getRepository(TransformType)
                .findOne(TransformTypeEnum.DETACH);
            /**
             * Промяна/ преобразуване на институция
             */
            const deleteProcedureType = await transactionManager
                .getRepository(ProcedureType)
                .findOne(ProcedureTypeEnum.UPDATE);
            /**
             * Статус на процедура - приключена / потвърдена
             */
            const statusType = await transactionManager
                .getRepository(StatusType)
                .findOne(StatusTypeEnum.COMPLETED_AND_CONFIRMED);

            const riProcedure = await this.service.createProcedure(
                authedRequest,
                body,
                transactionManager,
                divideTransformType,
                deleteProcedureType,
                statusType,
            );

            const riInstitutionDto: RIInstitutionDTO = {
                ...body.miToUpdate,
                // important to be NULL to prevent update of resource if ID exist in request
                RIInstitutionID: null,
                InstitutionID: riProcedure.InstitutionID,
                RIProcedure: riProcedure,
            };
            const riInstitution = await transactionManager
                .getRepository(RIInstitution)
                .save(riInstitutionDto);

            for (const flexFieldValue of body.miToUpdate.RIFlexFieldValues) {
                const riFlexFieldValueDto: RIFlexFieldValueDto = {
                    ...flexFieldValue,
                    // important to be NULL to prevent update of resource if ID exist in request
                    RIFlexFieldValueID: null,
                    RIInstitution: riInstitution,
                };
                await transactionManager.getRepository(RIFlexFieldValue).save(riFlexFieldValueDto);
            }

            const responseCreated = [];
            for (let i = 0; i < body.misToCreate.length; i += 1) {
                let idOfPerviousInstitution;
                const procedureToCreateDto: RIProcedureDto = body.misToCreate[i].RIProcedure;

                delete body.misToCreate[i]?.ValidFrom;
                delete body.misToCreate[i]?.ValidTo;

                idOfPerviousInstitution = procedureToCreateDto.InstitutionID;

                if (i > 0) {
                    const procedureDtoToCreatePrevious: RIProcedureDto = body.misToCreate[i - 1].RIProcedure;
                    idOfPerviousInstitution = procedureDtoToCreatePrevious.InstitutionID;
                }

                procedureToCreateDto.InstitutionID = await this.service.generateInstitutionID(
                    body.misToCreate[i].Town,
                    idOfPerviousInstitution,
                );

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
                    body.miToUpdate.RIProcedure,
                    transactionManager,
                );

                let riProcedureToCreate = await this.riProcedureService.createProcedure(
                    {
                        ...procedureToCreateDto,
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

                const CPLRArea = body.misToCreate[i].CPLRAreaType?.CPLRAreaTypeID;
                if (CPLRArea) {
                    const RICPLRDto = { ...CPLRArea, CPLRAreaType: CPLRArea };
                    riProcedureToCreate = await this.riProcedureService.createProcedure(
                        {
                            ...riProcedureToCreate,
                            RICPLRArea: RICPLRDto,
                        },
                        transactionManager,
                    );
                }

                const riInstitutionToCreateDto: RIInstitutionDTO = {
                    ...body.misToCreate[i],
                    // important to be NULL to prevent update of resource if ID exist in request
                    RIInstitutionID: null,
                    InstitutionID: riProcedureToCreate.InstitutionID,
                    RIProcedure: riProcedureToCreate,
                };
                const riInstitutionToCreate = await transactionManager
                    .getRepository(RIInstitution)
                    .save(riInstitutionToCreateDto);

                for (const flexFieldValue of body.misToCreate[i].RIFlexFieldValues) {
                    const riFlexFieldValueDto: RIFlexFieldValueDto = {
                        ...flexFieldValue,
                        // important to be NULL to prevent update of resource if ID exist in request
                        RIFlexFieldValueID: null,
                        RIInstitution: riInstitutionToCreate,
                    };
                    await transactionManager
                        .getRepository(RIFlexFieldValue)
                        .save(riFlexFieldValueDto);
                }

                responseCreated.push(riInstitutionToCreate);
                await this.auditService.insertAudit(
                    {
                        authedRequest,
                        action: AuditAction.INSERT,
                        objectName: AuditObjectName.RI_INSTITUTION,
                        objectID: riInstitutionToCreate.RIInstitutionID,
                        oldValue: null,
                        newValue: riInstitutionToCreate,
                    },
                    transactionManager,
                );

                await this.service.azureSyncCreateSchool(
                    riInstitutionToCreate,
                    authedRequest,
                    transactionManager,
                );
            }

            return responseCreated;
        });
    }
}
