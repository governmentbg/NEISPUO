import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { RoleEnum } from '@authentication/models/role.enum';
import { ROUTING_CONSTANTS } from '@shared/constants/routing.constants';
import { filter, map, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  /*this guard is designed to stop further routing if your role is not present in the route.data object (passed through the routing module)
   and reroute you to the reroute url below*/
  reroute = `/${ROUTING_CONSTANTS.LOGIN}`;

  constructor(private authQuery: AuthQuery, private router: Router) {}

  private isAuthRole(route: ActivatedRouteSnapshot) {
    let roles = route.data.roles as Array<RoleEnum>;

    return this.authQuery.selectedRole$.pipe(
      filter((resp) => !!resp),
      take(1),
      map((role) => roles.includes(role?.SysRoleID as RoleEnum) || (this.router.navigate([this.reroute]) && false))
    );
  }

  canActivate(route: ActivatedRouteSnapshot) {
    return this.isAuthRole(route);
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.isAuthRole(route);
  }
}
