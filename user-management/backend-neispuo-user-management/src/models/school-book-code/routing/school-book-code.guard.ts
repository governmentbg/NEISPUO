import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { HttpMethodEnum } from 'src/common/constants/enum/http-method.enum';
import { Connection, Equal } from 'typeorm';
import { AuthObject, AuthedRequest } from 'src/common/dto/authed-request.interface';
import { InstitutionEntity } from 'src/common/entities/institution.entity';
import { CondOperator, RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';

@Injectable()
export class SchoolBookCodeGuard implements CanActivate {
    constructor(private connection: Connection) {}

    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private async canInstitutionModifyRows(personIDs: number[], institutionID: number) {
        const requestedRows = await this.connection
            .createQueryBuilder()
            .select('es.PersonID', 'personID')
            .from('core.EducationalState', 'es')
            .where(`es.PersonID IN (:...personIDs)`, { personIDs })
            .andWhere(`es.InstitutionID = :institutionID`, { institutionID })
            .orderBy('PersonID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.personID));
        return (
            personIDs.length === requestedRows.length &&
            personIDs.sort((a, b) => a - b).every((value, index) => value === requestedRows[index])
        );
    }

    private async canLeadTeacherModifyRows(personIDs: number[], institutionID: number) {
        const requestedRows = await this.connection
            .createQueryBuilder()
            .select('es.PersonID', 'personID')
            .from('core.EducationalState', 'es')
            .where(`es.PersonID IN (:...personIDs)`, { personIDs })
            .andWhere(`es.InstitutionID = :institutionID`, { institutionID })
            .orderBy('PersonID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.personID));
        return (
            personIDs.length === requestedRows.length &&
            personIDs.sort((a, b) => a - b).every((value, index) => value === requestedRows[index])
        );
    }

    private async canRUOModifyRows(personIDs: number[], regionID: number) {
        const allowedInstitutions = (
            await this.connection.getRepository(InstitutionEntity).find({
                where: { town: { municipality: { region: { regionID: Equal(regionID) } } } },
                relations: ['town', 'town.municipality', 'town.municipality.region'],
            })
        ).map((i) => i.institutionID);

        const requestedRows = await this.connection
            .createQueryBuilder()
            .select('es.PersonID', 'personID')
            .from('core.EducationalState', 'es')
            .where(`es.PersonID IN (:...personIDs)`, { personIDs })
            .andWhere(`es.InstitutionID IN (:...allowedInstitutions)`, { allowedInstitutions })
            .orderBy('PersonID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.personID));
        return (
            personIDs.length === requestedRows.length &&
            personIDs.sort((a, b) => a - b).every((value, index) => value === requestedRows[index])
        );
    }

    private async grantAccess(reqMethod: HttpMethodEnum, authObject: AuthObject, req: AuthedRequest) {
        let accessGranted = false;
        if (!authObject?.selectedRole) {
            return false;
        }

        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = this.grantReadAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = await this.grantCreateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = this.grantDeleteAccess(authObject, req);
        }

        return accessGranted;
    }

    private grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool } = authObject;

        if (isMon) {
            return true;
        } else if (isSchool) {
            this.scopeAccessForInstitution(authObject, req);
            return true;
        } else if (isRuo) {
            this.scopeAccessForRuo(authObject, req);
            return true;
        }

        return false;
    }

    private scopeAccessForInstitution(authObject: AuthObject, req: AuthedRequest) {
        const parsed = RequestQueryParser.create().parseQuery(req.query).getParsed();
        /** Allow only where region is same as selectedRole.region */
        const scoped = RequestQueryBuilder.create().search({
            $and: [
                {
                    institutionID: {
                        [CondOperator.EQUALS]: authObject.selectedRole.InstitutionID,
                    },
                },
                parsed.search,
            ],
        }).queryObject;

        req.query.s = scoped.s;
    }

    private scopeAccessForRuo(authObject: AuthObject, req: AuthedRequest) {
        const parsed = RequestQueryParser.create().parseQuery(req.query).getParsed();
        /** Allow only where region is same as selectedRole.region */
        const scoped = RequestQueryBuilder.create().search({
            $and: [
                {
                    regionID: {
                        [CondOperator.EQUALS]: authObject.selectedRole.RegionID,
                    },
                },
                parsed.search,
            ],
        }).queryObject;

        req.query.s = scoped.s;
    }

    private async grantCreateAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool, selectedRole, isLeadTeacher } = authObject;
        const personIDs = req.body.personIDs;
        if (isMon) {
            return true;
        } else if (isRuo) {
            const regionID = selectedRole.RegionID;
            const canModify = await this.canRUOModifyRows(personIDs, regionID);
            return canModify;
        } else if (isSchool) {
            const institutionID = selectedRole.InstitutionID;
            const canModify = await this.canInstitutionModifyRows(personIDs, institutionID);
            return canModify;
        } else if (isLeadTeacher) {
            const institutionID = selectedRole.InstitutionID;
            const canModify = await this.canLeadTeacherModifyRows(personIDs, institutionID);
            return canModify;
        }

        return false;
    }

    private grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }
}
