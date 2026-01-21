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

  getSessionId(): string {
    const tokenName = `oidc.user:${environment.oidcBaseUrl}:inst_basic`;
    const jsonToken = JSON.parse(localStorage.getItem(tokenName)) || JSON.parse(sessionStorage.getItem(tokenName));
    return jsonToken ? this.helper.decodeToken(jsonToken.access_token).sessionID : null;
  }

  getType(): string {
    const type = sessionStorage.getItem("type");
    return type ? atob(type) : type;
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

  isRuo() {
    return [2, 3, 4, 9].includes(this.getRole());
  }

  isMon() {
    return [1, 10, 11, 12, 13, 15, 16, 17, 19].includes(this.getRole());
  }

  hasNoRights() {
    return [5, 6, 7, 8].includes(this.getRole());
  }

  isHeadmaster() {
    return [0, 14, 20].includes(this.getRole());
  }

  getInstId() {
    const token = this.getToken();
    return token ? this.helper.decodeToken(token).selected_role.InstitutionID : null;
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

    const lastEntry = url?.[url.length - 1];
    return lastEntry ? [lastEntry.prevUrl, lastEntry.prevUrlParams] : null;
  }

  setPrevUrlData(prevUrl, prevUrlParams) {
    const nextUrl = { prevUrl, prevUrlParams };
    let prev = sessionStorage.getItem("url");
    let arr: any[] = [];

    if (prev) {
      arr = JSON.parse(prev);
    }

    arr.push(nextUrl);
    sessionStorage.setItem("url", JSON.stringify(arr));
  }

  removeUrl() {
    const urlHistory = sessionStorage.getItem("url");
    if (urlHistory) {
      const urlArr = JSON.parse(urlHistory);

      if (urlArr.length > 1) {
        urlArr.pop();
        sessionStorage.setItem("url", JSON.stringify(urlArr));
      } else {
        sessionStorage.removeItem("url");
        sessionStorage.removeItem("regixData");
      }
    }
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

  setRegixData(regixData) {
    sessionStorage.setItem("regixData", regixData);
  }

  getRegixData() {
    return sessionStorage.getItem("regixData");
  }

  removeRegixData() {
    sessionStorage.removeItem("regixData");
  }

  async signout() {
    sessionStorage.removeItem("type");
    sessionStorage.removeItem("list");
    sessionStorage.removeItem("paramsParent");
    sessionStorage.removeItem("url");
    sessionStorage.removeItem("tableName");
    sessionStorage.removeItem("regixData");
    await this.oidcService.userManager.signoutRedirect();
  }

  signin() {
    this.oidcService.userManager.signinRedirect();
  }
}
