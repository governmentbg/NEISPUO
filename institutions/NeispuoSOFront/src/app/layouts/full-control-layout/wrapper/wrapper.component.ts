import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { forkJoin, Subscribable } from "rxjs";
import { AuthService } from "src/app/auth/auth.service";
import { Tabs } from "src/app/enums/tabs.enum";
import { MenuItem } from "src/app/models/menu-item.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-wrapper",
  templateUrl: "./wrapper.component.html",
  styleUrls: ["./wrapper.component.scss"]
})
export class WrapperComponent implements OnInit {
  hidden = false;
  isLoading = true;
  menuData: MenuItem[] = [];
  authorized: boolean = true;
  menuItem: string = "";
  currentTab: string = "";
  version: string;

  constructor(
    private formDataService: FormDataService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private helperService: HelperService
  ) {}

  get tabs() {
    return Tabs;
  }

  async ngOnInit() {
    const type = this.route.snapshot.params.type ? this.route.snapshot.params.type : this.authService.getType();

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };
    const id = queryParams.instid ? queryParams.instid : this.authService.getInstId();
    id && this.helperService.routeParamChanged.next({ paramName: "id", paramValue: id });

    const versionRes: any = this.route.parent.snapshot.data.version;
    this.version = versionRes.length ? versionRes[0].Version : versionRes.Version || "0.0";
    this.authorized = this.authService.isMon() || this.authService.isRuo();

    if (!this.formDataService.mainMenuData) {
      const requests: Subscribable<any>[] = [];
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

        this.currentTab = this.route.snapshot.params.tab || this.route.snapshot.url[0].path;

        if (this.currentTab !== Tabs.list) {
          this.menuData = this.currentTab === Tabs.physical ? JSON.parse(JSON.stringify(reqRes[1])) : JSON.parse(JSON.stringify(reqRes[0]));
        }
      });
    } else {
      this.isLoading = false;

      this.currentTab = this.route.snapshot.params.tab || this.route.snapshot.url[0].path;

      if (this.currentTab !== Tabs.list) {
        this.menuData = this.currentTab === Tabs.physical ? this.formDataService.physicalMenuData : this.formDataService.mainMenuData;
      }
    }
  }

  tabChanged(tabName: string) {
    this.currentTab = tabName;
  }
}
