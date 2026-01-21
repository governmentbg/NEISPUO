import { HttpInterceptor, HttpRequest, HttpHandler } from "@angular/common/http";

import { environment } from "../../environments/environment";

export class DomainInterceptorService implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    let modifiedRequest;
    if (req.url.startsWith("assets") || req.url.startsWith("http")) {
      modifiedRequest = req;
    } else if (req.url.includes("azure-integrations")) {
      modifiedRequest = req.clone({ url: environment.azureUrl + req.url });
    } else {
      modifiedRequest = req.clone({ url: environment.apiUrl + req.url });
    }
    return next.handle(modifiedRequest);
  }
}
