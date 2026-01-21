import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { Connection } from 'typeorm';
import { SysRoleEnum } from '../../domain/sys-role/enums/sys-role.enum';
import { HttpMethodEnum } from '../enums/http-method.enum';
import { AuthedRequest, AuthObject } from '../interfaces/authed-request.interface';

@Injectable()
export class BasicAuthReadOnlyGuard implements CanActivate {
    private allowedRoles = [
        SysRoleEnum.MON_ADMIN,
        SysRoleEnum.MON_EXPERT,
        SysRoleEnum.MON_CHRAO,
        SysRoleEnum.MON_OBGUM,
        SysRoleEnum.MON_OBGUM_FINANCES,
        SysRoleEnum.RUO,
        SysRoleEnum.RUO_EXPERT,
        SysRoleEnum.MUNICIPALITY,
    ];

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
        const userRole = authObject.selectedRole.SysRoleID;
        return this.allowedRoles.includes(userRole);
    }

    private grantCreateAccess(authObject: AuthObject) {
        return false;
    }

    private async grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private async grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }
}
