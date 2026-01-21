import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { EnvironmentService } from '@core/services/environment.service';
import { NomenclatureService } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.service';
import * as Oidc from 'oidc-client';
import { UserManager } from 'oidc-client';
import { from, Observable, of } from 'rxjs';
import { catchError, map, take } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class OIDCService {
  public readonly environment;

  public userManager: UserManager;

  /**
   *
   */
  constructor(
    private authService: AuthService,
    private nmService: NomenclatureService,
    private router: Router,
    private envService: EnvironmentService,
  ) {
    this.environment = this.envService.environment;
    if (!this.environment.production) {
      Oidc.Log.logger = console;
      Oidc.Log.level = Oidc.Log.DEBUG;
    }

    this.userManager = new UserManager({
      authority: this.environment.OIDC_BASE_URL,
      client_id: this.environment.OIDC_CLIENT_ID,
      redirect_uri: `${this.environment.APP_URL}/signin-callback`,
      silent_redirect_uri: `${this.environment.APP_URL}/silent-signin-callback`,
      post_logout_redirect_uri: `${this.environment.APP_URL}/login`,
      response_type: 'code',
      scope: 'openid offline_access',
      monitorSession: true,
      checkSessionInterval: 10000,
      stopCheckSessionOnError: false,
      automaticSilentRenew: true,
    });
  }

  start() {
    this.userManager.events.addSilentRenewError(() => {
      this.authService.updateAuthUser({ authReady: true });
    });
    this.userManager.events.addUserLoaded((user) => {
      this.authService.setAuthUser({
        ...user.profile,
        jwt: user.id_token,
        authReady: true,
        oidcAccessToken: user.access_token,
      });
      this.nmService.loadAll().subscribe(() => {
        console.log('loaded all');
      });
    });
    Promise.all([this.userManager.removeUser(), this.userManager.signinSilent()])
      .catch((e) => console.log(e))

      .finally(() => {
        this.userManager.events.addUserUnloaded(() => this.authService.removeAuthUser());
        this.userManager.events.addUserSignedOut(() => this.userManager.removeUser());
        this.authService.updateAuthUser({ authReady: true });
      });
  }

  getJwt(): Observable<{ accessToken: string; idToken: string }> {
    return from(this.userManager.getUser()).pipe(
      take(1),
      map((u) => ({ accessToken: u.access_token, idToken: u.id_token })),
      catchError((e) => of(null)),
    );
  }
}
