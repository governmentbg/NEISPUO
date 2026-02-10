import { Component, OnInit } from '@angular/core';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { OIDCService } from '@authentication/auth-state-manager/oidc.service';
import { PrimeNGConfig } from 'primeng/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Портал НЕИСПУО';
  /**
   *
   */
  constructor(private oidcService: OIDCService, private authQuery: AuthQuery, private primengConfig: PrimeNGConfig) {}
  ngOnInit(): void {
    this.oidcService.start();
    this.primengConfig.ripple = true;
  }


}
