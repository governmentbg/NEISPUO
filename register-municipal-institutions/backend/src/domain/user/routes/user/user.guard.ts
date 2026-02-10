import { Injectable, CanActivate, ExecutionContext, Inject } from '@nestjs/common';
import { AuthedRequest, AuthObject } from '../../../../shared/interfaces/authed-request.interface';
import { HttpMethodEnum } from '../../../../shared/enums/http-method.enum';
import { UserService } from './user.service';
import { RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';

@Injectable()
export class UserGuard implements CanActivate {
    constructor(@Inject('UserService') private readonly userService: UserService) {}
    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private async grantAccess(
        reqMethod: HttpMethodEnum,
        authObject: AuthObject,
        req: AuthedRequest
    ) {
        let accessGranted: boolean = false;
        if (!authObject) {
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
        // return authObject.isAdmin || authObject.user.SysUserID === +req.params?.id;
        return false;
    }

    private grantCreateAccess(authObject: AuthObject) {
        // return authObject && authObject.isAdmin;
        return false;
    }

    private async grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        // return authObject.isAdmin || authObject.user.SysUserID === +req.params?.id;
        return false;
    }

    private async grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }
}
