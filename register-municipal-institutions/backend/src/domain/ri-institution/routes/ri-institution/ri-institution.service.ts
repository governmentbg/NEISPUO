import {
    Inject, Injectable, Logger, Req,
} from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Connection, Repository } from 'typeorm';
import { Town } from '@domain/town/town.entity';
import axios from 'axios';
import { RIProcedure } from '@domain/ri-procedure/ri-procedure.entity';
import { RIProcedureService } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.service';
import { RIProcedureDto } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.dto';
import { RIFlexFieldValueDto } from '@domain/ri-flex-field-value/routes/ri-flex-field-value/ri-flex-field-value.dto';
import { RIFlexFieldValue } from '@domain/ri-flex-field-value/ri-flex-field-value.entity';
import { RIPremInstitutionDTO } from '@domain/ri-prem-institution/routes/ri-prem-institution/ri-prem-institution.dto';
import { RIPremInstitution } from '@domain/ri-prem-institution/ri-prem-institution.entity';
import { AuditAction } from '@shared/enums/audit-action';
import { AuditObjectName } from '@shared/enums/audit-object-name';
import { AuditService } from '@domain/ri-institution/audit/audit.service';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import * as http from 'http';
import { UserManagementAPIRequestService } from '@domain/user-management-api-request/user-management-api-request.service';
import { ProcedureTypeEnum } from '../../../procedure-type/enums/procedure-type.enum';
import { RIInstitutionDTO } from './ri-institution.dto';
import { RIInstitution } from '../../ri-institution.entity';

@Injectable()
export class RIInstitutionService extends TypeOrmCrudService<RIInstitution> {
    constructor(
        public connection: Connection,
        public riProcedureService: RIProcedureService,
        public auditService: AuditService,
        public userManagementAPIRequestService: UserManagementAPIRequestService,
        // public authedRequest: AuthedRequest,
        @InjectRepository(RIInstitution) public repo: Repository<RIInstitution>,
        @Inject('winston') private readonly logger: Logger,
    ) {
        super(repo);
    }

    public async createRIInstitution(dto: RIInstitutionDTO, transactionManager: any) {
        const repo = transactionManager.getRepository(RIInstitution);
        return repo.save(dto);
    }

    public async generateInstitutionID(
        town: any,
        idOfPerviousInstitution?: number,
    ): Promise<number> {
        const townObj = await this.connection.manager.findOne(Town, town, {
            relations: ['Municipality'],
        });
        let regionId = townObj.Municipality.Region.RegionID;
        if (regionId === 3) {
            regionId = 4;
        } else if (regionId === 4) {
            regionId = 3;
        }
        const minId = idOfPerviousInstitution ? idOfPerviousInstitution + 1 : regionId * 100000;
        const maxId = regionId * 100000 + 99999;

        const list = await this.connection.manager
            .createQueryBuilder()
            .select('RIInstitution')
            .from(RIInstitution, 'RIInstitution')
            .where('RIInstitution.InstitutionID BETWEEN :idMin AND :idMax', {
                idMin: minId,
                idMax: maxId,
            })
            .orderBy('InstitutionID')
            .getMany();

        let possible: number;
        for (let i = minId; i < maxId; i += 1) {
            const result = list.find((obj) => obj.InstitutionID === i);
            if (result === undefined) {
                possible = i;
                break;
            }
        }

        return possible;
    }

    public async isRIInstitutionClosed(riInstitutionID: number): Promise<boolean> {
        const rIInstitution = await this.repo.findOne({
            where: { RIInstitutionID: riInstitutionID },
        });
        const riInstitutionWithProcedures = await this.repo.find({
            where: { InstitutionID: rIInstitution.InstitutionID },
            relations: ['RIProcedure', 'RIProcedure.ProcedureType'],
        });

        const riInstitutionHasCloseProcedure = riInstitutionWithProcedures.some(
            (riwp) => riwp.RIProcedure.ProcedureType.ProcedureTypeID === ProcedureTypeEnum.DELETE,
        );
        return riInstitutionHasCloseProcedure;
    }

    async azureSyncCreateSchool(
        institution: RIInstitution,
        authedRequest: AuthedRequest,
        transactionManager: any,
    ) {
        try {
            const httpAgent = new http.Agent();

            const httpOptions = {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: authedRequest.get('Authorization'),
                },
                httpAgent,
            };

            // eslint-disable-next-line @typescript-eslint/no-unused-vars
            const response = await axios.post(
                process.env.AZURE_USERMNG_URL_CREATE_SCHOOL,
                {
                    institutionID: institution.InstitutionID,
                },
                httpOptions,
            );

            await this.userManagementAPIRequestService.insertUserManagementAPIRequest(
                {
                    authedRequest,
                    Url: process.env.AZURE_USERMNG_URL_CREATE_SCHOOL,
                    Operation: 'EnrollmentSchoolCreate',
                    Response: 'StatusCode: 201, IsSuccessStatusCode: True',
                    ResponseHttpCode: 201,
                    IsError: 0,
                    InstitutionID: institution.InstitutionID,
                },
                transactionManager,
            );
        } catch (e) {
            await this.userManagementAPIRequestService.insertUserManagementAPIRequest(
                {
                    authedRequest,
                    Url: process.env.AZURE_USERMNG_URL_CREATE_SCHOOL,
                    Operation: 'EnrollmentSchoolCreate',
                    Response: `StatusCode: ${e?.response?.error
                        || 500},  IsSuccessStatusCode: False`,
                    ResponseHttpCode: e?.response?.statusCode || 500,
                    IsError: 1,
                    InstitutionID: institution.InstitutionID,
                },
                transactionManager,
            );

            this.logger.error(e);
        }
    }

    async azureSyncDeleteSchool(
        institution: RIInstitution,
        authedRequest: AuthedRequest,
        transactionManager: any,
    ) {
        try {
            const httpAgent = new http.Agent();
            const httpOptions = {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: authedRequest.get('Authorization'),
                },
                httpAgent,
            };

            // eslint-disable-next-line @typescript-eslint/no-unused-vars
            const response = await axios.post(
                process.env.AZURE_USERMNG_URL_DELETE_SCHOOL,
                {
                    institutionID: institution.InstitutionID,
                },
                httpOptions,
            );
            await this.userManagementAPIRequestService.insertUserManagementAPIRequest(
                {
                    authedRequest,
                    Url: process.env.AZURE_USERMNG_URL_DELETE_SCHOOL,
                    Operation: 'EnrollmentSchoolDelete',
                    Response: process.env.AZURE_USERMNG_SUCCESS_RESPONSE,
                    ResponseHttpCode: 201,
                    IsError: 0,
                    InstitutionID: institution.InstitutionID,
                },
                transactionManager,
            );
        } catch (e) {
            await this.userManagementAPIRequestService.insertUserManagementAPIRequest(
                {
                    authedRequest,
                    Url: process.env.AZURE_USERMNG_URL_DELETE_SCHOOL,
                    Operation: 'EnrollmentSchoolDelete',
                    Response: `StatusCode: ${e?.response?.error
                        || 500}, IsSuccessStatusCode: False`,
                    ResponseHttpCode: e?.response?.statusCode || 500,
                    IsError: 1,
                    InstitutionID: institution.InstitutionID,
                },
                transactionManager,
            );
            this.logger.error(e);
        }
    }

    public async createProcedure(
        @Req() authedRequest: AuthedRequest,
        body: any,
        transactionManager: any,
        transformType: any,
        procedureType: any,
        statusType: any,
    ) {
        // important to be NULL to prevent update of resource if ID exist in request
        if (body.miToUpdate.RIProcedure?.RICPLRArea) {
            delete body.miToUpdate.RIProcedure.RICPLRArea.RICPLRAreaID;
        }
        if (body.miToUpdate.RIProcedure?.RIInstitutionDepartments) {
            for (const institutionDepartment of body.miToUpdate.RIProcedure
                .RIInstitutionDepartments) {
                delete institutionDepartment.RIInstitutionDepartmentID;
            }
        }
        delete body.miToUpdate?.ValidFrom;
        delete body.miToUpdate?.ValidTo;

        await this.auditService.insertAudit(
            {
                authedRequest,
                action: AuditAction.UPDATE,
                objectName: AuditObjectName.RI_PROCEDURE,
                objectID: body.miToUpdate.InstitutionID,
                oldValue: body.misToDelete,
                newValue: body.miToUpdate,
            },
            transactionManager,
        );

        const procedureDto: RIProcedureDto = {
            ...body.miToUpdate.RIProcedure,
            // important to be NULL to prevent update of resource if ID exist in request
            RIProcedureID: null,
            InstitutionID: body.miToUpdate.InstitutionID,
        };

        const oldProcedures = await transactionManager
            .getRepository(RIProcedure)
            .find({ InstitutionID: body.miToUpdate.InstitutionID });
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
                TransformType: transformType,
                ProcedureType: procedureType,
                StatusType: statusType,
                // last procedure should be IsActive = 1
                IsActive: true,
                Ord: order,
            },
            transactionManager,
        );

        return riProcedure;
    }

    public async deleteProcedures(
        reqBody: any,
        transactionManager: any,
        joinedTransformType: any,
        deleteProcedureType: any,
        statusType: any,
        authedRequest: AuthedRequest,
    ) {
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

            for (const flexFieldValue of body.RIFlexFieldValues) {
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
                // maybe PremInstitutionID should be InstitutionID of miToUpdate (host Institution)
                // PremInstitutionID: reqBody.miToUpdate.InstitutionID,
                RIProcedure: riProcedure,
            });
        }
    }

    public async getHistory(institutionID: number) {
        return await this.repo
            .createQueryBuilder('InstitutionID')
            .leftJoinAndSelect('InstitutionID.RIProcedure', 'RIProcedure')
            .leftJoinAndSelect('RIProcedure.ProcedureType', 'ProcedureType')
            .where('RIProcedure.InstitutionID = :id', { id: institutionID })
            .orderBy('RIProcedure.Ord')
            .getMany();
    }
}
