import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { filter, map, take } from 'rxjs/operators';
import { CONSTANTS } from 'src/app/shared/constants';
import { RoleEnum } from 'src/app/shared/enums/roles.enum';
import { AuthQuery } from '../authentication/auth.query';

@Injectable({
    providedIn: 'root',
})
export class HomeRedirectGuard implements CanActivate {
    institutionRoute = `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_INSTITUTIONS}`;

    teacherClassesRoute = `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_TEACHER_CLASSES}`;

    constructor(private authQuery: AuthQuery, private router: Router) {}

    private redirectRole(route: ActivatedRouteSnapshot) {
        return this.authQuery.selectedRole$.pipe(
            filter((resp) => !!resp),
            take(1),
            map(
                (role) =>
                    (role?.SysRoleID === RoleEnum.TEACHER &&
                        role?.IsLeadTeacher &&
                        this.router.navigate([this.teacherClassesRoute])) ||
                    (role?.SysRoleID === RoleEnum.MON_ADMIN && this.router.navigate([this.institutionRoute])) ||
                    (role?.SysRoleID === RoleEnum.RUO && this.router.navigate([this.institutionRoute])) ||
                    (role?.SysRoleID === RoleEnum.INSTITUTION && this.router.navigate([this.institutionRoute])) ||
                    true,
            ),
        );
    }

    canActivate(route: ActivatedRouteSnapshot): any {
        return this.redirectRole(route);
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.redirectRole(route);
    }
}
