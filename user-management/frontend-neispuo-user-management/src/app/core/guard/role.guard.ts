import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { filter, map, take } from 'rxjs/operators';
import { CONSTANTS } from 'src/app/shared/constants';
import { RoleEnum } from 'src/app/shared/enums/roles.enum';
import { AuthQuery } from '../authentication/auth.query';

@Injectable({
    providedIn: 'root',
})
export class RoleGuard implements CanActivate {
    /* this guard is designed to stop further routing if your role is not institution and reroute you to the reroute url below */

    reroute = `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_UNAUTHORIZED}`;

    constructor(private authQuery: AuthQuery, private router: Router) {}

    private isAuthRole(route: ActivatedRouteSnapshot) {
        const roles = route.data.roles as Array<RoleEnum>;

        return this.authQuery.selectedRole$.pipe(
            filter((resp) => !!resp),
            take(1),
            map(
                (role) =>
                    roles.includes(role?.SysRoleID as RoleEnum) || (this.router.navigate([this.reroute]) && false),
            ),
        );
    }

    canActivate(route: ActivatedRouteSnapshot) {
        return this.isAuthRole(route);
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.isAuthRole(route);
    }
}
