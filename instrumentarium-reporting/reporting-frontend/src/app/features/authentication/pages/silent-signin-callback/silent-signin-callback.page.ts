import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OIDCService } from '@authentication/auth-state-manager/oidc.service';

@Component({
  selector: 'app-silent-signin-callback',
  templateUrl: './silent-signin-callback.page.html',
  styleUrls: ['./silent-signin-callback.page.scss']
})
export class SilentSigninCallbackPage implements OnInit {
  error: any;
  constructor(private oidcService: OIDCService, private router: Router) {}

  async ngOnInit() {
    let response;
    try {
      response = await this.oidcService.userManager.signinSilentCallback();
      await this.router.navigateByUrl('/');
    } catch (e) {
      console.log('signinSilentCallback failed', e);
      this.error = e;
    }
  }
}
