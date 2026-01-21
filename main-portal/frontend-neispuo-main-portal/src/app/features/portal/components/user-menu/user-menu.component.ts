import { Component, OnInit } from '@angular/core';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { OIDCService } from '@authentication/auth-state-manager/oidc.service';

@Component({
  selector: 'app-user-menu',
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.scss']
})
export class UserMenuComponent implements OnInit {
  public email$ = this.authQuery.email$;
  public fullName$ = this.authQuery.fullName$;
  public abbreviatedName$ = this.authQuery.abbreviatedName$;

  constructor(private authQuery: AuthQuery, private oidcService: OIDCService) {}

  ngOnInit(): void {}

  public async logout() {
    await this.oidcService.userManager.signoutRedirect();
  }
}
