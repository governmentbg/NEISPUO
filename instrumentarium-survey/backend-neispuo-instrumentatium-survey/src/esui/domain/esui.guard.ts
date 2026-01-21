import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import {
    AuthedRequest,
    AuthObject,
} from '@shared/interfaces/authed-request.interface';
import { HttpMethodEnum } from 'src/shared/enums/http-method.enum';
import { Connection } from 'typeorm';
@Injectable()
export class EsuiGuard implements CanActivate {

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
        if(!process.env.ESUI_ACCESS_IPS) return false;
        
        const allowedIps = process.env.ESUI_ACCESS_IPS.split(",");
        // req.ip gets the real client IP address only if 
        // backend nginx configuration (/etc/nginx/sites-available/surveys-backend) contains this:
        // proxy_set_header X-Forwarded-For $remote_addr;
        const clientIp = req.ip;

        return allowedIps.includes(clientIp);
    }
}
