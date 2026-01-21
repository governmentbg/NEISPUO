import { HttpInterceptor, HttpRequest, HttpHandler, HttpEventType } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { SnackbarService } from "./snackbar.service";
import { environment } from "../../environments/environment";
import { Router } from "@angular/router";
import { Menu } from "../enums/menu.enum";

@Injectable()
export class ResponseInterceptorService implements HttpInterceptor {
  constructor(private snackBarService: SnackbarService, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    if (req.url.includes(environment.azureUrl))
      return next.handle(req).pipe(map(this.transformRes.bind(this, req)), catchError(this.catchAzureError));
    else if (req.url.includes(environment.azureUrl) || req.url.includes(environment.blobsMon) || req.url.includes(environment.regixUrl)) {
      return next.handle(req).pipe(map(this.transformRes.bind(this, req)), catchError(this.catchOtherError));
    } else {
      return next.handle(req).pipe(map(this.transformRes.bind(this, req)), catchError(this.catchCommonErr));
    }
  }

  private transformRes = (req, res) => {
    if (res && res.type === HttpEventType.Response) {
      if (res.body && res.body.exception) {
        if (!environment.production) {
          let errorMsg = "грешка - " + res.body.exception.split("--->")[0] + "\n";
          errorMsg += "файл - " + req.url.match(`(?:[^\\/](?!(\\|/)))+$`)[0] + "\n";
          errorMsg += "подадени данни - " + JSON.stringify(req.body);

          console.log(errorMsg);
        }

        this.snackBarService.openErrorSnackbar("Възникна грешка");
        this.router.navigate(["/", Menu.Home]);
        res = res.clone({ body: {} });
      } else if (res.body && res.body.data) {
        res = res.clone({ body: res.body.data });
      }
    }
    return res;
  };

  private catchCommonErr = error => {
    this.snackBarService.openErrorSnackbar("Възникна грешка");
    this.router.navigate(["/", Menu.Home]);
    return throwError("err");
  };

  private catchOtherError: any = error => {
    return throwError(error);
  };

  private catchAzureError = error => {
    return throwError(error.error);
  };
}
