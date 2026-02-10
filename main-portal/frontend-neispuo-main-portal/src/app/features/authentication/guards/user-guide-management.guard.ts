import { Injectable } from '@angular/core';
import { CanActivate, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { filter, map, switchMap, take } from 'rxjs/operators';
import { combineLatest } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserGuideManagementGuard implements CanActivate {
  reroute = '/portal';

  constructor(
    private authQuery: AuthQuery,
    private router: Router
  ) {}

  private hasAccess() {
    return this.authQuery.select('authReady').pipe(
      filter((resp) => !!resp),
      take(1),
      switchMap(() => combineLatest([this.authQuery.isMon$, this.authQuery.isHelpdesk$])),
      map(([isMon, isHelpdesk]) => isMon || isHelpdesk || (this.router.navigate([this.reroute]) && false))
    );
  }

  canActivate() {
    return this.hasAccess();
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.hasAccess();
  }
}
