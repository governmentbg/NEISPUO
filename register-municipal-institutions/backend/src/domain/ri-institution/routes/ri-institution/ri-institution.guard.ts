import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { CondOperator, RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { Connection } from 'typeorm';
import { HttpMethodEnum } from '../../../../shared/enums/http-method.enum';
import { AuthedRequest, AuthObject } from '../../../../shared/interfaces/authed-request.interface';
import { RIInstitution } from '../../ri-institution.entity';
import { RIInstitutionService } from './ri-institution.service';

@Injectable()
export class RIInstitutionGuard implements CanActivate {
    constructor(
        private connection: Connection,
        private riInstitutionService: RIInstitutionService,
    ) {}

    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private async grantAccess(
        reqMethod: HttpMethodEnum,
        authObject: AuthObject,
        req: AuthedRequest,
    ) {
        let accessGranted = false;
        if (!authObject?.selectedRole) {
            return false;
        }

        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = this.grantReadAccess(authObject, req);
        }
        if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = this.grantCreateAccess(authObject);
        } else if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = await this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = await this.grantDeleteAccess(authObject, req);
        }

        return accessGranted;
    }

    private grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isMunicipality } = authObject;

        if (isMon) {
            return true;
        }
        if (isRuo) {
            this.scopeAccessForRuo(authObject, req);
            return true;
        }
        if (isMunicipality) {
            this.scopeAccessForMunicipality(authObject, req);
            return true;
        }
        return false;
    }

    private grantCreateAccess(authObject: AuthObject) {
        return authObject.isMunicipality;
    }

    private async grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        if (!authObject.isMunicipality) {
            return false;
        }

        const riInstitutionID = +req.params.RIInstitutionID;
        const isClosed = await this.riInstitutionService.isRIInstitutionClosed(riInstitutionID);
        if (isClosed) {
            return false;
        }

        // check if is the owner municipality
        const userMunicipalityID = authObject.selectedRole.MunicipalityID;
        const target = await this.connection
            .getRepository(RIInstitution)
            .findOne(riInstitutionID, { relations: ['Town', 'Town.Municipality'] });
        return userMunicipalityID === target.Town.Municipality.MunicipalityID;
    }

    private async grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        if (!authObject.isMunicipality) {
            return false;
        }

        const riInstitutionID = +req.params.RIInstitutionID;
        const isClosed = await this.riInstitutionService.isRIInstitutionClosed(riInstitutionID);
        if (isClosed) {
            return false;
        }

        // check if is the owner municipality
        const userMunicipalityID = authObject.selectedRole.MunicipalityID;
        const target = await this.connection
            .getRepository(RIInstitution)
            .findOne(riInstitutionID, { relations: ['Town', 'Town.Municipality'] });
        return userMunicipalityID === target.Town.Municipality.MunicipalityID;
    }

    private scopeAccessForRuo(authObject: AuthObject, req: AuthedRequest) {
        const parsed = RequestQueryParser.create()
            .parseQuery(req.query)
            .getParsed();

        /** Allow only where region is same as selectedRole.region */
        const scoped = RequestQueryBuilder.create().search({
            $and: [
                {
                    'town_municipality_region.RegionID': {
                        [CondOperator.EQUALS]: authObject.selectedRole.RegionID,
                    },
                },
                parsed.search,
            ],
        }).queryObject;

        req.query.s = scoped.s;
    }

    private scopeAccessForMunicipality(authObject: AuthObject, req: AuthedRequest) {
        const parsed = RequestQueryParser.create()
            .parseQuery(req.query)
            .getParsed();

        /** Allow only where municipality is same as selectedRole.municipality */
        const scoped = RequestQueryBuilder.create().search({
            $and: [
                {
                    'town_municipality.MunicipalityID': {
                        [CondOperator.EQUALS]: authObject.selectedRole.MunicipalityID,
                    },
                },
                parsed.search,
            ],
        }).queryObject;

        req.query.s = scoped.s;
    }
}
