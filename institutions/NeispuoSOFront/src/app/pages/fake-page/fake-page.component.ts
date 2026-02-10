import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { forkJoin, Subscribable } from "rxjs";
import { AuthService } from "src/app/auth/auth.service";
import { FormDataService } from "src/app/services/form-data.service";

@Component({
  selector: "app-fake-page",
  templateUrl: "./fake-page.component.html",
  styleUrls: ["./fake-page.component.scss"]
})
export class FakePageComponent implements OnInit {
  isMonRuo: boolean = false;
  isLoading: boolean;

  constructor(
    private authService: AuthService,
    private formDataService: FormDataService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.isMonRuo = this.authService.isMon() || this.authService.isRuo();

    if (!this.formDataService.mainMenuData) {
      this.isLoading = true;
      let type = this.route.snapshot.params.type ? this.route.snapshot.params.type : this.authService.getType();

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
      });
    }
  }
}
