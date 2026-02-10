import { CanActivate, ExecutionContext, Logger, mixin } from '@nestjs/common';

export const RoleGuard = (roles: number[]) => {
  class RoleGuardMixin implements CanActivate {
    constructor() {}

    async canActivate(ctx: ExecutionContext) {
      try {
        const request: any = ctx.switchToHttp().getRequest();

        if (roles.some(r => r === request.user?.selected_role?.SysRoleID)) {
          return true;
        }
      } catch (err) {
        Logger.error(`Error in RoleGuard`, err);
      }
      return false;
    }
  }
  const guard = mixin(RoleGuardMixin);
  return guard;
};
