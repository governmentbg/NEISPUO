import { Injectable, CanActivate, ExecutionContext, Inject } from '@nestjs/common';
import { RequestQueryBuilder, RequestQueryParser } from '@nestjsx/crud-request';
import { CondOperator } from '@nestjsx/crud-request';

@Injectable()
export class RoleGuard implements CanActivate {

    canActivate(ctx: ExecutionContext) {
        const request = ctx.switchToHttp().getRequest();
        const scoped = RequestQueryBuilder.create()
            .search(
                {

                    'neispuoModules.roles.id': request.user.selected_role.SysRoleID
                }
            ).queryObject;

        request.query.s = scoped.s;
        return true;
    }
}