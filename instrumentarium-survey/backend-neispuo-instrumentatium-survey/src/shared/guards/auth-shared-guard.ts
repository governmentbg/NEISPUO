import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { AuthedRequest, AuthObject } from '@shared/interfaces/authed-request.interface';
import { HttpMethodEnum } from 'src/shared/enums/http-method.enum';
import { Connection } from 'typeorm';

@Injectable()
export abstract class AuthSharedGuard implements CanActivate {
    constructor(private connection: Connection) {}

    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    async grantAccess(reqMethod: HttpMethodEnum, authObject: AuthObject, req: AuthedRequest) {
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
            accessGranted = this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = this.grantDeleteAccess(authObject, req);
        }

        return accessGranted;
    }

    abstract grantReadAccess(authObject: AuthObject, req: AuthedRequest): boolean;

    abstract grantCreateAccess(authObject: AuthObject): boolean;

    abstract grantUpdateAccess(authObject: AuthObject, req: AuthedRequest): boolean;

    abstract grantDeleteAccess(authObject: AuthObject, req: AuthedRequest): boolean;
}
