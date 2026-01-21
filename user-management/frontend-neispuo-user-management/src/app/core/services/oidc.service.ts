import { Injectable } from '@angular/core';
import { AuthService } from '@core/authentication/auth.service';
import * as Oidc from 'oidc-client';
import { UserManager } from 'oidc-client';
import { EnvironmentService } from '@core/services/environment.service';

@Injectable({
    providedIn: 'root',
})
export class OIDCService {
    public readonly environment;

    public userManager: UserManager;

    constructor(private authService: AuthService, private envService: EnvironmentService) {
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
            checkSessionInterval: 60000,
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
        });
        Promise.all([this.userManager.removeUser(), this.userManager.signinSilent()])
            .catch((e) => console.log(e))

            .finally(() => {
                this.userManager.events.addUserUnloaded(() => this.authService.removeAuthUser());
                this.userManager.events.addUserSignedOut(() => this.userManager.removeUser());
                this.authService.updateAuthUser({ authReady: true });
            });
    }
}
