import { Injectable } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { environment } from "src/environments/environment";
import { OIDCService } from "../services/oidc.service";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  constructor(private oidcService: OIDCService, private helper: JwtHelperService) {}

  private getUserName(): string {
    const tokenName =
      "oidc.user:" +
      this.oidcService.userManager.settings.authority +
      ":" +
      this.oidcService.userManager.settings.client_id;

    const oidcToken = sessionStorage.getItem(tokenName) || localStorage.getItem(tokenName);
    const profile = oidcToken ? JSON.parse(oidcToken).profile : null;
    const name = profile ? profile.FirstName + " " + profile.LastName : "";
    return name;
  }

  private getEmail(): string {
    const profile = this.oidcService.authState ? this.oidcService.authState.profile : null;
    return profile ? profile.selected_role.Username : "";
  }

  getToken(): string {
    return this.helper.tokenGetter();
  }

  getSessionId(): string {
    const tokenName = `oidc.user:${environment.oidcBaseUrl}:inst_basic`;
    const jsonToken = JSON.parse(localStorage.getItem(tokenName)) || JSON.parse(sessionStorage.getItem(tokenName));
    return jsonToken ? this.helper.decodeToken(jsonToken.access_token).sessionID : null;
  }

  private getRole() {
    const token = this.getToken();
    return token ? this.helper.decodeToken(token).selected_role.SysRoleID : null;
  }

  getSysUserId() {
    const token = this.getToken();
    return token ? this.helper.decodeToken(token).selected_role.SysUserID : null;
  }

  getSysRoleId() {
    const token = this.getToken();
    return token ? this.helper.decodeToken(token).selected_role.SysRoleID : null;
  }

  isAuthorized() {
    return [1, 10].includes(this.getRole());
  }

  getUserData() {
    return { name: this.getUserName(), email: this.getEmail() };
  }

  isLoggedIn() {
    return !!this.getToken();
  }

  getPrevUrlData() {
    let url: any = sessionStorage.getItem("url");
    url && (url = JSON.parse(url));
    return url ? [url.prevUrl, url.prevUrlParams] : null;
  }

  setPrevUrlData(prevUrl, prevUrlParams) {
    const url = { prevUrl, prevUrlParams };
    sessionStorage.setItem("url", JSON.stringify(url));
  }

  removeUrl() {
    sessionStorage.removeItem("url");
  }

  getTableName(): string {
    return sessionStorage.getItem("tableName");
  }

  setTableName(accordionTableName: string) {
    sessionStorage.setItem("tableName", accordionTableName);
  }

  removeTableName() {
    sessionStorage.removeItem("tableName");
  }

  async signout() {
    sessionStorage.removeItem("type");
    sessionStorage.removeItem("list");
    sessionStorage.removeItem("paramsParent");
    sessionStorage.removeItem("url");
    sessionStorage.removeItem("tableName");
    await this.oidcService.userManager.signoutRedirect();
  }

  signin() {
    this.oidcService.userManager.signinRedirect();
  }
}
