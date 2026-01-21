import { HttpInterceptor, HttpRequest, HttpHandler } from "@angular/common/http";

import { environment } from "../../environments/environment";

export class DomainInterceptorService implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    let modifiedRequest;

    if (req.url.startsWith("assets") || req.url.startsWith("http")) {
      modifiedRequest = req;
    } else {
      modifiedRequest = req.clone({ url: environment.apiUrl + req.url });
    }

    //TODO - remove
    if (req.body) {
      req.body.instid = 2999951;
      req.body.instType = 2;
      req.body.extdata = 0;
    }

    return next.handle(modifiedRequest);
  }
}
