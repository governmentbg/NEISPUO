import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthQuery } from '@core/authentication/auth.query';
import { filter, map, switchMap, take } from 'rxjs/operators';

/** Guards routes that should not be available when user is authenticated. Eg. login/register */
@Injectable({
    providedIn: 'root',
})
export class NoAuthGuard implements CanActivate {
    reroute = '/user-management';

    constructor(private authQuery: AuthQuery, private router: Router) {}

    private isNotAuthenticated() {
        return this.authQuery.select('authReady').pipe(
            filter((resp) => !!resp),
            take(1),
            switchMap(() => this.authQuery.isLoggedIn$.pipe(take(1))),
            map((isLoggedIn) => !isLoggedIn || (this.router.navigate([this.reroute]) && false)),
        );
    }

    canActivate() {
        return this.isNotAuthenticated();
    }
}
