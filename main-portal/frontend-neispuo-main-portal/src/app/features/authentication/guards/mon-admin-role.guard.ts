import { Injectable } from '@angular/core';
import { CanActivate, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { filter, map, switchMap, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MonAdminRoleGuard implements CanActivate {
  reroute = '/portal';

  constructor(
    private authQuery: AuthQuery,
    private router: Router
  ) {}

  private isMonRole() {
    return this.authQuery.select('authReady').pipe(
      filter((resp) => !!resp),
      take(1),
      switchMap(() => this.authQuery.isMon$),
      map((isMon) => isMon || (this.router.navigate([this.reroute]) && false))
    );
  }

  canActivate() {
    return this.isMonRole();
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.isMonRole();
  }
}
