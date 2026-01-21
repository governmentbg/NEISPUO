import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "../../auth/auth.service";

@Component({
  selector: "app-menu-profile",
  templateUrl: "./menu-profile.component.html",
  styleUrls: ["./menu-profile.component.scss"]
})
export class MenuProfileComponent {
  constructor(private authService: AuthService, private router: Router) {}

  logout() {
    const token = this.authService.getToken();
    this.authService.removeFromStorage("token");
    this.router.navigate(["/"]).then(res => {
      if (res) {
        this.authService.clearStorage();
      } else {
        this.authService.setToken(token);
      }
    });
  }
}
