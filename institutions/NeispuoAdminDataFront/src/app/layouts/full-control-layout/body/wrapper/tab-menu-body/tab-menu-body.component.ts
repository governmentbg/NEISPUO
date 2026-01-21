import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Subscription } from "rxjs";
import { MenuItem } from "src/app/models/menu-item.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../../../../environments/environment";

@Component({
  selector: "app-tab-menu-body",
  templateUrl: "./tab-menu-body.component.html",
  styleUrls: ["./tab-menu-body.component.scss"]
})
export class TabMenuBodyComponent implements OnInit, OnDestroy {
  menuData: MenuItem[] = [];

  currentTab: MenuItem;
  childTab: MenuItem = null;

  hasBackButton: boolean = false;
  prevPath: string;
  prevTab: string;

  data: { code: number | string; label: string; additionalParams: Object }[] = null;
  current: { code: number | string; label: string };
  switchLabel: string;
  paramName: string;

  private routeSubscription: Subscription;
  private tableItemSelectedSubscription: Subscription;
  private paramSubsctiption: Subscription;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private formDataService: FormDataService,
    private helperService: HelperService
  ) {}

  ngOnInit() {
    this.initParamSubscription();
    this.initTableItemSubscription();
  }

  private initParamSubscription() {
    this.paramSubsctiption = this.route.params.subscribe(params => {
      this.menuData = JSON.parse(JSON.stringify(this.formDataService.mainMenuData));

      this.hasBackButton = !!params.parentForm;

      if ((params.menuItem && params.menuItem !== this.prevPath) || params.tab !== this.prevTab) {
        const main = this.menuData.find(tab => tab.path === params.menuItem);

        if (params.grandParentForm) {
          const parent = main.children.find(tab => tab.path === params.grandParentForm);
          this.currentTab = parent.children.find(tab => tab.path === params.parentForm);
        } else if (params.parentForm) {
          this.currentTab = main.children.find(tab => tab.path === params.parentForm);
        } else {
          this.currentTab = main;
        }

        this.prevPath = params.parentForm ? params.parentForm : params.menuItem;
        this.prevTab = params.tab;
      }

      if (
        this.currentTab &&
        this.currentTab.children &&
        (!this.childTab || (params.formName && params.formName !== this.childTab.path))
      ) {
        this.childTab = this.currentTab.children.find(tab => tab.path === params.formName);
      }
    });
  }

  private initTableItemSubscription() {
    this.tableItemSelectedSubscription = this.helperService.tableItemSelected.subscribe(
      (data: { paramName: string; formDataId: string | number; additionalParams: Object }) => {
        const path = this.childTab.children[0].path;

        sessionStorage.setItem("paramsParent", JSON.stringify(this.route.snapshot.queryParams));

        let queryParams = environment.production
          ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
          : { ...this.route.snapshot.queryParams };

        queryParams[data.paramName] = data.formDataId;
        queryParams = { ...queryParams, ...data.additionalParams };
        environment.production && (queryParams = this.helperService.encodeParams(queryParams));

        this.router.navigate([`${path}`], { relativeTo: this.route, queryParams });
      }
    );
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.tableItemSelectedSubscription && this.tableItemSelectedSubscription.unsubscribe();
    this.paramSubsctiption && this.paramSubsctiption.unsubscribe();
  }

  onTabChanged(tab: MenuItem) {
    if (this.childTab !== tab) {
      const path = `../${tab.path}`;
      this.childTab = tab;

      this.router.navigate([path], { relativeTo: this.route, queryParamsHandling: "preserve" });
    }
  }

  goBack() {
    const path = `..`;
    let queryParams;
    if (this.route.snapshot.params.grandParentForm) {
      queryParams = JSON.parse(sessionStorage.getItem("paramsParent"));
      sessionStorage.removeItem("paramsParent");
    } else {
      queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      sessionStorage.removeItem("list");
    }

    this.router.navigate([path], { relativeTo: this.route, queryParams });
  }
}
