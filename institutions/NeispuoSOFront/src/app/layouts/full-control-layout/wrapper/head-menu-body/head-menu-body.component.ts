import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Menu } from "src/app/enums/menu.enum";
import { TabsInt } from "src/app/enums/tabs.enum";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../../../environments/environment";

@Component({
  selector: "app-head-menu-body",
  templateUrl: "./head-menu-body.component.html",
  styleUrls: ["./head-menu-body.component.scss"]
})
export class HeadMenuBodyComponent implements OnInit {
  tabIndex: any = 0;
  isLoading: boolean;

  @Output() pathChanged: EventEmitter<string> = new EventEmitter<string>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formDataService: FormDataService,
    private helperService: HelperService
  ) {}

  ngOnInit() {
    this.tabIndex = TabsInt[this.route.snapshot.params.tab] || TabsInt[this.route.snapshot.url[0].path];
  }

  async onPathChanged(index) {
    const type = this.route.snapshot.params.type;
    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    this.isLoading = true;
    let res: any = await this.formDataService.getIsLocked(queryParams.instid).toPromise();
    res && res.length && (res = res[0]);
    this.formDataService.instIsLocked = !!res.isLocked;
    this.isLoading = false;

    queryParams = {
      instid: queryParams.instid,
      sysuserid: queryParams.sysuserid,
      sysroleid: queryParams.sysroleid,
      isLocked: this.formDataService.instIsLocked,
      schoolYear: this.formDataService.schoolYear,
      detailedSchoolType: this.formDataService.detailedSchoolType
    };
    environment.production && (queryParams = this.helperService.encodeParams(queryParams));

    this.pathChanged.emit(TabsInt[index]);

    if (index !== 1) {
      const tab =
        index === TabsInt.physical
          ? this.formDataService.physicalMenuData[0]
          : index === TabsInt.history
          ? this.formDataService.historyMenuData[0]
          : this.formDataService.mainMenuData[0];

      const tabChild = tab.children[0];

      const path = `/${Menu.Home}/${type}/${TabsInt[index]}/${tab.path}/${tabChild.path}`;

      this.router.navigate([path], { queryParams }).then(res => {
        if (res) {
          sessionStorage.removeItem("list");
          this.tabIndex = index;
        }
      });
    } else {
      const tabName = index === 1 ? "list" : "data";
      const path = index === 1 ? `/${Menu.Home}/${type}/${tabName}/validate` : `/${Menu.Home}/${type}/${tabName}`;
      this.router.navigate([path], { queryParams }).then(res => {
        if (res) {
          this.helperService.routeParamChanged.next({ paramName: "tab", paramValue: tabName });
          sessionStorage.removeItem("list");
          this.tabIndex = index;
        }
      });
    }
  }
}
