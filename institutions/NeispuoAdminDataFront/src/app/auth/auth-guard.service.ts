import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivate, Router } from "@angular/router";
import { FormWrapperComponent } from "../pages/form-wrapper/form-wrapper.component";
import { AuthService } from "./auth.service";
import { environment } from "../../environments/environment";

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
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const isLoggedIn = this.authService.isLoggedIn();
    const url = route.url && route.url.length > 0 ? route.url[0].path : null;

    if (!isLoggedIn) {
      this.authService.signin();
      return false;
    } else if (!this.authService.isAuthorized() && url !== environment.unauthorizedRedirectUrl) {
      this.router.navigate([environment.unauthorizedRedirectUrl]);
      return false;
    }

    return true;
  }
}
