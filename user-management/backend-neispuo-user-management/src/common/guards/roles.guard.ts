import { CanActivate, ExecutionContext, mixin } from '@nestjs/common';

export const RoleGuard = (roles: number[]) => {
    class RoleGuardMixin implements CanActivate {
        constructor() {}

        async canActivate(ctx: ExecutionContext) {
            try {
                const request: any = ctx.switchToHttp().getRequest();

                if (roles.some((r) => r === request._authObject.selectedRole.SysRoleID)) {
                    return true;
                }
            } catch (err) {}
            return false;
        }
    }
    const guard = mixin(RoleGuardMixin);
    return guard;
};
