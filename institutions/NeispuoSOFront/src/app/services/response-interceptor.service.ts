import { HttpInterceptor, HttpRequest, HttpHandler, HttpEventType } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { SnackbarService } from "./snackbar.service";
import { environment } from "../../environments/environment";
import { Router } from "@angular/router";
import { Menu } from "../enums/menu.enum";
import { HelperService } from "./helpers.service";

@Injectable()
export class ResponseInterceptorService implements HttpInterceptor {
  constructor(private snackBarService: SnackbarService, private router: Router, private helperService: HelperService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    if (req.url.includes(environment.azureUrl) || req.url.includes(environment.blobsMon) || req.url.includes(environment.regixUrl) || req.url.includes(environment.ipUrl)) {
      return next.handle(req).pipe(map(this.transformRes.bind(this, req)), catchError(this.catchOtherError));
    } else {
      return next.handle(req).pipe(map(this.transformRes.bind(this, req)), catchError(this.catchCommonErr));
    }
  }

  private transformRes = (req, res) => {
    if (res && res.type === HttpEventType.Response) {
      if (res.body && res.body.exception) {
        sessionStorage.removeItem("list");
        sessionStorage.removeItem("url");
        sessionStorage.removeItem("paramsParent");
        sessionStorage.removeItem("tableName");

        if (!environment.production) {
          let errorMsg = "грешка - " + res.body.exception.split("--->")[0] + "\n";
          errorMsg += "файл - " + req.url.match(`(?:[^\\/](?!(\\|/)))+$`)[0] + "\n";
          errorMsg += "подадени данни - " + JSON.stringify(req.body);

          console.log(errorMsg);
        }

        this.snackBarService.openErrorSnackbar("Възникна грешка");
        this.helperService.routeParamChanged.next({ paramName: "type", paramValue: "" });
        this.helperService.errorOccured = true;

        this.router.navigate(["/", Menu.Home]);
        res = res.clone({ body: {} });
      } else if (res.body && res.body.data) {
        res = res.clone({ body: res.body.data });
      }
    }

    return res;
  };

  private catchCommonErr = error => {
    sessionStorage.removeItem("list");
    sessionStorage.removeItem("url");
    sessionStorage.removeItem("paramsParent");
    sessionStorage.removeItem("tableName");

    this.helperService.routeParamChanged.next({ paramName: "type", paramValue: "" });
    this.helperService.errorOccured = true;

    this.router.navigate(["/", Menu.Home]);

    const hasErrorMsg = error && error.error && typeof error.error === "string" && error.error.includes("Number:50000");

    if (hasErrorMsg) {
      const err = error.error.split("\n");
      const messageArr = err.length ? err[0].split(":") : ["Възникна грешка"];
      this.snackBarService.openErrorSnackbar(messageArr.length > 1 ? messageArr[1] : messageArr[0]);
    } else {
      this.snackBarService.openErrorSnackbar("Възникна грешка");
    }

    return throwError(-1);
  };

  private catchOtherError: any = error => {
    return throwError(error);
  };
}
