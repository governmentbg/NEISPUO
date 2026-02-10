import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { forkJoin, Subscribable, Subscription } from "rxjs";
import { AuthService } from "src/app/auth/auth.service";
import { headmasterMenu, monRuoMenu } from "src/app/enums/constants";
import { FormTypeInt } from "../../../enums/formType.enum";
import { headmasterMenuInt, Menu, monRuoMenuInt } from "../../../enums/menu.enum";
import { environment } from "../../../../environments/environment";
import { HelperService } from "src/app/services/helpers.service";
import { TabsInt } from "src/app/enums/tabs.enum";
import { FormDataService } from "src/app/services/form-data.service";
import { MenuItem } from "src/app/models/menu-item.interface";

@Component({
  selector: "app-main-tab",
  templateUrl: "./main-tab.component.html",
  styleUrls: ["./main-tab.component.scss"]
})
export class MainTabComponent implements OnInit, OnDestroy {
  tabIndex: number;
  tabItems = [];

  isLoading = false;
  isMonRuo: boolean;

  private routeParamChangedSubscription: Subscription;

  constructor(
    private router: Router,
    private helperService: HelperService,
    private authService: AuthService,
    private formDataService: FormDataService
  ) {}

  ngOnInit() {
    this.isMonRuo = this.authService.isRuo() || this.authService.isMon();
    const menu = this.isMonRuo ? monRuoMenu : headmasterMenu;
    const menuInt = this.isMonRuo ? monRuoMenuInt : headmasterMenuInt;
    const path = this.getPath();
    this.tabIndex = path && menuInt[path] >= 0 ? menuInt[path] : -1;

    for (const key in menu) {
      if ((key === Menu.Settings) && !this.authService.isMon()) {
        continue;
      }

      this.tabItems.push(menu[key]);
    }

    this.routeParamChangedSubscription = this.helperService.routeParamChanged.subscribe(
      (paramChange: { paramName: string; paramValue: string }) => {
        if (paramChange && paramChange.paramName === "type" && this.tabIndex !== 0) {
          this.tabIndex = 0;
        }
      }
    );
  }

  ngOnDestroy() {
    this.routeParamChangedSubscription && this.routeParamChangedSubscription.unsubscribe();
  }

  async onPathChanged(index: number) {
    if (index !== this.tabIndex) {
      const type = this.authService.getType();

      sessionStorage.removeItem("url");
      // sessionStorage.removeItem("regixData");

      await this.getTab(type, index);
    }
  }

  private async getTab(type: string, index: number) {
    this.isMonRuo && index !== 4 && index !== 5 && (this.formDataService.mainMenuData = null);
    if (this.authService.isMon() && index === 5) {
      const path = `${Menu.Home}/settings`;
      this.router.navigate([path], {}).then(res => {
        if (res) {
          sessionStorage.removeItem("list");
          this.tabIndex = index;
        }
      });
    } 
    else if (this.isMonRuo && index === 4) {
      const path = `${Menu.Home}/infotable`;
      this.router.navigate([path], {}).then(res => {
        if (res) {
          sessionStorage.removeItem("list");
          this.tabIndex = index;
        }
      });
    }
    else if (!this.formDataService.mainMenuData) {
      this.isLoading = true;
      type = this.isMonRuo ? FormTypeInt[index] : type;

      let requests: Subscribable<any>[] = [];
      requests.push(this.formDataService.getMainMenuData(type));
      requests.push(this.formDataService.getPhysicalMenuData());
      requests.push(this.formDataService.getHistoryMenuData(type));
      requests.push(this.formDataService.getSampleListData());

      forkJoin(requests).subscribe((reqRes: any[]) => {
        this.formDataService.mainMenuData = reqRes[0];
        this.formDataService.physicalMenuData = reqRes[1];
        this.formDataService.historyMenuData = reqRes[2];
        this.formDataService.sampleListData = reqRes[3];

        this.isLoading = false;

        const tab = this.findTab(index);
        const tabName = index === 1 ? "list" : "data";
        const subTab = "validate";
        const path = index === 1 ? `/${Menu.Home}/${type}/${tabName}/${subTab}` : `/${Menu.Home}/${type}/${tabName}`;
        tab || this.isMonRuo
          ? this.navigateAway(type, tab, index)
          : this.router.navigate([path]).then(res => {
              if (res) {
                sessionStorage.removeItem("list");
                this.tabIndex = index;
              }
            });
      });
    } else {
      const tab = this.findTab(index);
      const tabName = index === 1 ? "list" : "data";
      const subTab = "validate";
      const path = index === 1 ? `/${Menu.Home}/${type}/${tabName}/${subTab}` : `/${Menu.Home}/${type}/${tabName}`;

      !this.isMonRuo &&
        this.helperService.routeParamChanged.next({ paramName: "tab", paramValue: tab ? tab.path : tabName });

      this.isLoading = true;
      const instid = this.authService.getInstId();

      let res: any = await this.formDataService.getIsLocked(instid).toPromise();
      res && res.length && (res = res[0]);
      this.formDataService.instIsLocked = !!res.isLocked;

      this.isLoading = false;

      let queryParams: any = {
        instid,
        sysuserid: this.authService.getSysUserId(),
        sysroleid: this.authService.getSysRoleId(),
        isLocked: this.formDataService.instIsLocked,
        schoolYear: this.formDataService.schoolYear,
        detailedSchoolType: this.formDataService.detailedSchoolType
      };

      environment.production && (queryParams = this.helperService.encodeParams(queryParams));

      tab
        ? this.navigateAway(type, tab, index)
        : this.router.navigate([path], { queryParams }).then(res => {
            if (res) {
              sessionStorage.removeItem("list");
              this.tabIndex = index;
            }
          });
    }
  }

  private navigateAway(type: string, tab: MenuItem, index: number) {
    const path = this.isMonRuo
      ? `${Menu.Home}/${FormTypeInt[index]}`
      : `/${Menu.Home}/${type}/${TabsInt[index]}/${tab.path}/${tab.children[0].path}`;

    let queryParams: any = this.isMonRuo
      ? { sysuserid: this.authService.getSysUserId(), sysroleid: this.authService.getSysRoleId() }
      : {
          instid: this.authService.getInstId(),
          sysuserid: this.authService.getSysUserId(),
          sysroleid: this.authService.getSysRoleId(),
          isLocked: this.formDataService.instIsLocked,
          schoolYear: index !== 5 ? this.formDataService.schoolYear : null,
          detailedSchoolType: this.formDataService.detailedSchoolType
        };
    environment.production && (queryParams = this.helperService.encodeParams(queryParams));

    (type || this.isMonRuo) &&
      this.router.navigate([path], { queryParams }).then(res => {
        if (res) {
          sessionStorage.removeItem("list");
          this.tabIndex = index;
        }
      });
  }

  private getPath(): string {
    const urlParts = this.router.url.split(/\/|\?/);

    let res;

    if (this.isMonRuo) {
      res = urlParts && urlParts.length > 2 ? urlParts[2] : "";
    } else {
      res = urlParts && urlParts.length > 3 ? urlParts[3] : "";
    }

    return res;
  }

  private findTab(index) {
    switch (index) {
      case TabsInt.physical:
        return this.formDataService.physicalMenuData[0];
      case TabsInt.history:
        return this.formDataService.historyMenuData[0];
      case TabsInt.main:
        return this.formDataService.mainMenuData[0];
      default:
        return null;
    }
  }
}
