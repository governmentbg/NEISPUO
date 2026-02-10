import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { HttpMethodEnum } from '../../shared/enums/http-method.enum';
import { SysRoleEnum } from '../../shared/enums/role.enum';
import {
  AuthedRequest,
  AuthObject,
} from '../../shared/interfaces/authed-request.interface';

@Injectable()
export class SchemaDefinitionGuard implements CanActivate {
  private allowedRoles = [SysRoleEnum.MON_ADMIN];

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
    if (authObject.hasApiKey || authObject.isMonAdmin) {
      return true;
    }

    return false;
  }
}
