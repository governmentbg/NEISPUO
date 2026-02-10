import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivate, Router } from "@angular/router";
import { FormWrapperComponent } from "../pages/form-wrapper/form-wrapper.component";
import { AuthService } from "./auth.service";
import { Menu } from "../enums/menu.enum";
import { FormTypeInt } from "../enums/formType.enum";
import { environment } from "../../environments/environment";
import { HelperService } from "../services/helpers.service";
import { Tabs } from "../enums/tabs.enum";

@Injectable({
  providedIn: "root"
})
export class FormGuard implements CanDeactivate<any> {
  constructor() {}

  canDeactivate(
    component: FormWrapperComponent,
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean {
    if (component && component.form && component.form.formGroup && component.form.formGroup.dirty) {
      return component.navigateAway();
    } else {
      return true;
    }
  }
}

@Injectable({ providedIn: "root" })
export class AuthGuardLogged implements CanActivate {
  constructor(private authService: AuthService, private router: Router, private helperService: HelperService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const isLoggedIn = this.authService.isLoggedIn();
    const isMonRuo = this.authService.isMon() || this.authService.isRuo();
    const type = route.params.type;
    const actualType = this.authService.getType();
    const homePath = isMonRuo
      ? `${Menu.Home}/${FormTypeInt[0]}`
      : `/${Menu.Home}/${actualType}/${Tabs.main}/institution/institution`;
    const url = route.url && route.url.length > 0 ? route.url[0].path : null;
    const actualInstid = this.authService.getInstId();
    const actualSysUserId = this.authService.getSysUserId();
    const actualSysRoleId = this.authService.getSysRoleId();
    const currentQueryParams = environment.production
      ? this.helperService.decodeParams(route.queryParams.q)
      : route.queryParams;
    const sysuserid = currentQueryParams.sysuserid;
    const sysroleid = currentQueryParams.sysroleid;
    const instid = currentQueryParams.instid;
    const editableByMonRuo = currentQueryParams.editableByMonRuo;

    if (!isLoggedIn) {
      this.authService.signin();
      return false;
    } else if (this.authService.hasNoRights() && url !== environment.unauthorizedRedirectUrl) {
      this.router.navigate([environment.unauthorizedRedirectUrl]);
      return false;
    } else if (
      !isMonRuo &&
      ((type != actualType && actualType && url === Menu.Home) ||
        (instid && instid != actualInstid) ||
        (sysuserid && sysuserid != actualSysUserId) ||
        (sysroleid && sysroleid != actualSysRoleId))
    ) {
      let queryParams: any = { instid: actualInstid, sysuserid: actualSysUserId, sysroleid: actualSysRoleId };
      environment.production && (queryParams = this.helperService.encodeParams(queryParams));
      this.router.navigate([homePath], { queryParams });
      return false;
    } else if (
      isMonRuo &&
      (url === Menu.Create ||
        (url === Menu.Edit && !editableByMonRuo) ||
        url === Menu.NewStaffMember ||
        (sysuserid && sysuserid != actualSysUserId) ||
        (sysroleid && sysroleid != actualSysRoleId))
    ) {
      let queryParams: any = { sysuserid: actualSysUserId, sysroleid: actualSysRoleId };
      environment.production && (queryParams = this.helperService.encodeParams(queryParams));
      this.router.navigate([homePath], { queryParams });
      return false;
    } else if (this.authService.isHeadmaster() && !actualType && url !== Menu.Home) {
      this.router.navigate(["/", Menu.Home]);
      return false;
    }

    return true;
  }
}

@Injectable({ providedIn: "root" })
export class AuthGuardAdmin implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const isLoggedIn = this.authService.isLoggedIn();
    const isMon = this.authService.isMon();

    if (!isLoggedIn) {
      this.authService.signin();
      return false;
    } else if (!isMon) {
      this.router.navigate(["/", Menu.Home]);
      return false;
    }

    return true;
  }
}
