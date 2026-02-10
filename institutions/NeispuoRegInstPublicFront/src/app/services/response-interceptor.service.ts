import { HttpInterceptor, HttpRequest, HttpHandler, HttpEventType } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { of } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { SnackbarService } from "./snackbar.service";

@Injectable()
export class ResponseInterceptorService implements HttpInterceptor {
  constructor(private snackBarService: SnackbarService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    return next.handle(req).pipe(
      map(res => {
        if (res.type === HttpEventType.Response) {
          if (res.body.exception) {
            this.snackBarService.openErrorSnackbar("Възникна грешка");
            res = res.clone({ body: [] });
          } else if (res.body.data) {
            res = res.clone({ body: res.body.data });
          }
        }
        return res;
      }),
      catchError(error => {
        // for blob download
        const status = error?.error?.status || error?.status;
        if (status === 401) {
          this.snackBarService.openErrorSnackbar("Нямате права да извършите това действие");
        } else {
          this.snackBarService.openErrorSnackbar("Възникна грешка");
        }

        return of(error);
      })
    );
  }
}
