import { Component, OnInit, ViewChild } from "@angular/core";
import { MatTabGroup } from "@angular/material/tabs";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { FormTypeInt } from "src/app/enums/formType.enum";
import { Menu } from "src/app/enums/menu.enum";
import { ModeInt } from "src/app/enums/mode.enum";
import { responseType } from "src/app/enums/responseType";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-sample-list",
  templateUrl: "./sample-list.component.html",
  styleUrls: ["./sample-list.component.scss"]
})
export class SampleListComponent implements OnInit {
  @ViewChild("matTabGroup") matTabGroup: MatTabGroup;

  isLoading: boolean = false;
  tabIndex: number = 0;
  tabs: string[] = [];
  canSign: boolean;
  infoLs: { label: string; text: string; list: string; resultType?: responseType }[];

  constructor(
    private authService: AuthService,
    private formDataService: FormDataService,
    private helperService: HelperService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  async ngOnInit() {
    this.tabIndex = this.route.snapshot.params.menuItem !== "sign" ? 0 : 1;

    this.tabs = [...this.formDataService.sampleListData];

    if (this.tabIndex === 1) {
      let queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : { ...this.route.snapshot.queryParams };
      const res: any = await this.formDataService.isOpenCampaign(queryParams.instid).toPromise();
      queryParams.isOpenCampaign = res && res.length ? res[0].isOpen : res.isOpen;
      environment.production && (queryParams = this.helperService.encodeParams(queryParams));

      this.router.navigate(["."], { relativeTo: this.route, queryParams });
    }

    setTimeout(() => {
      this.matTabGroup.selectedIndex = this.tabIndex;
    }, 0);
  }

  private getBody() {
    const body = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };
    const params = this.route.snapshot.params;
    const instType = params.type ? FormTypeInt[params.type] + 1 : FormTypeInt[this.authService.getType()] + 1;
    body.instType = instType;
    return body;
  }

  onGetListView() {
    // to avoid changed after checked when list-view query param checked is active
    setTimeout(() => {
      this.isLoading = true;
      const body = this.getBody();

      this.formDataService
        .submitForm({
          data: { instid: body.instid },
          operationType: ModeInt.update,
          procedureName: "sovalidity.dataValidityCheck"
        })
        .subscribe(
          (res: any) => {
            try {
              res && (res = JSON.parse(res));
            } catch (err) {}

            if (res && res.length) {
              if (res[0].hasResult) {
                this.formDataService.getListViewData(body).subscribe(
                  (res: { label: string; text: string; list: string; resultType?: responseType }[]) => {
                    this.infoLs = res;
                    this.isLoading = false;
                  },
                  err => (this.isLoading = false)
                );
              } else {
                this.infoLs = [];
                this.isLoading = false;
              }
            } else {
              this.infoLs = [];
              this.isLoading = false;
            }
          },
          err => (this.isLoading = false)
        );
    }, 0);
  }

  async onTabChange(event) {
    const index = event.index;

    if (index !== this.tabIndex) {
      const type = this.route.snapshot.params.type;
      const tabName = index ? "sign" : "validate";

      let queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams.q)
        : { ...this.route.snapshot.queryParams };
      queryParams.check = null;

      const res: any = await this.formDataService.isOpenCampaign(queryParams.instid).toPromise();
      queryParams.isOpenCampaign = res && res.length ? res[0].isOpen : res.isOpen;

      environment.production && (queryParams = this.helperService.encodeParams(queryParams));

      this.router.navigate([`/${Menu.Home}/${type}/list/${tabName}`], { queryParams }).then(res => {
        if (res) {
          this.tabIndex = index;
        }
      });
    }
  }
}
