import { Component, OnInit } from "@angular/core";
import { OIDCService } from "../../../services/oidc.service";

@Component({
  selector: "app-silent-signin-callback",
  templateUrl: "./silent-signin-callback.component.html",
  styleUrls: ["./silent-signin-callback.component.scss"]
})
export class SilentSigninCallbackComponent implements OnInit {
  constructor(private oidcService: OIDCService) {}

  async ngOnInit() {
    try {
      await this.oidcService.userManager.signinSilentCallback();
    } catch (e) {
      console.log("signinSilentCallback failed", e);
    }
  }
}
