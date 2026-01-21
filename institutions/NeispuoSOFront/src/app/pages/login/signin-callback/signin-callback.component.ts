import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Menu } from "../../../enums/menu.enum";
import { OIDCService } from "../../../services/oidc.service";

@Component({
  selector: "app-signin-callback",
  templateUrl: "./signin-callback.component.html",
  styleUrls: ["./signin-callback.component.scss"]
})
export class SigninCallbackComponent implements OnInit {
  error = null;

  constructor(private oidcService: OIDCService, private router: Router) {}

  ngOnInit(): void {
    this.handleResponse();
  }

  private async handleResponse() {
    try {
      await this.oidcService.userManager.signinRedirectCallback();
      this.goHome();
    } catch (e) {
      this.error = e;
    }
  }

  goHome() {
    this.router.navigate(["/", Menu.Home]);
  }
}
