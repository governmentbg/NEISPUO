import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { HttpMethodEnum } from 'src/common/constants/enum/http-method.enum';
import { Connection, Equal } from 'typeorm';
import { AuthObject, AuthedRequest } from 'src/common/dto/authed-request.interface';
import { InstitutionEntity } from 'src/common/entities/institution.entity';

@Injectable()
export class SchoolBooksGuard implements CanActivate {
    constructor(private connection: Connection) {}

    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private async grantAccess(reqMethod: HttpMethodEnum, authObject: AuthObject, req: AuthedRequest) {
        let accessGranted = false;
        if (!authObject?.selectedRole) {
            return false;
        }

        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = await this.grantReadAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = this.grantCreateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = this.grantDeleteAccess(authObject, req);
        }

        return accessGranted;
    }

    private async grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool } = authObject;

        if (isMon) {
            return true;
        } else if (isSchool) {
            await this.scopeAccessForInstitution(authObject, req);
            return true;
        } else if (isRuo) {
            await this.scopeAccessForRuo(authObject, req);
            return true;
        }

        return false;
    }

    private scopeAccessForInstitution(authObject: AuthObject, req: AuthedRequest) {
        req.query.institutionID = `${authObject?.selectedRole?.InstitutionID}`;
    }

    private async scopeAccessForRuo(authObject: AuthObject, req: AuthedRequest) {
        const { RegionID: regionID } = authObject?.selectedRole;
        const { institutionID } = req.query;
        const allowedInstitutions = (
            await this.connection.getRepository(InstitutionEntity).find({
                where: { town: { municipality: { region: { regionID: Equal(regionID) } } } },
                relations: ['town', 'town.municipality', 'town.municipality.region'],
            })
        ).map((i) => i.institutionID);
        if (!allowedInstitutions.includes(+institutionID)) req.query.institutionID = null;
    }

    private grantCreateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }
}
