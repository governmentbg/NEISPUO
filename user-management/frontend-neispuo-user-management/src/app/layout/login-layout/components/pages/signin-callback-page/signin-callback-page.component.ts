import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OIDCService } from '@core/services/oidc.service';

@Component({
    selector: 'app-signin-callback',
    templateUrl: './signin-callback-page.component.html',
    styleUrls: ['./signin-callback-page.component.scss'],
})
export class SignInCallbackPageComponent implements OnInit {
    error: any;

    constructor(private oidcService: OIDCService, private router: Router) {}

    ngOnInit(): void {
        this.handleResponse();
    }

    private async handleResponse() {
        try {
            await this.oidcService.userManager.signinRedirectCallback();
            this.router.navigateByUrl('/user-management');
        } catch (e) {
            console.log(e);
            this.error = e;
        }
    }
}
