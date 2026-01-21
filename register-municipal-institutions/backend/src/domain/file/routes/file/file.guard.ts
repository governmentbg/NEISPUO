import { Injectable, CanActivate, ExecutionContext, Inject } from '@nestjs/common';
import { HttpMethodEnum } from '../../../../shared/enums/http-method.enum';
import { AuthedRequest } from '../../../../shared/middleware/auth.middleware';
import { InjectRepository } from '@nestjs/typeorm';
import { File } from '../../file.entity';
import { Repository } from 'typeorm';
import { AuthObject } from '../../../../shared/interfaces/authed-request.interface';

@Injectable()
export class FileGuard implements CanActivate {
    constructor(@InjectRepository(File) private fileRepo: Repository<File>) {}

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
        if (!authObject) {
            return false;
        }

        let accessGranted: boolean = false;
        if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = this.grantCreateAccess(authObject);
        }
        if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = this.grantUpdateAccess(authObject, req);
        }
        if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = await this.grantDeleteAccess(authObject, req);
        }
        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = await this.grantReadAccess(authObject, req);
        }

        return accessGranted;
    }

    private grantCreateAccess(authObject: AuthObject) {
        return !!authObject;
    }

    private grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private async grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false; // -> not allowing hard deletes for now
        // const fileBelongsToUser = await this.fileRepo.findOne({
        //     id: req.params.fileId,
        //     user: authObject.user
        // });
        // return !!fileBelongsToUser;
    }

    private async grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        return true;
    }
}
