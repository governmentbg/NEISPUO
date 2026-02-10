import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { FormTypeInt } from "src/app/enums/formType.enum";
import { Mode, ModeInt } from "src/app/enums/mode.enum";
import { Form } from "src/app/models/form.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-new-staff-position",
  templateUrl: "./new-staff-position.component.html",
  styleUrls: ["./new-staff-position.component.scss"]
})
export class NewStaffPositionComponent implements OnInit {
  isLoading: boolean = false;
  positionName: string;
  data: Form;
  formGroup: FormGroup;
  message: string = "";
  dataTaken: boolean = false;

  get modes() {
    return Mode;
  }

  constructor(
    private formDataService: FormDataService,
    private router: Router,
    private authService: AuthService,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {}

  getData() {
    this.dataTaken = true;
    this.message = "";
    this.data = null;
    this.isLoading = true;
    this.positionName = this.positionName.trim();
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    const body = {
      data: { positionName: this.positionName, instid: queryParams.instid },
      procedureName: "inst_basic.CheckNKPDPositionInstitution",
      operationType: 2
    };

    this.formDataService.performProcedure(body).subscribe(
      async (res: any) => {
        try {
          res && (res = JSON.parse(res));
        } catch (err) {}

        await this.getMessage(res);

        let status;

        if (res) {
          if (res.length) {
            status = res[0].status;
          } else {
            status = res.status;
          }
        }

        if (status !== 1 && status !== 3) {
          let operationType = ModeInt.view;
          const instType = FormTypeInt[this.authService.getType()] + 1;

          const data: any = await this.formDataService.fillData(operationType, instType, res, "nkpdFoundInstitution").toPromise();
          this.formGroup = this.fb.group({});
          this.data = data;
        }

        this.isLoading = false;
      },
      () => (this.isLoading = false)
    );
  }

  private async getMessage(res) {
    const messageCode = res.length ? res[0].messageCode : res.messageCode;

    if (messageCode) {
      const messages = await this.formDataService.getMessages().toPromise();
      this.message = messages[messageCode];
    }
  }

  goBack() {
    let path = "/";
    let queryParams = {};

    if (this.authService.getPrevUrlData()) {
      [path, queryParams] = this.authService.getPrevUrlData();
      this.authService.removeUrl();
    }

    this.router.navigate([path], { queryParams });
  }
}
