import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { FormDataService } from "src/app/services/form-data.service";
import { environment } from "src/environments/environment";
import { FormType } from "../../../enums/formType.enum";
import { Menu, MenuInt } from "../../../enums/menu.enum";

@Component({
  selector: "app-main-tab",
  templateUrl: "./main-tab.component.html",
  styleUrls: ["./main-tab.component.scss"]
})
export class MainTabComponent implements OnInit {
  tabIndex: number;
  authorized: boolean = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private formDataService: FormDataService
  ) {}

  ngOnInit() {
    this.tabIndex = MenuInt[this.route.snapshot.url[0].path];
    this.authorized = this.authService.isMon();
  }

  onPathChanged(index: number) {
    if (index !== this.tabIndex) {
      let path = `${MenuInt[index]}`;
      path = path === Menu.Active || path === Menu.Inactive ? "/" + path + `/${FormType.School}` : "/" + path;

      let queryParams: any = {
        sysuserid: this.authService.getSysUserId(),
        region: this.authService.getRegion()
      };

      environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

      this.router.navigate([path], { queryParams }).then(res => {
        if (res) {
          this.tabIndex = index;
        }
      });
    }
  }
}
