import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Subscription } from "rxjs";
import { Menu } from "src/app/enums/menu.enum";
import { Mode } from "src/app/enums/mode.enum";
import { MenuItem } from "src/app/models/menu-item.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../../../../environments/environment";

@Component({
  selector: "app-second-main-tab",
  templateUrl: "./second-main-tab.component.html",
  styleUrls: ["./second-main-tab.component.scss"]
})
export class SecondMainTabComponent implements OnInit, OnDestroy {
  menuData: MenuItem[] = [];
  menuItem: string;
  mode: Mode;
  isLoading: boolean = false;

  private paramSubsctiption: Subscription;
  private routeParamChangedSubscription: Subscription;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private formDataService: FormDataService
  ) {}

  ngOnInit() {
    const url = this.route.snapshot.url;

    this.mode = url[0].path === Menu.Edit || url[0].path === Menu.Create ? Mode.Edit : Mode.View;
    this.menuData = JSON.parse(JSON.stringify(this.formDataService.mainMenuData));

    this.paramSubsctiption = this.route.params.subscribe(params => {
      if (params.menuItem && params.menuItem !== this.menuItem) {
        this.menuItem = params.menuItem;
      }
    });

    this.routeParamChangedSubscription = this.helperService.routeParamChanged.subscribe(
      (paramChange: { paramName: string; paramValue: string }) => {
        if (
          paramChange &&
          paramChange.paramName === "type" &&
          this.menuData &&
          this.menuData.length &&
          this.menuItem !== this.menuData[0].path
        ) {
          this.menuItem = this.menuData[0].path;
        }
      }
    );
  }

  ngOnDestroy() {
    this.routeParamChangedSubscription && this.routeParamChangedSubscription.unsubscribe();
    this.paramSubsctiption && this.paramSubsctiption.unsubscribe();
  }

  onPathChanged(tabPath) {
    const tab = this.menuData.find(tab => tab.path === tabPath);
    if (tabPath !== this.menuItem && tab && tab.children && tab.children.length > 0) {
      sessionStorage.removeItem("list");

      let path = this.route.snapshot.params.parentForm
        ? `../../../${tabPath}/${tab.children[0].path}`
        : `../../${tabPath}/${tab.children[0].path}`;

      this.route.snapshot.params.grandParentForm && (path = `../../../../${tabPath}/${tab.children[0].path}`);

      let queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      let newQueryParams: any = {
        instid: queryParams.instid,
        sysuserid: queryParams.sysuserid,
        sysroleid: queryParams.sysroleid
      };

      environment.production && (newQueryParams = this.helperService.encodeParams(newQueryParams));

      this.router
        .navigate([path], {
          relativeTo: this.route,
          queryParams: newQueryParams
        })
        .then(res => {
          if (res) {
            this.menuItem = tabPath;
          }
        });
    }
  }
}
