import { Component, OnInit } from "@angular/core";
import { AuthService } from "src/app/auth/auth.service";

@Component({
  selector: "app-unauthorized",
  templateUrl: "./unauthorized.component.html",
  styleUrls: ["./unauthorized.component.scss"]
})
export class UnauthorizedComponent implements OnInit {
  constructor(private authService: AuthService) {}

  ngOnInit(): void {}

  async logout() {
    await this.authService.signout();
  }
}
