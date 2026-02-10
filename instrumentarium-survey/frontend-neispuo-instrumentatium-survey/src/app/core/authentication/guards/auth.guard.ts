import { Injectable } from '@angular/core';
import { CanActivate, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthQuery } from 'src/app/core/authentication/auth-state-manager/auth.query';
import { filter, map, switchMap, take } from 'rxjs/operators';
import { ROUTING_CONSTANTS } from '@shared/constants/routing.constants';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  reroute = `/${ROUTING_CONSTANTS.LOGIN}`;

  constructor(private authQuery: AuthQuery, private router: Router) { }

  private isAuthenticated() {
    return this.authQuery.select('authReady')
      .pipe(
        filter((resp) => !!resp),
        take(1),
        switchMap(
          () => this.authQuery.isLoggedIn$
        ),
        map(isLoggedIn => isLoggedIn || this.router.navigate([this.reroute]) && false)
      );
  }

  canActivate() {
    return this.isAuthenticated();
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.isAuthenticated();
  }
}
