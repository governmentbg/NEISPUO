import { ProcedureType } from '@domain/procedure-type/procedure-type.entity';
import { RIFlexFieldValue } from '@domain/ri-flex-field-value/ri-flex-field-value.entity';
import { RIFlexFieldValueDto } from '@domain/ri-flex-field-value/routes/ri-flex-field-value/ri-flex-field-value.dto';
import { RIProcedure } from '@domain/ri-procedure/ri-procedure.entity';
import { RIProcedureDto } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.dto';
import { RIProcedureService } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.service';
import { StatusType } from '@domain/status-type/status-type.entity';
import { TransformType } from '@domain/transform-type/transform-type.entity';
import { Body, Injectable, Req } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { Connection, Repository } from 'typeorm';
import { RIPremInstitutionDTO } from '@domain/ri-prem-institution/routes/ri-prem-institution/ri-prem-institution.dto';
import { RIPremInstitution } from '@domain/ri-prem-institution/ri-prem-institution.entity';
import { TransformTypeEnum } from '@domain/transform-type/enums/transform-type.enum';
import { ProcedureTypeEnum } from '@domain/procedure-type/enums/procedure-type.enum';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { AuditAction } from '@shared/enums/audit-action';
import { AuditObjectName } from '@shared/enums/audit-object-name';
import { AuditService } from '@domain/ri-institution/audit/audit.service';
import { StatusTypeEnum } from '@domain/status-type/enums/status-type.enum';
import { RIInstitutionDTO } from '../ri-institution.dto';
import { RIInstitutionService } from '../ri-institution.service';
import { RIInstitution } from '../../../ri-institution.entity';

@Injectable()
export class RIInstitutionDivideService extends TypeOrmCrudService<RIInstitution> {
    constructor(
        public service: RIInstitutionService,
        public connection: Connection,
        public riProcedureService: RIProcedureService,
        public auditService: AuditService,
        @InjectRepository(RIInstitution) public repo: Repository<RIInstitution>,
    ) {
        super(repo);
    }

    public async divideInstitutions(@Req() authedRequest: AuthedRequest, @Body() body: any) {
        return await this.connection.transaction(async (transactionManager) => {
            /**
             * преобразуване чрез разделяне - преобразувано (закрито)
             */
            const divideTransformType = await transactionManager
                .getRepository(TransformType)
                .findOne(TransformTypeEnum.DIVIDE);
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

            if (body.miToDelete.RIProcedure?.RICPLRArea) {
                delete body.miToDelete.RIProcedure.RICPLRArea.RICPLRAreaID;
            }
            if (body.miToDelete.RIProcedure?.RIInstitutionDepartments) {
                for (const institutionDepartment of body.miToDelete.RIProcedure
                    .RIInstitutionDepartments) {
                    delete institutionDepartment.RIInstitutionDepartmentID;
                }
            }

            const procedureDto: RIProcedureDto = {
                ...body.miToDelete.RIProcedure,
                // important to be NULL to prevent update of resource if ID exist in request
                RIProcedureID: null,
                InstitutionID: body.miToDelete.InstitutionID,
            };
            delete procedureDto?.ValidFrom;
            delete procedureDto?.ValidTo;

            // TODO: move logic in service
            const oldProcedures = await transactionManager
                .getRepository(RIProcedure)
                .find({ InstitutionID: body.miToDelete.InstitutionID });
            for (const oldProcedure of oldProcedures) {
                await transactionManager.getRepository(RIProcedure).save({
                    ...oldProcedure,
                    IsActive: false,
                });
            }

            await this.auditService.insertAudit(
                {
                    authedRequest,
                    action: AuditAction.DELETE,
                    objectName: AuditObjectName.RI_PROCEDURE,
                    objectID: body.miToDelete.riProcedureId,
                    oldValue: body.miToDelete,
                    newValue: null,
                },
                transactionManager,
            );
            const order = await this.riProcedureService.generateOrderField(
                procedureDto,
                transactionManager,
            );

            const riProcedure = await this.riProcedureService.createProcedure(
                {
                    ...procedureDto,
                    TransformType: divideTransformType,
                    ProcedureType: deleteProcedureType,
                    StatusType: statusType,
                    // last procedure should be IsActive = 1
                    IsActive: true,
                    Ord: order,
                },
                transactionManager,
            );

            const riInstitutionDto: RIInstitutionDTO = {
                ...body.miToDelete,
                // important to be NULL to prevent update of resource if ID exist in request
                RIInstitutionID: null,
                InstitutionID: riProcedure.InstitutionID,
                RIProcedure: riProcedure,
            };
            const riInstitution = await transactionManager
                .getRepository(RIInstitution)
                .save(riInstitutionDto);

            for (const flexFieldValue of body.miToDelete.RIFlexFieldValues) {
                const riFlexFieldValueDto: RIFlexFieldValueDto = {
                    ...flexFieldValue,
                    // important to be NULL to prevent update of resource if ID exist in request
                    RIFlexFieldValueID: null,
                    RIInstitution: riInstitution,
                };
                await transactionManager.getRepository(RIFlexFieldValue).save(riFlexFieldValueDto);
            }

            const riPremInstitutionDto: RIPremInstitutionDTO = {
                ...riProcedure.RIPremInstitution,
            };
            await transactionManager.getRepository(RIPremInstitution).save({
                ...riPremInstitutionDto,
                // important to be NULL to prevent update of resource if ID exist in request
                RIPremInstitutionID: null,
                RIProcedure: riProcedure,
            });

            const responseData = [];

            for (let i = 0; i < body.misToCreate.length; i += 1) {
                let idOfPerviousInstitution;
                const procedureDtoToCreate: RIProcedureDto = body.misToCreate[i].RIProcedure;
                idOfPerviousInstitution = procedureDtoToCreate.InstitutionID;

                delete body.misToCreate[i]?.ValidFrom;
                delete body.misToCreate[i]?.ValidTo;

                if (i > 0) {
                    const procedureDtoToCreatePrevious: RIProcedureDto = body.misToCreate[i - 1].RIProcedure;
                    idOfPerviousInstitution = procedureDtoToCreatePrevious.InstitutionID;
                }

                procedureDtoToCreate.InstitutionID = await this.service.generateInstitutionID(
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
                    procedureDtoToCreate,
                    transactionManager,
                );

                let riProcedureToCreate = await this.riProcedureService.createProcedure(
                    {
                        ...procedureDtoToCreate,
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
                responseData.push(riInstitutionToCreate);

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
            await this.service.azureSyncDeleteSchool(
                body.miToDelete,
                authedRequest,
                transactionManager,
            );

            return responseData;
        });
    }
}
