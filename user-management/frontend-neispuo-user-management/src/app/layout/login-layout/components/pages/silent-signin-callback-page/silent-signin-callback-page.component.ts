import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OIDCService } from '@core/services/oidc.service';

@Component({
    selector: 'app-silent-signin-callback',
    templateUrl: './silent-signin-callback-page.component.html',
    styleUrls: ['./silent-signin-callback-page.component.scss'],
})
export class SilentSigninCallbackPageComponent implements OnInit {
    error: any;

    constructor(private oidcService: OIDCService, private router: Router) {}

    async ngOnInit() {
        try {
            await this.oidcService.userManager.signinSilentCallback();
            this.router.navigateByUrl('/user-management');
        } catch (e) {
            console.log('signinSilentCallback failed', e);
            this.error = e;
        }
    }
}
