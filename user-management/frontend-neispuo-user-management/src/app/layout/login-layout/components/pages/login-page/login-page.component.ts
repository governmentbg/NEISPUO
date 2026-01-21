import { Component } from '@angular/core';
import { OIDCService } from '@core/services/oidc.service';
import { EnvironmentService } from '@core/services/environment.service';

@Component({
    selector: 'app-login-page',
    templateUrl: './login-page.component.html',
    styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent {
    public environment = this.envService.environment;

    constructor(private oidcService: OIDCService, private envService: EnvironmentService) {
        this.login();
    }

    async login(): Promise<void> {
        await this.oidcService.userManager.signinRedirect();
    }
}
