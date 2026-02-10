import {
 Injectable, CanActivate, ExecutionContext,
} from '@nestjs/common';
import { RequestQueryBuilder } from '@nestjsx/crud-request';
/** Example role guard */

@Injectable()
export class RoleGuard implements CanActivate {
    canActivate(ctx: ExecutionContext) {
        const request = ctx.switchToHttp().getRequest();
        const scoped = RequestQueryBuilder.create().search({
            'neispuoModules.roles.id': request.user.selected_role.SysRoleID,
        }).queryObject;

        request.query.s = scoped.s;
        return true;
    }
}
