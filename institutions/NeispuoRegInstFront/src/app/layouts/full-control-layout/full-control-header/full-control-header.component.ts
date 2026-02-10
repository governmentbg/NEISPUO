import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Menu } from '../../../enums/menu.enum';

@Component({
  selector: "app-full-control-header",
  templateUrl: "./full-control-header.component.html",
  styleUrls: ["./full-control-header.component.scss"]
})
export class FullControlHeaderComponent implements OnInit {
  showSecondMenu: boolean;
  showFirstMenu: boolean;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    const currentRoute = this.route.snapshot.url[0].path;
    this.showSecondMenu = currentRoute === Menu.Inactive || currentRoute === Menu.Active;
    this.showFirstMenu = Object.values(Menu).includes(<Menu>currentRoute);
  }
}
