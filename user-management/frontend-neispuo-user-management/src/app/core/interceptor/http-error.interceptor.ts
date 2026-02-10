import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize, retry } from 'rxjs/operators';
import { SpinnerService } from 'src/app/shared/services/spinner-service';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
    constructor(private spinnerService: SpinnerService, private router: Router) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this.spinnerService.displaySpinner();
        return next.handle(request).pipe(
            catchError((response: HttpErrorResponse) => {
                if (response.error.statusCode === 401) {
                    // no JWT or expired JWT –> redirect to login
                    this.router.navigate(['/login']);
                }
                return throwError(response.error.message);
            }),
            finalize(() => {
                // hide loading spinner
                this.spinnerService.hideSpinner();
            }),
        );
    }
}
