import { Injectable } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { Mode } from "../enums/mode.enum";
import { Role, RoleInt } from "../enums/role.enum";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  constructor(private helper: JwtHelperService) {}

  getToken(): string {
    return this.helper.tokenGetter();
  }

  getMode() {
    const decoded = this.helper.decodeToken(this.helper.tokenGetter());
    const role = decoded.role;
    const mode = RoleInt[role - 1] === Role.Mon ? Mode.Edit : Mode.View;
    return mode;
  }

  getRole() {
    const decoded = this.helper.decodeToken(this.helper.tokenGetter());
    return decoded ? decoded.baseschooltype : null;
  }

  getInstId() {
    const decoded = this.helper.decodeToken(this.helper.tokenGetter());
    return decoded ? decoded.instid : null;
  }

  getFromStorage(formName: string) {
    return JSON.parse(sessionStorage.getItem(formName));
  }

  setToken(token: string) {
    sessionStorage.setItem("token", token);
  }

  setToStorage(formName: string, formValue: Object) {
    sessionStorage.setItem(formName, JSON.stringify(formValue));
  }

  removeFromStorage(itemName: string) {
    sessionStorage.removeItem(itemName);
    localStorage.removeItem(itemName);
  }

  isLoggedIn() {
    const token = this.helper.tokenGetter();
    return token ? true : false;
  }

  clearStorage() {
    sessionStorage.clear();
    localStorage.clear();
  }
}
