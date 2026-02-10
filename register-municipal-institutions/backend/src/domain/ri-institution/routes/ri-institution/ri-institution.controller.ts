import { EnumBaseSchoolType } from '@domain/base-school-type/enums/base-school-type-enum';
import { Institution } from '@domain/institution/institution.entity';
import { InstitutionDto } from '@domain/institution/routes/institution/institution.dto';
import { InstitutionService } from '@domain/institution/routes/institution/institution.service';
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
import {
 BadRequestException, Body, Controller, Get, Param, Req, UseGuards,
} from '@nestjs/common';
import {
    Crud,
    CrudController,
    CrudOptions,
    CrudRequest,
    Override,
    ParsedBody,
    ParsedRequest,
} from '@nestjsx/crud';
import { AuditAction } from '@shared/enums/audit-action';
import { AuditObjectName } from '@shared/enums/audit-object-name';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { Connection } from 'typeorm';
import { RIInstitution } from '../../ri-institution.entity';
import { RIInstitutionDTO } from './ri-institution.dto';
import { RIInstitutionGuard } from './ri-institution.guard';
import { RIInstitutionService } from './ri-institution.service';

export const riInstitutionCrudOptions: CrudOptions = {
    model: {
        type: RIInstitution,
    },
    params: {
        RIInstitutionID: {
            type: 'number',
            primary: true,
            field: 'RIInstitutionID',
        },
    },
    routes: {
        only: ['getManyBase', 'getOneBase', 'createOneBase', 'updateOneBase', 'deleteOneBase'],
    },
    query: {
        alwaysPaginate: true,
        join: {
            RIFlexFieldValues: { eager: true },
            'RIFlexFieldValues.RIFlexField': { eager: true },
            RIProcedure: { eager: true },
            'RIProcedure.RICPLRArea': { eager: true, alias: 'ri-procedure_ri-cplr-area' },
            'RIProcedure.RICPLRArea.CPLRAreaType': {
                eager: true,
                alias: 'ri-procedure_ri-cplr-area_cplr-area-type',
            },
            'RIProcedure.RIDocument': { eager: true },
            'RIProcedure.ProcedureType': { eager: true },
            'RIProcedure.RIInstitutionDepartments': {
                eager: true,
                alias: 'ri-procedure_ri-institution-departments',
            },
            'RIProcedure.RIInstitutionDepartments.Town': {
                eager: true,
                alias: 'ri-procedure_ri-institution-departments_town',
            },
            'RIProcedure.RIInstitutionDepartments.Country': {
                eager: true,
                alias: 'ri-procedure_ri-institution-departments_country',
            },
            'RIProcedure.RIPremInstitution': { eager: true },
            'RIProcedure.TransformType': { eager: true },
            // Institution: { eager: true },
            Town: { eager: true },
            'Town.Municipality': { eager: true, alias: 'town_municipality' },
            'Town.Municipality.Region': { eager: true, alias: 'town_municipality_region' },
            Country: { eager: true },
            LocalArea: { eager: true },
            FinancialSchoolType: { eager: true },
            DetailedSchoolType: { eager: true },
            BudgetingInstitution: { eager: true },
            SysUser: { eager: true },
            BaseSchoolType: { eager: true },
        },
        filter: [
            {
                field: 'FinancialSchoolType.FinancialSchoolTypeID',
                operator: '$eq',
                value: 2,
            },
            {
                field: 'BudgetingInstitution.BudgetingInstitutionID',
                operator: '$eq',
                value: 5,
            },
            {
                field: 'BaseSchoolType.BaseSchoolTypeID',
                operator: '$in',
                value: [
                    EnumBaseSchoolType.KINDERGARTEN,
                    EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE,
                ],
            },
        ],
        sort: [
            {
                field: 'InstitutionID',
                order: 'ASC',
            },
        ],
    },
};
@UseGuards(RIInstitutionGuard)
@Crud(riInstitutionCrudOptions)
@Controller('v1/ri-institution')
export class RIInstitutionController implements CrudController<RIInstitution> {
    get base(): CrudController<RIInstitution> {
        return this;
    }

    constructor(
        public service: RIInstitutionService,
        public institutionService: InstitutionService,
        private connection: Connection,
        public riProcedureService: RIProcedureService,
        public auditService: AuditService,
    ) {}

    // override POST
    @Override()
    async createOne(
        @Req() authedRequest: AuthedRequest,
        @ParsedRequest() req: CrudRequest,
        @ParsedBody() body: any,
    ) {
        return await this.connection.transaction(async (transactionManager) => {
            const procedureDto: RIProcedureDto = body.RIProcedure;

            procedureDto.InstitutionID = await this.service.generateInstitutionID(body.Town);

            const isBulstatValid = this.institutionService.validateBulstat(body.Bulstat);
            if (!isBulstatValid) {
                throw new BadRequestException('Bulstat is invalid.');
            }

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
                { ...procedureDto },
                transactionManager,
            );

            let riProcedure = await this.riProcedureService.createProcedure(
                {
                    ...procedureDto,
                    TransformType: createTransformType,
                    ProcedureType: createProcedureType,
                    StatusType: procedureStatusType,
                    // last procedure should be IsActive = 1
                    IsActive: true,
                    Ord: order,
                },
                transactionManager,
            );

            const CPLRArea = body.CPLRAreaType?.CPLRAreaTypeID;
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
                ...body,
                InstitutionID: riProcedure.InstitutionID,
                RIProcedure: riProcedure,
            };
            const riInstitution = await transactionManager
                .getRepository(RIInstitution)
                .save(riInstitutionDto);

            for (const flexFieldValue of body.RIFlexFieldValues) {
                const riFlexFieldValueDto: RIFlexFieldValueDto = {
                    ...flexFieldValue,
                    RIInstitution: riInstitution,
                };
                await transactionManager.getRepository(RIFlexFieldValue).save(riFlexFieldValueDto);
            }

            const institution = {
                ...riInstitution,
                Name: riInstitution.Name,
                Abbreviation: riInstitution.Abbreviation,
            } as InstitutionDto;
            await transactionManager.getRepository(Institution).save(institution);

            await this.service.azureSyncCreateSchool(
                riInstitution,
                authedRequest,
                transactionManager,
            );

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

            return riInstitution;
        });
    }

    @Override()
    async updateOne(
        @Req() authedRequest: AuthedRequest,
        @ParsedRequest() req: CrudRequest,
        @Param() params: any,
        @Body() body: any,
    ) {
        return await this.connection.transaction(async (transactionManager) => {
            // important to be NULL to prevent update of resource if ID exist in request
            if (body.RIProcedure?.RICPLRArea) {
                delete body.RIProcedure.RICPLRArea.RICPLRAreaID;
            }
            if (body.RIProcedure?.RIInstitutionDepartments) {
                for (const institutionDepartment of body.RIProcedure.RIInstitutionDepartments) {
                    delete institutionDepartment.RIInstitutionDepartmentID;
                }
            }

            const procedureDto: RIProcedureDto = {
                ...body.RIProcedure,
                // important to be NULL to prevent update of resource if ID exist in request
                RIProcedureID: null,
                InstitutionID: body.InstitutionID,
            };

            /**
             * промяна в обстоятелства
             */
            let transformTypeID = 15;

            // get RIInstitutionID from request param in url
            const riInstitutionForChange = await transactionManager
                .getRepository(RIInstitution)
                .findOne(body.RIInstitutionID);
            if (
                riInstitutionForChange.RIProcedure.RICPLRArea !== null
                && riInstitutionForChange.BaseSchoolType.BaseSchoolTypeID
                    === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE
                && riInstitutionForChange?.RIProcedure?.RICPLRArea?.CPLRAreaType?.CPLRAreaTypeID
                    !== body.CPLRAreaType.CPLRAreaTypeID
            ) {
                /**
                 * промяна в предмета на дейност
                 */
                transformTypeID = 17;
            }
            const updateTransformType = await transactionManager
                .getRepository(TransformType)
                .findOne(transformTypeID);
            /**
             * Промяна/ преобразуване на институция
             */
            const updateProcedureType = await transactionManager
                .getRepository(ProcedureType)
                .findOne(ProcedureTypeEnum.UPDATE);
            /**
             * Статус на процедура - приключена / потвърдена
             */
            const procedureStatusType = await transactionManager
                .getRepository(StatusType)
                .findOne(StatusTypeEnum.COMPLETED_AND_CONFIRMED);

            // TODO: move logic in service
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

            let riProcedure = await this.riProcedureService.createProcedure(
                {
                    ...procedureDto,
                    TransformType: updateTransformType,
                    ProcedureType: updateProcedureType,
                    StatusType: procedureStatusType,
                    // last procedure should be IsActive = 1
                    IsActive: true,
                    Ord: order,
                },
                transactionManager,
            );

            const CPLRArea = body.CPLRAreaType?.CPLRAreaTypeID;
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
                await transactionManager.getRepository(RIFlexFieldValue).save(riFlexFieldValueDto);
            }

            await this.auditService.insertAudit(
                {
                    authedRequest,
                    action: AuditAction.UPDATE,
                    objectName: AuditObjectName.RI_INSTITUTION,
                    objectID: riInstitution.RIInstitutionID,
                    oldValue: riInstitution,
                    newValue: riInstitution,
                },
                transactionManager,
            );

            return riInstitution;
        });
    }

    @Override()
    async deleteOne(
        @Req() authedRequest: AuthedRequest,
        @ParsedRequest() req: CrudRequest,
        @Param() params: any,
        @Body() body: any,
    ) {
        // important to be NULL to prevent update of resource if ID exist in request
        delete body.RIProcedure?.RICPLRArea?.RICPLRAreaID;
        if (body.RIProcedure?.RIInstitutionDepartments) {
            for (const institutionDepartment of body.RIProcedure.RIInstitutionDepartments) {
                delete institutionDepartment.RIInstitutionDepartmentID;
            }
        }

        return await this.connection.transaction(async (transactionManager) => {
            const procedureDto: RIProcedureDto = {
                ...body.RIProcedure,
                // important to be NULL to prevent update of resource if ID exist in request
                RIProcedureID: null,
                InstitutionID: body.InstitutionID,
            };
            /**
             *  закрито
             */
            const deleteTransformType = await transactionManager
                .getRepository(TransformType)
                .findOne(TransformTypeEnum.CLOSE);
            /**
             * вид на процедура - Закриване/ отписване на институция
             */
            const deleteProcedureType = await transactionManager
                .getRepository(ProcedureType)
                .findOne(ProcedureTypeEnum.DELETE);
            /**
             * Статус на процедура - приключена / потвърдена
             */

            const deleteStatusType = await transactionManager
                .getRepository(StatusType)
                .findOne(StatusTypeEnum.COMPLETED_AND_CONFIRMED);

            // TODO: move logic in service
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

            let riProcedure = await this.riProcedureService.createProcedure(
                {
                    ...procedureDto,
                    TransformType: deleteTransformType,
                    ProcedureType: deleteProcedureType,
                    StatusType: deleteStatusType,
                    // last procedure should be IsActive = 1
                    IsActive: true,
                    Ord: order,
                },
                transactionManager,
            );

            const CPLRArea = body.CPLRAreaType?.CPLRAreaTypeID;
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
                await transactionManager.getRepository(RIFlexFieldValue).save(riFlexFieldValueDto);
            }

            const riPremInstitutionDto: RIPremInstitutionDTO = { ...riProcedure.RIPremInstitution };
            await transactionManager.getRepository(RIPremInstitution).save({
                ...riPremInstitutionDto,
                // important to be NULL to prevent update of resource if ID exist in request
                RIPremInstitutionID: null,
                RIProcedure: riProcedure,
            });

            await this.auditService.insertAudit(
                {
                    authedRequest,
                    action: AuditAction.DELETE,
                    objectName: AuditObjectName.RI_INSTITUTION,
                    objectID: riInstitution.RIInstitutionID,
                    oldValue: riInstitution,
                    newValue: null,
                },
                transactionManager,
            );

            await this.service.azureSyncDeleteSchool(
                riInstitution,
                authedRequest,
                transactionManager,
            );
            return riInstitution;
        });
    }

    @Get('history/:RIInstitutionID')
    async history(
        @ParsedRequest() req: CrudRequest,
        @Param('RIInstitutionID') RIInstitutionID: number,
    ) {
        const SelectedRIInstitution = await this.service.repo.findOneOrFail(RIInstitutionID);
        const institution = await this.service.repo.find({
            where: [{ InstitutionID: SelectedRIInstitution.InstitutionID }],
        });

        const instituionId = institution[0].InstitutionID;
        return await this.service.getHistory(instituionId);
    }
}
