import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { environment } from "src/environments/environment";
import { Menu } from "../../enums/menu.enum";
import { Mode, ModeInt } from "../../enums/mode.enum";
import { Form } from "../../models/form.interface";
import { FormDataService } from "../../services/form-data.service";

@Component({
  selector: "app-home-screen",
  templateUrl: "./home-screen.component.html",
  styleUrls: ["./home-screen.component.scss"]
})
export class HomeScreenComponent implements OnInit {
  form: Form;
  formGroup: FormGroup;
  isLoading = true;
  canCreateNew: boolean;

  get modes() {
    return Mode;
  }

  constructor(
    private formDataService: FormDataService,
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.canCreateNew = !this.authService.isExpert();
    this.getTable();
  }

  private getTable() {
    this.isLoading = true;
    this.formDataService
      .getFormData("home", { sysuserid: this.authService.getSysUserId(), region: this.authService.getRegion() }, ModeInt.view, null, null)
      .subscribe(
        res => {
          this.isLoading = false;
          this.form = res;
          this.formGroup = this.fb.group({});
        },
        err => {
          this.isLoading = false;
        }
      );
  }

  createNew() {
    let queryParams: any = {
      sysuserid: this.authService.getSysUserId(),
      region: this.authService.getRegion()
    };

    environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

    this.router.navigate(["/", Menu.NewInstitution], { queryParams });
  }
}
