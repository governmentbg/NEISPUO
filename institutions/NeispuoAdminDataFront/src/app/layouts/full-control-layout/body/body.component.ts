import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Menu } from "../../../enums/menu.enum";
import { FormDataService } from "../../../services/form-data.service";

@Component({
  selector: "app-body",
  templateUrl: "./body.component.html",
  styleUrls: ["./body.component.scss"]
})
export class BodyComponent implements OnInit {
  constructor(private formDataService: FormDataService, private router: Router) {}

  ngOnInit() {
    this.formDataService.getMainMenuData().subscribe((menu: any) => {
      this.formDataService.mainMenuData = menu;
      if (this.formDataService.mainMenuData) {
        const tab = this.formDataService.mainMenuData[0];
        this.router.navigate(["/", Menu.Home, tab.path, tab.children[0].path]);
      }
    });
  }
}
