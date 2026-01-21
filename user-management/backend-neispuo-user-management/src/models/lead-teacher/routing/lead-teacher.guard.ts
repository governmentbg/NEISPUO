import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { CondOperator, RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { HttpMethodEnum } from 'src/common/constants/enum/http-method.enum';
import { AuthObject, AuthedRequest } from 'src/common/dto/authed-request.interface';
import { Connection } from 'typeorm';

@Injectable()
export class LeadTeacherGuard implements CanActivate {
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
            accessGranted = await this.grantCreateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = this.grantDeleteAccess(authObject, req);
        }

        return accessGranted;
    }

    private async grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isLeadTeacher } = authObject;

        if (isLeadTeacher) {
            await this.scopeAccessForLeadTeacher(authObject, req);
            return true;
        }
        return false;
    }

    private async grantCreateAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isLeadTeacher } = authObject;

        if (isLeadTeacher) {
            await this.scopeAccessForLeadTeacher(authObject, req);
            return true;
        }
        return false;
    }

    private grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        return false;
    }

    private async scopeAccessForLeadTeacher(authObject: AuthObject, req: AuthedRequest) {
        const parsed = RequestQueryParser.create().parseQuery(req.query).getParsed();

        const subClasses = await this.connection
            .createQueryBuilder()
            .select('ClassID')
            .from('inst_year.ClassGroup', 'cg')
            .where('cg.ParentClassID IN (:...parentClassIDs)', {
                parentClassIDs: authObject.selectedRole.LeadTeacherClasses,
            })
            .printSql()
            .getRawMany();

        const scoped = RequestQueryBuilder.create().search({
            $and: [
                {
                    ClassID: {
                        [CondOperator.IN]: [...subClasses.map((sc) => sc.ClassID)],
                    },
                },
                parsed.search,
            ],
        }).queryObject;

        req.query.s = scoped.s;
    }
}
