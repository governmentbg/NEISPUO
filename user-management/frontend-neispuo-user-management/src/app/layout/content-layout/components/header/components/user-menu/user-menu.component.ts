import { Component } from '@angular/core';
import { AuthQuery } from 'src/app/core/authentication/auth.query';
import { OIDCService } from '@core/services/oidc.service';
import { CONSTANTS } from '@shared/constants';
import { ImpersonationService } from '@core/impersonation/impersonation.service';

@Component({
    selector: 'app-user-menu',
    templateUrl: './user-menu.component.html',
    styleUrls: ['./user-menu.component.scss'],
})
export class UserMenuComponent {
    public CONSTANTS = CONSTANTS;

    public email$ = this.authQuery.email$;

    public fullName$ = this.authQuery.fullName$;

    public abbreviatedName$ = this.authQuery.abbreviatedName$;

    public isImpersonator$ = this.authQuery.isImpersonator$;

    constructor(
        private authQuery: AuthQuery,
        private oidcService: OIDCService,
        private impersonationService: ImpersonationService,
    ) {}

    async logout() {
        await this.oidcService.userManager.signoutRedirect();
    }

    public async endImpersonation() {
        this.impersonationService.endImpersonate().subscribe(() => {
            window.location.reload();
        });
    }
}
