import { Component, OnInit } from "@angular/core";
import { AuthService } from "src/app/auth/auth.service";

@Component({
  selector: "app-body",
  templateUrl: "./body.component.html",
  styleUrls: ["./body.component.scss"]
})
export class BodyComponent implements OnInit {
  hidden = false;
  authorized: boolean = true;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authorized = this.authService.isRuo() || this.authService.isMon();
  }
}
