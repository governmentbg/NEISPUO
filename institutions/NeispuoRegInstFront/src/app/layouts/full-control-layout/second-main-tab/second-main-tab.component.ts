import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { FormDataService } from "src/app/services/form-data.service";
import { environment } from "src/environments/environment";
import { FormTypeInt } from "../../../enums/formType.enum";

@Component({
  selector: "app-second-main-tab",
  templateUrl: "./second-main-tab.component.html",
  styleUrls: ["./second-main-tab.component.scss"]
})
export class SecondMainTabComponent implements OnInit {
  tabIndex;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private formDataService: FormDataService
  ) {}

  ngOnInit() {
    this.tabIndex = FormTypeInt[this.route.snapshot.params["type"]];
  }

  onPathChanged(index: number) {
    if (index !== this.tabIndex) {
      let path = `../${FormTypeInt[index]}`;

      let queryParams: any = {
        sysuserid: this.authService.getSysUserId(),
        region: this.authService.getRegion()
      };

      environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

      this.router.navigate([path], { relativeTo: this.route, queryParams }).then(res => {
        if (res) {
          this.tabIndex = index;
        }
      });
    }
  }
}
