import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { CondOperator, RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { HttpMethodEnum } from 'src/common/constants/enum/http-method.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { AuthObject, AuthedRequest } from 'src/common/dto/authed-request.interface';
import { Connection } from 'typeorm';

@Injectable()
export class RuoAzureGuard implements CanActivate {
    constructor(private connection: Connection) {}

    canActivate(ctx: ExecutionContext): boolean {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private grantAccess(reqMethod: HttpMethodEnum, authObject: AuthObject, req: AuthedRequest) {
        let accessGranted = false;
        if (!authObject?.selectedRole) {
            return false;
        }

        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = this.grantReadAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = this.grantCreateAccess(authObject);
        } else if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = this.grantDeleteAccess(authObject, req);
        }

        return accessGranted;
    }

    private grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isMunicipality } = authObject;

        const parsed = RequestQueryParser.create().parseQuery(req.query).getParsed();

        const scopeRuo = RequestQueryBuilder.create().search({
            $and: [
                {
                    $or: [
                        { SysRoleID: { [CondOperator.EQUALS]: RoleEnum.RUO } },
                        { SysRoleID: { [CondOperator.EQUALS]: RoleEnum.RUO_EXPERT } },
                    ],
                },
                parsed.search,
            ],
        }).queryObject;

        req.query.s = scopeRuo.s;

        if (isMon) {
            return true;
        } else if (isRuo) {
            this.scopeAccessForRuo(authObject, req);
            return true;
        }
        return false;
    }

    private grantCreateAccess(authObject: AuthObject) {
        return false;
    }

    private grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
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
}
