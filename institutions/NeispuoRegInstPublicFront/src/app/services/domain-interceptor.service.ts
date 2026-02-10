import { HttpInterceptor, HttpRequest, HttpHandler } from "@angular/common/http";

import { environment } from "../../environments/environment";

export class DomainInterceptorService implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const modifiedRequest = req.url.startsWith("assets") ? req : req.clone({ url: environment.apiUrl + req.url });
    return next.handle(modifiedRequest);
  }
}
