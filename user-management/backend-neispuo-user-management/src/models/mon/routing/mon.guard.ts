import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { CondOperator, RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { HttpMethodEnum } from 'src/common/constants/enum/http-method.enum';
import { Connection } from 'typeorm';
import { AuthObject, AuthedRequest } from 'src/common/dto/authed-request.interface';

@Injectable()
export class MonGuard implements CanActivate {
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
        const { isMonAdmin, isHelpDesk } = authObject;

        const parsed = RequestQueryParser.create().parseQuery(req.query).getParsed();

        const scopeMon = RequestQueryBuilder.create().search({
            $and: [
                {
                    $or: [
                        { SysRoleID: { [CondOperator.EQUALS]: RoleEnum.MON_ADMIN } },
                        { SysRoleID: { [CondOperator.EQUALS]: RoleEnum.MON_EXPERT } },
                        { SysRoleID: { [CondOperator.EQUALS]: RoleEnum.MON_OBGUM } },
                        { SysRoleID: { [CondOperator.EQUALS]: RoleEnum.MON_OBGUM_FINANCES } },
                        { SysRoleID: { [CondOperator.EQUALS]: RoleEnum.MON_CHRAO } },
                        { SysRoleID: { [CondOperator.EQUALS]: RoleEnum.MON_USER_ADMIN } },
                    ],
                },
                parsed.search,
            ],
        }).queryObject;

        req.query.s = scopeMon.s;

        if (isMonAdmin) {
            return true;
        }

        if (isHelpDesk) {
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
}
