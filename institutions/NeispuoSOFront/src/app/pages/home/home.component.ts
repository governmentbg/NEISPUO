import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { AuthService } from "src/app/auth/auth.service";
import { instType } from "src/app/enums/constants";
import { FormType } from "src/app/enums/formType.enum";
import { Menu } from "src/app/enums/menu.enum";
import { Tabs } from "src/app/enums/tabs.enum";
import { MenuItem } from "src/app/models/menu-item.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../environments/environment";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.scss"]
})
export class HomeComponent implements OnInit, OnDestroy {
  isLoading = true;

  private oidcStartedSubscription: Subscription;

  constructor(
    private formDataService: FormDataService,
    private helperService: HelperService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    if (!this.helperService.errorOccured) {
      this.init();
    } else {
      this.helperService.errorOccured = false;
      this.isLoading = false;
    }
  }

  ngOnDestroy() {
    this.oidcStartedSubscription && this.oidcStartedSubscription.unsubscribe();
  }

  private init() {
    const type = this.authService.getType();
    if (this.authService.isHeadmaster() && !type) {
      const instid = this.authService.getInstId();

      this.formDataService.getInstType(instid).subscribe(
        (data: { instType: number }[]) => {
          data && data.length && sessionStorage.setItem("type", btoa(<FormType>instType[data[0].instType - 1]));

          if (data && data.length) {
            this.getMenuData();
          } else {
            !(data && data.length) && (this.isLoading = false);
          }
        },
        err => {
          this.isLoading = false;
        }
      );
    } else if (!this.authService.isHeadmaster() && type) {
      this.getMenuData();
    } else {
      let queryParams: any = { sysuserid: this.authService.getSysUserId(), sysroleid: this.authService.getSysRoleId() };
      environment.production && (queryParams = this.helperService.encodeParams(queryParams));
      this.router.navigate(["/", Menu.Home, FormType.School], { queryParams });
      this.isLoading = false;
    }
  }

  private getMenuData() {
    const type = this.authService.getType();
    this.formDataService.getMainMenuData(type).subscribe(
      (menuData: MenuItem[]) => {
        const tab = menuData && menuData.length > 0 ? menuData[0] : null;
        const tabChild = tab && tab.children && tab.children.length > 0 ? tab.children[0] : null;
        const instid = this.authService.getInstId();
        const sysuserid = this.authService.getSysUserId();
        const sysroleid = this.authService.getSysRoleId();

        if (tab && tabChild) {
          const path = `/${Menu.Home}/${type}/${Tabs.main}/${tab.path}/${tabChild.path}`;

          let queryParams: any = {
            instid,
            sysuserid,
            sysroleid,
            isLocked: this.formDataService.instIsLocked,
            detailedSchoolType: this.formDataService.detailedSchoolType
          };
          environment.production && (queryParams = this.helperService.encodeParams(queryParams));

          type && this.router.navigate([path], { queryParams });
        }

        this.isLoading = false;
      },
      err => {
        this.isLoading = false;
      }
    );
  }
}
