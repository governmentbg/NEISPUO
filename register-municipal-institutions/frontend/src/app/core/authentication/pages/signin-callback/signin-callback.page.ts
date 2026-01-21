import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
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
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      response = await this.oidcService.userManager.signinRedirectCallback();
      await this.router.navigateByUrl('/municipal-institutions');
    } catch (e) {
      this.error = e;
    }
  }
}
