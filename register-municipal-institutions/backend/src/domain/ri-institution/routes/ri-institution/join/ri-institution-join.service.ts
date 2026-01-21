import { ProcedureTypeEnum } from '@domain/procedure-type/enums/procedure-type.enum';
import { ProcedureType } from '@domain/procedure-type/procedure-type.entity';
import { RIFlexFieldValue } from '@domain/ri-flex-field-value/ri-flex-field-value.entity';
import { RIFlexFieldValueDto } from '@domain/ri-flex-field-value/routes/ri-flex-field-value/ri-flex-field-value.dto';
import { AuditService } from '@domain/ri-institution/audit/audit.service';
import { RIPremInstitution } from '@domain/ri-prem-institution/ri-prem-institution.entity';
import { RIPremInstitutionDTO } from '@domain/ri-prem-institution/routes/ri-prem-institution/ri-prem-institution.dto';
import { RIProcedure } from '@domain/ri-procedure/ri-procedure.entity';
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
export class RIInstitutionJoinService extends TypeOrmCrudService<RIInstitution> {
    constructor(
        public auditService: AuditService,
        public service: RIInstitutionService,
        public connection: Connection,
        public riProcedureService: RIProcedureService,
        @InjectRepository(RIInstitution) public repo: Repository<RIInstitution>,
    ) {
        super(repo);
    }

    public async joinInstitutions(@Req() authedRequest: AuthedRequest, @Body() reqBody: any) {
        await this.connection.transaction(async (transactionManager) => {
            /**
             * преобразуване чрез вливане - преобразувано (закрито)
             */
            const joinedTransformType = await transactionManager
                .getRepository(TransformType)
                .findOne(TransformTypeEnum.JOIN);
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

            for (const body of reqBody.misToDelete) {
                // important to be NULL to prevent update of resource if ID exist in request
                if (body.RIProcedure?.RICPLRArea) {
                    delete body.RIProcedure.RICPLRArea.RICPLRAreaID;
                }
                if (body.RIProcedure?.RIInstitutionDepartments) {
                    for (const institutionDepartment of body.RIProcedure.RIInstitutionDepartments) {
                        delete institutionDepartment.RIInstitutionDepartmentID;
                    }
                }
                delete body?.ValidTo;
                delete body?.ValidFrom;

                await this.auditService.insertAudit(
                    {
                        authedRequest,
                        action: AuditAction.DELETE,
                        objectName: AuditObjectName.RI_PROCEDURE,
                        objectID: reqBody.misToDelete.InstitutionID,
                        oldValue: reqBody.misToDelete,
                        newValue: null,
                    },
                    transactionManager,
                );

                const procedureDto: RIProcedureDto = {
                    ...body.RIProcedure,
                    // important to be NULL to prevent update of resource if ID exist in request
                    RIProcedureID: null,
                    InstitutionID: body.InstitutionID,
                };
                const oldProcedures = await transactionManager
                    .getRepository(RIProcedure)
                    .find({ InstitutionID: body.InstitutionID });
                for (const oldProcedure of oldProcedures) {
                    await transactionManager.getRepository(RIProcedure).save({
                        ...oldProcedure,
                        IsActive: false,
                    });
                }

                const order = await this.riProcedureService.generateOrderField(
                    procedureDto,
                    transactionManager,
                );

                const riProcedure = await this.riProcedureService.createProcedure(
                    {
                        ...procedureDto,
                        TransformType: joinedTransformType,
                        ProcedureType: deleteProcedureType,
                        StatusType: statusType,
                        // last procedure should be IsActive = 1
                        IsActive: true,
                        Ord: order,
                    },
                    transactionManager,
                );
                const riInstitutionDto: RIInstitutionDTO = {
                    ...body,
                    // important to be NULL to prevent update of resource if ID exist in request
                    RIInstitutionID: null,
                    InstitutionID: riProcedure.InstitutionID,
                    RIProcedure: riProcedure,
                };
                const riInstitution = await transactionManager
                    .getRepository(RIInstitution)
                    .save(riInstitutionDto);

                for (const flexFieldValue of body.RIFlexFieldValues) {
                    const riFlexFieldValueDto: RIFlexFieldValueDto = {
                        ...flexFieldValue,
                        // important to be NULL to prevent update of resource if ID exist in request
                        RIFlexFieldValueID: null,
                        RIInstitution: riInstitution,
                    };
                    await transactionManager
                        .getRepository(RIFlexFieldValue)
                        .save(riFlexFieldValueDto);
                }

                const riPremInstitutionDto: RIPremInstitutionDTO = {
                    ...riProcedure.RIPremInstitution,
                };
                await transactionManager.getRepository(RIPremInstitution).save({
                    ...riPremInstitutionDto,
                    // important to be NULL to prevent update of resource if ID exist in request
                    RIPremInstitutionID: null,
                    // maybe PremInstitutionID should be InstitutionID of miToUpdate (host Institution)
                    // PremInstitutionID: reqBody.miToUpdate.InstitutionID,
                    RIProcedure: riProcedure,
                });
            }
            await this.auditService.insertAudit(
                {
                    authedRequest,
                    action: AuditAction.UPDATE,
                    objectName: AuditObjectName.RI_INSTITUTION,
                    objectID: reqBody.miToUpdate.InstitutionID,
                    oldValue: reqBody.misToDelete,
                    newValue: reqBody.miToUpdate,
                },
                transactionManager,
            );
            for (const mitd of reqBody.misToDelete) {
                await this.service.azureSyncDeleteSchool(mitd, authedRequest, transactionManager);
            }
        });
        /**
         * Do not update host Institution on Join procedure !!
         * maybe PremInstitutionID should be InstitutionID of miToUpdate (host Institution)
         */

        return reqBody.miToUpdate;
    }
}
