import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { Menu } from "../../../../enums/menu.enum";
import { HelperService } from "src/app/services/helpers.service";
import { FormDataService } from "src/app/services/form-data.service";

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

  constructor(private router: Router, private helperService: HelperService, private formDataService: FormDataService) {}

  ngOnInit() {
    const path = this.getPath();
    this.tabIndex = path === Menu.Settings ? 1 : 0;

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
      await this.getTab(index);
    }
  }

  private async getTab(index: number) {
    this.isMonRuo && index !== 4 && (this.formDataService.mainMenuData = null);

    if (index === 0) {
      this.isLoading = true;

      this.router.navigate([Menu.Home]).then(res => {
        if (res) {
          this.tabIndex = index;
        }
      });
    } else {
      this.router.navigate([Menu.Settings]).then(res => {
        if (res) {
          this.tabIndex = index;
        }
      });
    }
  }

  private getPath(): string {
    const urlParts = this.router.url.split(/\/|\?/);

    let res = urlParts && urlParts.length > 1 ? urlParts[1] : "";

    return res;
  }
}
