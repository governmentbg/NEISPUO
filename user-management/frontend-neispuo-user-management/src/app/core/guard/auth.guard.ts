import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AuthQuery } from '@core/authentication/auth.query';
import { filter, map, switchMap, take } from 'rxjs/operators';

@Injectable({
    providedIn: 'root',
})
export class AuthGuard implements CanActivate {
    reroute = '/login';

    constructor(private authQuery: AuthQuery, private router: Router) {}

    private isAuthenticated() {
        return this.authQuery.select('authReady').pipe(
            filter((resp) => !!resp),
            take(1),
            switchMap(() => this.authQuery.isLoggedIn$),
            map((isLoggedIn) => isLoggedIn || (this.router.navigate([this.reroute]) && false)),
        );
    }

    canActivate() {
        return this.isAuthenticated();
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.isAuthenticated();
    }
}
