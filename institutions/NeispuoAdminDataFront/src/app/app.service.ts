import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { forkJoin, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { FormDataService } from "./services/form-data.service";
import { HelperService } from "./services/helpers.service";
import { MessagesService } from "./services/messages.service";
import { OIDCService } from "./services/oidc.service";

@Injectable({
  providedIn: "root"
})
export class AppService implements Resolve<any> {
  constructor(
    private oidcService: OIDCService,
    private helperService: HelperService,
    private formDataService: FormDataService,
    private msgService: MessagesService
  ) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
    return forkJoin([
      this.oidcService.start(),
      this.msgService.getMessages(),
      this.formDataService.getIpAddress()
    ]).pipe(
      tap((res: any) => {
        this.formDataService.ipAddress = res[2];
        this.helperService.oidcStarted.next();
      })
    );
  }
}
