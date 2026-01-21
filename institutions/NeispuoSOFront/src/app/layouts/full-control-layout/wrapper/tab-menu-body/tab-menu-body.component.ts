import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { MatTabGroup } from "@angular/material/tabs";
import { ActivatedRoute, Router } from "@angular/router";
import { combineLatest, Subscription } from "rxjs";
import { Tabs } from "src/app/enums/tabs.enum";
import { MenuItem } from "src/app/models/menu-item.interface";
import { Option } from "src/app/models/option.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../../../environments/environment";

@Component({
  selector: "app-tab-menu-body",
  templateUrl: "./tab-menu-body.component.html",
  styleUrls: ["./tab-menu-body.component.scss"]
})
export class TabMenuBodyComponent implements OnInit, OnDestroy {
  @ViewChild("matTabGroup") matTabGroup: MatTabGroup;

  menuData: MenuItem[] = [];

  currentTab: MenuItem;
  visibleTabs: MenuItem[] = [];
  childTab: MenuItem = null;

  hasBackButton: boolean = false;
  hasAddress: boolean;
  hasPeriodFilters: boolean;
  prevPath: string;
  prevTab: string;

  data: { code: number | string; label: string; additionalParams: Object }[] = null;
  current: { code: number | string; label: string };
  switchLabel: string;
  paramName: string;

  addressData: Option[] = null;
  yearData: Option[] = null;
  periodData: Option[] = null;

  private routeSubscription: Subscription;
  private tableItemSelectedSubscription: Subscription;
  private paramSubsctiption: Subscription;

  private addressDataSubscription: Subscription;
  private historyDataSubscription: Subscription;

  private prevParams: any;
  private prevInstid: string;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private formDataService: FormDataService,
    private helperService: HelperService
  ) {}

  ngOnInit() {
    this.initParamSubscription();
    this.initTableItemSubscription();
    this.initAddressSubsctiprion();
    this.initHistorySubscription();
  }

  private initParamSubscription(): void {
    this.paramSubsctiption = combineLatest([
      this.route.params,
      this.route.queryParams
    ]).subscribe(([params, queryParams]) => {
      const decodedQueryParams = environment.production
        ? this.helperService.decodeParams(queryParams['q'] || '')
        : queryParams;

      const currentInstid = decodedQueryParams['instid'];

      const paramsChanged = JSON.stringify(params) !== JSON.stringify(this.prevParams);
      const instidChanged = currentInstid !== this.prevInstid;

      if (!paramsChanged && !instidChanged) {
        return;
      }

      this.prevParams = { ...params };
      this.prevInstid = currentInstid;

      this.hasAddress = params.tab === Tabs.physical && params.menuItem === this.formDataService.physicalMenuData[0].path;
      this.hasPeriodFilters = params.tab === Tabs.history;

      this.menuData =
        params.tab === Tabs.physical
          ? JSON.parse(JSON.stringify(this.formDataService.physicalMenuData))
          : params.tab === Tabs.history
            ? JSON.parse(JSON.stringify(this.formDataService.historyMenuData))
            : JSON.parse(JSON.stringify(this.formDataService.mainMenuData));

      this.hasBackButton = !!params.parentForm;

      if (params.parentForm && !this.prevPath && !this.data) {
        const tableParams = sessionStorage.getItem("list");

        if (this.hasBackButton && tableParams) {
          const decodedParams = this.helperService.decodeParams(tableParams);
          this.switchLabel = decodedParams.switchLabel;
          this.paramName = decodedParams.paramName;

          const dataName = params.parentForm;

          this.formDataService.getSwitchTableList(dataName, currentInstid, decodedParams).subscribe((res: any) => {
            this.data = res;

            const current = this.data.find(row => {
              let isCurrent = true;
              isCurrent = isCurrent && row.code == decodedQueryParams[this.paramName];

              for (let key in row.additionalParams) {
                isCurrent = isCurrent && row.additionalParams[key] == decodedQueryParams[key];
              }

              return isCurrent;
            });

            if (current) {
              this.current = current;
            }
          });
        }
      }

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

      if (this.currentTab && this.currentTab.children && (!this.childTab || (params.formName && params.formName !== this.childTab.path))) {
        const index = this.currentTab.children.findIndex(tab => tab.path === params.formName);
        this.childTab = this.currentTab.children[index];

        setTimeout(() => {
          this.matTabGroup.selectedIndex = index;
        }, 0);
      }

      const transformedTabs = this.currentTab.children.map(tab =>
        this.formDataService.transformTab(tab, decodedQueryParams)
      );

      this.visibleTabs = transformedTabs.filter(tab => tab.showTab);
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

        for (let key in queryParams) {
          if (queryParams[key] === null) {
            queryParams[key] = "null";
          }
        }

        environment.production && (queryParams = this.helperService.encodeParams(queryParams));
        this.router.navigate([`${path}`], { relativeTo: this.route, queryParams });
      }
    );
  }

  private initAddressSubsctiprion() {
    this.addressDataSubscription = this.helperService.addressDataGathered.subscribe(addressData => (this.addressData = addressData));
  }

  private initHistorySubscription() {
    this.historyDataSubscription = this.helperService.historyDataGathered.subscribe(res => {
      this.periodData = res.periodData;
      this.yearData = res.yearData;
    });
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.tableItemSelectedSubscription && this.tableItemSelectedSubscription.unsubscribe();
    this.paramSubsctiption && this.paramSubsctiption.unsubscribe();
    this.addressDataSubscription && this.addressDataSubscription.unsubscribe();
    this.historyDataSubscription && this.historyDataSubscription.unsubscribe();
  }

  onTabChanged(event) {
    const tab = this.currentTab.children[event.index];

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
      queryParams = {
        instid: queryParams.instid,
        sysroleid: queryParams.sysroleid,
        sysuserid: queryParams.sysuserid,
        address: queryParams.address,
        year: queryParams.year,
        period: queryParams.period,
        isLocked: this.formDataService.instIsLocked,
        schoolYear: this.formDataService.schoolYear,
        detailedSchoolType: this.formDataService.detailedSchoolType
      };
      environment.production && (queryParams = this.helperService.encodeParams(queryParams));
      sessionStorage.removeItem("list");
    }

    this.router.navigate([path], { relativeTo: this.route, queryParams });
  }
}
