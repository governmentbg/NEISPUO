import { Component, Input, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { FieldType } from "src/app/enums/fieldType.enum";
import { FormTypeInt } from "src/app/enums/formType.enum";
import { Mode, ModeInt } from "src/app/enums/mode.enum";
import { Tabs } from "src/app/enums/tabs.enum";
import { Form } from "src/app/models/form.interface";
import { Subsection } from "src/app/models/subsection.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../../environments/environment";

@Component({
  selector: "app-fast-preview",
  templateUrl: "./fast-preview.component.html",
  styleUrls: ["./fast-preview.component.scss"]
})
export class FastPreviewComponent implements OnInit {
  @Input() formName: string;
  @Input() columnNumber: number;

  isLoading: boolean = false;
  form: Form;
  formGroup: FormGroup;
  mode: Mode = Mode.View;
  instid: string;

  get type() {
    return FieldType;
  }

  constructor(
    private formDataService: FormDataService,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private authService: AuthService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;
    const params = this.route.snapshot.params || {};
    const body: any = { ...queryParams };

    this.isLoading = true;
    this.instid = queryParams.instid;

    for (const key in body) {
      if (body[key] === "null") {
        delete body[key];
      }
    }

    const instType = params.type ? FormTypeInt[params.type] + 1 : FormTypeInt[this.authService.getType()] + 1;
    const isHistory = params.tab === Tabs.history || queryParams.year !== undefined;

    this.formDataService
      .getFormData(this.formName, body, ModeInt.view, instType, isHistory, Mode.View, false)
      .subscribe(
        res => this.initForm(res),
        err => (this.isLoading = false)
      );
  }

  private initForm(form: Form) {
    this.form = form;
    this.formGroup = this.fb.group({});

    this.form.sections &&
      this.form.sections.forEach(section => {
        section.subsections &&
          section.subsections.forEach(subsection => {
            subsection.fields &&
              subsection.fields.forEach(field => {
                this.helperService.addControl(field, this.formGroup, false, false, false, false);
              });
          });
      });

    this.isLoading = false;
  }

  hasNonHiddenField(sub: Subsection) {
    return this.helperService.hasNonHiddenField(sub);
  }
}
