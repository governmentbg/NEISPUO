import { Injectable, CanActivate, ExecutionContext, Inject } from '@nestjs/common';
import { AuthedRequest, AuthObject } from 'src/shared/interfaces/authed-request.interface';
import { User } from '../../user.entity';
import { HttpMethodEnum } from '../../../../shared/enums/http-method.enum';
import { RoleNameEnum } from 'src/domain/role/enums/role-name.enum';
import { RegisterService } from './register.service';

@Injectable()
export class RegisterGuard implements CanActivate {
    constructor(@Inject('RegisterService') private readonly registerService: RegisterService) {}
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
        let accessGranted: boolean = true;

        if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = this.grantCreateAccess(authObject);
        } else if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = await this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = await this.grantDeleteAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = this.grantReadAccess(authObject, req);
        }

        return accessGranted;
    }

    private grantCreateAccess(authObject: AuthObject) {
        return true;
    }

    private async grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private async grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }
}
