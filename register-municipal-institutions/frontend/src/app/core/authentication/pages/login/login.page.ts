import { Component, OnInit } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { AuthQuery } from 'src/app/core/authentication/auth-state-manager/auth.query';
import { OIDCService } from 'src/app/core/authentication/auth-state-manager/oidc.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
})

export class LoginPage implements OnInit {
  public environment = this.envService.environment;

  constructor(private oidcService: OIDCService, private authQuery: AuthQuery, private envService: EnvironmentService) { }

  async login(): Promise<void> {
    await this.oidcService.userManager.signinRedirect();
  }

  ngOnInit(): void { }
}
