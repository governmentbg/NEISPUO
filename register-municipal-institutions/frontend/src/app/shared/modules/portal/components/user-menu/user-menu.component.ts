import { Component, OnInit } from '@angular/core';
import { AuthQuery } from 'src/app/core/authentication/auth-state-manager/auth.query';
import { OIDCService } from 'src/app/core/authentication/auth-state-manager/oidc.service';

@Component({
  selector: 'app-user-menu',
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.scss'],
})
export class UserMenuComponent implements OnInit {
  public email$ = this.authQuery.email$;

  public FirstName$ = this.authQuery.FirstName$;

  public LastName$ = this.authQuery.LastName$;

  public abbreviatedName$ = this.authQuery.abbreviatedName$;

  constructor(private authQuery: AuthQuery, private oidcService: OIDCService) { }

  ngOnInit(): void {
 }

  public async logout() {
    await this.oidcService.userManager.signoutRedirect();
  }

  public goToProfile() {
    window.alert('Not Implemented.');
  }
}
