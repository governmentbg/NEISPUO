import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { Subscription } from "rxjs";
import { AuthService } from "src/app/auth/auth.service";
import { Menu } from "src/app/enums/menu.enum";
import { Mode } from "src/app/enums/mode.enum";
import { Tabs } from "src/app/enums/tabs.enum";
import { MenuItem } from "src/app/models/menu-item.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { MessagesService } from "src/app/services/messages.service";
import { SnackbarService } from "src/app/services/snackbar.service";
import { environment } from "../../../../../environments/environment";

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
    private formDataService: FormDataService,
    private authService: AuthService,
    private snackbarService: SnackbarService,
    private messageService: MessagesService
  ) {}

  ngOnInit() {
    const url = this.route.snapshot.url;

    this.mode = url[0].path === Menu.Edit || url[0].path === Menu.Create ? Mode.Edit : Mode.View;

    this.paramSubsctiption = this.route.params.subscribe(params => {
      this.menuData =
        params.tab === Tabs.physical
          ? JSON.parse(JSON.stringify(this.formDataService.physicalMenuData))
          : params.tab === Tabs.history
          ? JSON.parse(JSON.stringify(this.formDataService.historyMenuData))
          : JSON.parse(JSON.stringify(this.formDataService.mainMenuData));

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
        sysroleid: queryParams.sysroleid,
        isLocked: this.formDataService.instIsLocked,
        schoolYear: this.formDataService.schoolYear,
        detailedSchoolType: this.formDataService.detailedSchoolType
      };

      if (this.route.snapshot.params.tab === Tabs.history) {
        newQueryParams.period = queryParams.period;
        newQueryParams.year = queryParams.year;
      }

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

  actualizeData() {
    this.isLoading = true;

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    const body = {
      data: { sysuserid: queryParams.sysuserid, instid: queryParams.instid },
      procedureName: "inst_year.confDataVersion",
      operationType: 2
    };

    this.formDataService.performProcedure(body).subscribe(
      res => {
        this.snackbarService.openSuccessSnackbar(this.messageService.successMessages.saveSuccess);
        this.isLoading = false;
      },
      err => (this.isLoading = false)
    );
  }

  showActualizationButton() {
    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    return (
      this.mode !== Mode.Edit &&
      queryParams.extAlldata == 1 &&
      queryParams.sysroleid == 0 &&
      !this.authService.isMon() &&
      !this.authService.isRuo()
    );
  }
}
