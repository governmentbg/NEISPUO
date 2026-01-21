import { Component } from "@angular/core";
import { OIDCService } from "./services/oidc.service";
import { MessagesService } from "./services/messages.service";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"]
})
export class AppComponent {
  title = "НЕИСПУО";

  constructor(private oidcService: OIDCService, private msgService: MessagesService) {}

  async ngOnInit() {
    await this.oidcService.start();
    await this.msgService.getMessages().toPromise();
  }
}
