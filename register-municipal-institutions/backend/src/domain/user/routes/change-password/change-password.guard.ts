import { Injectable, CanActivate, ExecutionContext } from '@nestjs/common';
import { AuthedRequest, AuthObject } from '../../../../shared/interfaces/authed-request.interface';
import { HttpMethodEnum } from '../../../../shared/enums/http-method.enum';

@Injectable()
export class ChangePasswordGuard implements CanActivate {
    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest.params.id);
    }

    private async grantAccess(
        reqMethod: HttpMethodEnum,
        authObject: AuthObject,
        requestedUserUuid: string
    ) {
        let accessGranted: boolean = false;
        if (!authObject) {
            return false;
        }

        if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = await this.grantUpdateAccess(authObject, requestedUserUuid);
        }

        return accessGranted;
    }

    private async grantUpdateAccess(authObject: AuthObject, requestedUserUuid: string) {
        return true;
    }
}
