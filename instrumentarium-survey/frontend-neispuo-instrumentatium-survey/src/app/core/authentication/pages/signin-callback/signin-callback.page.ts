import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ROUTING_CONSTANTS } from '@shared/constants/routing.constants';
import { OIDCService } from 'src/app/core/authentication/auth-state-manager/oidc.service';

@Component({
  selector: 'app-signin-callback',
  templateUrl: './signin-callback.page.html',
  styleUrls: ['./signin-callback.page.scss'],
})
export class SignInCallbackPage implements OnInit {
  error = null;

  constructor(private oidcService: OIDCService, private router: Router) { }

  ngOnInit(): void {
    this.handleResponse();
  }

  private async handleResponse() {
    let response;
    try {
      response = await this.oidcService.userManager.signinRedirectCallback();
      await this.router.navigateByUrl(`/${ROUTING_CONSTANTS.SURVEY}`);
    } catch (e) {
      this.error = e;
    }
  }

}
