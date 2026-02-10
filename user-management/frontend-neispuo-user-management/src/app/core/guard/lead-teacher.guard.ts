import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { filter, map, take } from 'rxjs/operators';
import { CONSTANTS } from 'src/app/shared/constants';
import { RoleEnum } from 'src/app/shared/enums/roles.enum';
import { AuthQuery } from '../authentication/auth.query';

@Injectable({
    providedIn: 'root',
})
export class LeadTeacherGuard implements CanActivate {
    reroute = `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_UNAUTHORIZED}`;

    constructor(private authQuery: AuthQuery, private router: Router) {}

    private isAuthRole(route: ActivatedRouteSnapshot) {
        return this.authQuery.selectedRole$.pipe(
            filter((resp) => !!resp),
            take(1),
            map(
                (role) =>
                    (role?.SysRoleID === RoleEnum.TEACHER && role?.IsLeadTeacher) ||
                    (this.router.navigate([this.reroute]) && false),
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
