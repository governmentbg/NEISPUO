import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Menu } from "../../enums/menu.enum";

@Component({
  selector: "app-layout",
  templateUrl: "./layout.component.html",
  styleUrls: ["./layout.component.scss"]
})
export class LayoutComponent implements OnInit {
  path: string;

  constructor(private route: ActivatedRoute) {}

  get menu() {
    return Menu;
  }

  ngOnInit() {
    this.path = this.route.snapshot.url[0].path;
  }
}
