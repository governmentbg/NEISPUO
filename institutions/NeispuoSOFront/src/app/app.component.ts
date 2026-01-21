import { Component } from "@angular/core";
import { FormDataService } from "./services/form-data.service";
import { HelperService } from "./services/helpers.service";
import { MessagesService } from "./services/messages.service";
import { OIDCService } from "./services/oidc.service";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"]
})
export class AppComponent {
  title = "НЕИСПУО";

  constructor(
    private oidcService: OIDCService,
    private helperService: HelperService,
    private formDataService: FormDataService,
    private msgService: MessagesService
  ) {}

  async ngOnInit() {
    await this.oidcService.start();
    await this.msgService.getMessages().toPromise();
    this.formDataService.ipAddress = await this.formDataService.getIpAddress().toPromise();
    this.helperService.oidcStarted.next();
  }
}
