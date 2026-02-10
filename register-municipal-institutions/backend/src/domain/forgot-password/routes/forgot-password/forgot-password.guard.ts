import { Injectable, CanActivate, ExecutionContext } from '@nestjs/common';
import { HttpMethodEnum } from '../../../../shared/enums/http-method.enum';
import { AuthedRequest } from '@shared/middleware/auth.middleware';
import { AuthObject } from '@shared//interfaces/authed-request.interface';

@Injectable()
export class ForgotPasswordGuard implements CanActivate {
    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject);
    }

    private async grantAccess(reqMethod: HttpMethodEnum, authObject: AuthObject) {
        let accessGranted: boolean = false;

        if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = this.grantCreateAccess(authObject);
        }
        if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = this.grantUpdateAccess(authObject);
        }
        if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = this.grantDeleteAccess(authObject);
        }
        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = this.grantReadAccess(authObject);
        }

        return accessGranted;
    }
    private grantCreateAccess(authObject: AuthObject) {
        // anyone can send forgot-password
        return true;
    }
    private grantUpdateAccess(authObject: AuthObject) {
        // no one can update forgot-password
        return false;
    }
    private grantDeleteAccess(authObject: AuthObject) {
        // no one can delete forgot-password
        return false;
    }
    private grantReadAccess(authObject: AuthObject) {
        // no one can read forgot-password
        return false;
    }
}
