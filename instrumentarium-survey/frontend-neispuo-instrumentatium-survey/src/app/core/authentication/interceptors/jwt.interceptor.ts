import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { concatMap } from 'rxjs/operators';
import { OIDCService } from '../auth-state-manager/oidc.service';

@Injectable({ providedIn: 'root' })
export class JwtInterceptor implements HttpInterceptor {
  constructor(private oidService: OIDCService) {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.oidService.getJwt().pipe(
      concatMap((u) => {
        if (u) {
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${u.idToken}`
            }
          });
        }
        return next.handle(request);
      })
    );
  }
}
