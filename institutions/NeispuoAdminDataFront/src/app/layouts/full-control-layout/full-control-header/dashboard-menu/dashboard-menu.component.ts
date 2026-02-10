import { Component, OnInit } from "@angular/core";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-dashboard-menu",
  templateUrl: "./dashboard-menu.component.html",
  styleUrls: ["./dashboard-menu.component.scss"]
})
export class DashboardMenuComponent implements OnInit {
  targetLink: string;

  constructor() {}

  ngOnInit(): void {
    this.targetLink = environment.portalUrl;
  }
}
