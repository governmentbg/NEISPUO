import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { HttpMethodEnum } from 'src/shared/enums/http-method.enum';
import { SysRoleEnum } from 'src/shared/enums/role.enum';
import {
  AuthedRequest,
  AuthObject,
} from 'src/shared/interfaces/authed-request.interface';

@Injectable()
export class ReportGuard implements CanActivate {
  private allowedRoles = [
    SysRoleEnum.MON_ADMIN,
    SysRoleEnum.MON_EXPERT,
    SysRoleEnum.MON_CHRAO,
    SysRoleEnum.MON_OBGUM,
    SysRoleEnum.MON_OBGUM_FINANCES,
    SysRoleEnum.CIOO,
    SysRoleEnum.RUO,
    SysRoleEnum.RUO_EXPERT,
    SysRoleEnum.BUDGETING_INSTITUTION,
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
    if (
      !authObject ||
      !this.allowedRoles.includes(authObject.selectedRole.SysRoleID)
    ) {
      return false;
    }

    return this.grantPersonalReportAccess(authObject, req);
  }

  private async grantPersonalReportAccess(
    authObject: AuthObject,
    req: AuthedRequest,
  ) {
    this.scopeAccess(authObject, req);

    return true;
  }

  private scopeAccess(authObject: AuthObject, req: AuthedRequest) {
    const parsed = RequestQueryParser.create()
      .parseQuery(req.query)
      .getParsed();

    /** Allow only where creator of the report tries to delete it */
    const scoped = RequestQueryBuilder.create().search({
      $and: [
        {
          'CreatedBy.SysUserID': authObject.selectedRole.SysUserID,
        },
        parsed.search,
      ],
    }).queryObject;

    req.query.s = scoped.s;
  }
}
