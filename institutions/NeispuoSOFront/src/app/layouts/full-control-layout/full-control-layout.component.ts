import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Subscription } from "rxjs";
import { Tabs } from "src/app/enums/tabs.enum";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-full-control-layout",
  templateUrl: "./full-control-layout.component.html",
  styleUrls: ["./full-control-layout.component.scss"]
})
export class FullControlLayoutComponent implements OnInit, OnDestroy {
  isHistory: boolean = false;
  currentTab: string;
  isLoading: boolean;

  private routeSubscription: Subscription;
  private queryParamSubscription: Subscription;

  constructor(
    private route: ActivatedRoute,
    private helperService: HelperService,
    private formDataService: FormDataService,
    private router: Router
  ) {}

  async ngOnInit() {
    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams.q)
      : { ...this.route.snapshot.queryParams };

    if (queryParams.instid) {
      this.isLoading = true;
      let res: any = await this.formDataService.getIsLocked(queryParams.instid).toPromise();
      res && res.length && (res = res[0]);
      this.formDataService.instIsLocked = !!res.isLocked;
      queryParams.isLocked = this.formDataService.instIsLocked;

      let detailedSchoolTypeRes: any = await this.formDataService.getDetailedSchoolType(queryParams.instid).toPromise();
      detailedSchoolTypeRes && detailedSchoolTypeRes.length && (detailedSchoolTypeRes = detailedSchoolTypeRes[0]);
      this.formDataService.detailedSchoolType = detailedSchoolTypeRes.detailedSchoolType;
      queryParams.detailedSchoolType = this.formDataService.detailedSchoolType;

      if (!this.formDataService.schoolYear) {
        let schoolYearRes: any = await this.formDataService.getSchoolYear(queryParams.instid).toPromise();
        schoolYearRes && schoolYearRes.length && (schoolYearRes = schoolYearRes[0]);
        this.formDataService.schoolYear = schoolYearRes.schoolYear;
      }

      queryParams.schoolYear = this.formDataService.schoolYear;

      environment.production && (queryParams = this.helperService.encodeParams(queryParams));
      this.router.navigate([this.router.url.split("?")[0]], { queryParams });
      this.isLoading = false;
    }

    this.routeSubscription = this.helperService.routeParamChanged.subscribe((paramChange: { paramName: string; paramValue: string }) => {
      setTimeout(() => {
        if (paramChange.paramName === "tab") {
          const queryParams = environment.production
            ? this.helperService.decodeParams(this.route.snapshot.queryParams.q)
            : { ...this.route.snapshot.queryParams };

          this.currentTab = paramChange.paramValue;
          this.isHistory = this.currentTab === Tabs.history || queryParams.year !== undefined;
        }
      }, 0);
    });

    this.queryParamSubscription = this.route.queryParams.subscribe(queryParams => {
      setTimeout(() => {
        queryParams = environment.production
          ? this.helperService.decodeParams(queryParams.q)
          : { ...queryParams };

        this.isHistory = this.currentTab === Tabs.history || queryParams.year !== undefined;
      }, 0);
    });
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.queryParamSubscription && this.queryParamSubscription.unsubscribe();
  }
}
