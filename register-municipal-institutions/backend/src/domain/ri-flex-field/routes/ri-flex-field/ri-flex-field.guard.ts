import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { CondOperator, RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { Connection } from 'typeorm';
import { HttpMethodEnum } from '../../../../shared/enums/http-method.enum';
import { AuthedRequest, AuthObject } from '../../../../shared/interfaces/authed-request.interface';
import { RIFlexField } from '../../ri-flex-field.entity';

@Injectable()
export class RIFlexFieldGuard implements CanActivate {
    constructor(private connection: Connection) {}

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
        const parsed = RequestQueryParser.create()
            .parseQuery(req.query)
            .getParsed();

        /** Allow only where municipality is same as selectedRole.municipality */
        const scoped = RequestQueryBuilder.create().search({
            $and: [
                {
                    MunicipalityID: {
                        [CondOperator.EQUALS]: authObject.selectedRole.MunicipalityID,
                    },
                },
                parsed.search,
            ],
        }).queryObject;

        req.query.s = scoped.s;
        return true;
    }

    private grantCreateAccess(authObject: AuthObject) {
        return authObject.isMunicipality;
    }

    private async grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        if (!authObject.isMunicipality) {
            return false;
        }

        const userMunicipalityID = authObject.selectedRole.MunicipalityID;
        const target = await this.connection
            .getRepository(RIFlexField)
            .findOne(+req.params.RIFlexFieldID);
        return userMunicipalityID === target.MunicipalityID;
    }

    private async grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        if (!authObject.isMunicipality) {
            return false;
        }

        const userMunicipalityID = authObject.selectedRole.MunicipalityID;
        const target = await this.connection
            .getRepository(RIFlexField)
            .findOne(+req.params.RIFlexFieldID);
        return userMunicipalityID === target.MunicipalityID;
    }
}
