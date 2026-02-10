import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "../../auth/auth.service";
import { OIDCService } from "../../services/oidc.service";

@Component({
  selector: "app-menu-profile",
  templateUrl: "./menu-profile.component.html",
  styleUrls: ["./menu-profile.component.scss"]
})
export class MenuProfileComponent implements OnInit {
  nameAbr = "";
  name = "";
  email = "";

  constructor(private authService: AuthService, private router: Router, public oidcService: OIDCService) {}

  ngOnInit() {
    const data = this.authService.getUserData();
    this.name = data.name;
    this.email = data.email;

    const splitName = this.name.split(" ");
    this.nameAbr =
      (splitName.length > 0 && splitName[0] ? splitName[0][0] : "") + (splitName.length > 1 && splitName[1] ? splitName[1][0] : "");
  }

  async logout() {
    await this.authService.signout();
  }
}
