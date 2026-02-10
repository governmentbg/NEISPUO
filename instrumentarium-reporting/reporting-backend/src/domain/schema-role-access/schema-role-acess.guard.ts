import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import {
  CondOperator,
  RequestQueryBuilder,
  RequestQueryParser,
} from '@nestjsx/crud-request';
import { HttpMethodEnum } from '../../shared/enums/http-method.enum';
import { ReportsScopes } from '../../shared/enums/reports-scopes.enum';
import { SysRoleEnum } from '../../shared/enums/role.enum';
import {
  AuthedRequest,
  AuthObject,
} from '../../shared/interfaces/authed-request.interface';

@Injectable()
export class SchemaRoleAccessGuard implements CanActivate {
  private allowedRoles = [
    SysRoleEnum.MON_ADMIN,
    SysRoleEnum.MON_EXPERT,
    SysRoleEnum.MON_CHRAO,
    SysRoleEnum.MON_OBGUM,
    SysRoleEnum.MON_OBGUM_FINANCES,
    SysRoleEnum.RUO,
    SysRoleEnum.RUO_EXPERT,
    SysRoleEnum.MUNICIPALITY,
    SysRoleEnum.INSTITUTION,
  ];

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
    let accessGranted: boolean = false;

    if (
      !authObject ||
      !this.allowedRoles.includes(authObject.selectedRole.SysRoleID)
    ) {
      return false;
    }

    if (reqMethod === HttpMethodEnum.GET) {
      accessGranted = this.grantReadAccess(authObject, req);
    } else if (
      reqMethod === HttpMethodEnum.POST ||
      reqMethod === HttpMethodEnum.PATCH ||
      reqMethod === HttpMethodEnum.PUT ||
      reqMethod === HttpMethodEnum.DELETE
    ) {
      accessGranted = this.grantCreateUpdateDeleteAccess(authObject, req);
    }

    return accessGranted;
  }

  private grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
    const { isMon, isRuo, isMunicipality, isSchool } = authObject;

    if (isMon || isRuo || isMunicipality || isSchool) {
      this.scopeReadAccess(authObject, req);
      return true;
    }
    return false;
  }

  private scopeReadAccess(authObject: AuthObject, req: AuthedRequest) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    /** Allow only where creator of the report tries to delete it */
    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          'AllowedSysRole.SysRoleID': authObject.selectedRole.SysRoleID,
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }

  private grantCreateUpdateDeleteAccess(
    authObject: AuthObject,
    req: AuthedRequest,
  ) {
    const { isMonAdmin } = authObject;
    return isMonAdmin;
  }
}
