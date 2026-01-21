import { Injectable } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { OIDCService } from "../services/oidc.service";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  constructor(private oidcService: OIDCService, private helper: JwtHelperService) {}

  private getUserName(): string {
    const profile = this.getProfile();
    return profile ? profile.FirstName + " " + profile.LastName : "";
  }

  private getEmail(): string {
    const profile = this.getProfile();
    return profile ? profile.selected_role.Username : "";
  }

  private getProfile() {
    const tokenName =
      "oidc.user:" + this.oidcService.userManager.settings.authority + ":" + this.oidcService.userManager.settings.client_id;

    const oidcToken = sessionStorage.getItem(tokenName) || localStorage.getItem(tokenName);
    return oidcToken ? JSON.parse(oidcToken).profile : null;
  }

  getToken(): string {
    return this.helper.tokenGetter();
  }

  getRole() {
    const token = this.getToken();
    return token ? this.helper.decodeToken(token).selected_role.SysRoleID : null;
  }

  getRegion() {
    const token = this.getToken();
    return token ? this.helper.decodeToken(token).selected_role.RegionID + "" : null;
  }

  getSysUserId() {
    const token = this.getToken();
    return token ? this.helper.decodeToken(token).selected_role.SysUserID : null;
  }

  isRuo() {
    return [2, 3, 9].includes(this.getRole());
  }

  isMon() {
    return [1, 10, 12, 13, 15, 16, 17, 19].includes(this.getRole());
  }

  isExpert() {
    return [9, 12, 16, 17, 19].includes(this.getRole());
  }

  hasNoRights() {
    return [0, 4, 5, 6, 7, 8, 11, 14, 20].includes(this.getRole());
  }

  getUserData() {
    return { name: this.getUserName(), email: this.getEmail() };
  }

  isLoggedIn() {
    return !!this.getToken();
  }

  async signout() {
    await this.oidcService.userManager.signoutRedirect();
  }

  signin() {
    this.oidcService.userManager.signinRedirect();
  }
}
