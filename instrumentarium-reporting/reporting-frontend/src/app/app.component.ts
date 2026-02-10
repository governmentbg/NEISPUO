import { Component } from '@angular/core';
import { OIDCService } from '@authentication/auth-state-manager/oidc.service';
import { PrimengConfigService } from '@shared/services/primeng-config.service';
import { PrimeNGConfig } from 'primeng/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'reporting-frontend';

  constructor(
    private primengConfig: PrimeNGConfig,
    private primengConfigService: PrimengConfigService,
    private oidcService: OIDCService
  ) {}

  ngOnInit() {
    this.primengConfig.ripple = true;
    this.primengConfigService.filterMatchModeConfig();
    this.primengConfigService.primengTranslateConfig();
    this.primengConfigService.customFiltersConfig();
    this.oidcService.start();
  }
}
