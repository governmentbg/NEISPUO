import { Component, OnInit } from '@angular/core';
import { AuthQuery } from 'src/app/core/authentication/auth-state-manager/auth.query';
import { OIDCService } from 'src/app/core/authentication/auth-state-manager/oidc.service';
import { EnvironmentService } from 'src/app/core/services/environment.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss']
})
export class LoginPage implements OnInit {
  public environment = this.envService.environment;

  constructor(private oidcService: OIDCService, private authQuery: AuthQuery, private envService: EnvironmentService) {
    this.login();
  }

  async login(): Promise<void> {
    await this.oidcService.userManager.signinRedirect();
  }

  ngOnInit(): void {}
}
