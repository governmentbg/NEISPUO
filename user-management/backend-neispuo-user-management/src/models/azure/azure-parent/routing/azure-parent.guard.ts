import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { HttpMethodEnum } from 'src/common/constants/enum/http-method.enum';
import { AuthObject, AuthedRequest } from 'src/common/dto/authed-request.interface';
import { Connection } from 'typeorm';

@Injectable()
export class AzureParentGuard implements CanActivate {
    constructor(private connection: Connection) {}

    canActivate(ctx: ExecutionContext): boolean {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private grantAccess(reqMethod: HttpMethodEnum, authObject: AuthObject, req: AuthedRequest) {
        const { hasApiKey } = authObject;

        return hasApiKey;
    }
}
