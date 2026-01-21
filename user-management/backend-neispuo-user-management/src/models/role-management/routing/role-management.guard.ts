import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { HttpMethodEnum } from 'src/common/constants/enum/http-method.enum';
import { Connection, Equal } from 'typeorm';
import { AuthObject, AuthedRequest } from 'src/common/dto/authed-request.interface';
import { InstitutionEntity } from 'src/common/entities/institution.entity';

@Injectable()
export class RoleManagementGuard implements CanActivate {
    constructor(private connection: Connection) {}

    async canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private canInstitutionModifyRows(institutionID: number, creatorInstitutionID: number) {
        return institutionID === creatorInstitutionID;
    }

    private async canRUOModifyRows(institutionID: number, regionID: number) {
        const allowedInstitutions = (
            await this.connection.getRepository(InstitutionEntity).find({
                where: { town: { municipality: { region: { regionID: Equal(regionID) } } } },
                relations: ['town', 'town.municipality', 'town.municipality.region'],
            })
        ).map((i) => i.institutionID);
        return allowedInstitutions.includes(institutionID);
    }

    private async grantAccess(reqMethod: HttpMethodEnum, authObject: AuthObject, req: AuthedRequest) {
        let accessGranted = false;
        if (!authObject?.selectedRole) {
            return false;
        }

        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = this.grantReadAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = await this.grantCreateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = this.grantDeleteAccess(authObject, req);
        }

        return accessGranted;
    }

    private grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool } = authObject;

        if (isMon) {
            return true;
        } else if (isRuo) {
            return true;
        } else if (isSchool) {
            return true;
        }

        return false;
    }

    private async grantCreateAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool, selectedRole } = authObject;
        const institutionID = req.body.institutionID;
        if (isMon) {
            return true;
        } else if (isRuo) {
            const regionID = selectedRole.RegionID;
            const canModify = await this.canRUOModifyRows(institutionID, regionID);
            return canModify;
        } else if (isSchool) {
            const creatorInstitutionID = selectedRole.InstitutionID;
            const canModify = this.canInstitutionModifyRows(institutionID, creatorInstitutionID);
            return canModify;
        }

        return false;
    }

    private grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }
}
