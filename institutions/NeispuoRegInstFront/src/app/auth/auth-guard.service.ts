import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivate, Router } from "@angular/router";
import { FormWrapperComponent } from "../pages/form-wrapper/form-wrapper.component";
import { AuthService } from "./auth.service";
import { Menu } from "../enums/menu.enum";
import { environment } from "../../environments/environment";
import { FormDataService } from "../services/form-data.service";

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
  constructor(private authService: AuthService, private router: Router, private formDataService: FormDataService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const url = route.url && route.url.length > 0 ? route.url[0].path : null;
    const isLoggedIn = this.authService.isLoggedIn();

    const currentQueryParams = environment.production
      ? this.formDataService.decodeParams(route.queryParams.q || "")
      : route.queryParams;
    const actualSysUserId = this.authService.getSysUserId();
    const sysuserid = currentQueryParams.sysuserid;

    const actualRegionId = this.authService.getRegion();
    const region = currentQueryParams.region;

    let queryParams: any = { sysuserid: actualSysUserId, region: actualRegionId + "" };
    environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

    if (!isLoggedIn) {
      this.authService.signin();
      return false;
    } else if (isLoggedIn && this.authService.hasNoRights() && url !== environment.unauthorizedRedirectUrl) {
      this.router.navigate([environment.unauthorizedRedirectUrl]);
      return false;
    } else if (isLoggedIn && this.authService.isExpert() && url === Menu.NewInstitution) {
      this.router.navigate(["/", Menu.Home, { queryParams }]);
      return false;
    } else if (
      isLoggedIn &&
      (sysuserid != actualSysUserId || region + "" != actualRegionId + "") &&
      url !== environment.unauthorizedRedirectUrl
    ) {
      this.router.navigate(["/", Menu.Home], { queryParams });
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
