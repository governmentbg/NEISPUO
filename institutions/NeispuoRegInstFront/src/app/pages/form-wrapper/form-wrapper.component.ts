import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { AuthService } from "../../auth/auth.service";
import { FormType, FormTypeInt } from "../../enums/formType.enum";
import { Menu } from "../../enums/menu.enum";
import { Mode, ModeInt } from "../../enums/mode.enum";
import { Form } from "../../models/form.interface";
import { Option } from "../../models/option.interface";
import { Subsection } from "../../models/subsection.interface";
import { FormDataService } from "../../services/form-data.service";
import { MessagesService } from "../../services/messages.service";
import { SnackbarService } from "../../services/snackbar.service";
import { ModalComponent } from "../../shared/modal/modal.component";
import { CreateEditFormComponent } from "./create-edit-form/create-edit-form.component";
import { environment } from "../../../environments/environment";
import { AzureService } from "src/app/services/azure.service";

@Component({
  selector: "app-form-wrapper",
  templateUrl: "./form-wrapper.component.html",
  styleUrls: ["./form-wrapper.component.scss"]
})
export class FormWrapperComponent implements OnInit, OnDestroy {
  form: CreateEditFormComponent;
  formData: Form;
  validate: boolean;
  isLoading: boolean = false;
  path: string;
  isHistoryActive: boolean = false;
  history: Option[] = [];
  currentOption: number;
  pos: number;
  id: string | number;
  formType: FormType;
  mode: Mode;
  canEdit: boolean;
  dataName: string;
  instKind: string;
  certificateSubsection: Subsection;
  certificateSubsectionFormGroup: FormGroup;
  showControlComponent: boolean = false;
  formGroup: FormGroup;
  procID: string | number;
  permissions;
  hideControls = false;
  disableButtons = false;

  private prevId: string | number;
  private routeSubscription: Subscription;
  private queryParamSubscription: Subscription;
  private dataTaken = false;

  @ViewChild("f", { static: false }) set f(form: CreateEditFormComponent) {
    if (form) {
      this.form = form;

      if (this.certificateSubsection) {
        this.certificateSubsectionFormGroup = this.formDataService.getFormGroup(this.certificateSubsection, this.form.formGroup);
      }

      this.cdr.detectChanges();
    }
  }

  get menu() {
    return Menu;
  }

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private formDataService: FormDataService,
    private dialog: MatDialog,
    private snackbarService: SnackbarService,
    private msgService: MessagesService,
    private authService: AuthService,
    private azureService: AzureService
  ) {}

  ngOnInit() {
    const parent = this.route.parent;
    const url = parent.snapshot.url.length > 0 ? parent.snapshot.url : parent.parent.snapshot.url;

    this.path = url[0].path;
    this.canEdit = this.path === Menu.EditProcedure || this.path === Menu.CreateProcedure;
    this.mode = this.canEdit ? Mode.Edit : Mode.View;

    this.subscribe();
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.queryParamSubscription && this.queryParamSubscription.unsubscribe();
  }

  private subscribe() {
    this.queryParamSubscription = this.route.queryParams.subscribe(queryParams => {
      this.subscribeTo(queryParams, this.route.snapshot.params);
    });

    this.routeSubscription = this.route.params.subscribe(params => {
      this.subscribeTo(this.route.snapshot.queryParams, params);
    });
  }

  private subscribeTo(queryParams, params) {
    if (!this.dataTaken) {
      this.dataTaken = true;
      if (environment.production) {
        queryParams = this.formDataService.decodeParams(queryParams["q"] || "");
      }

      this.prevId = this.id;
      this.formType = params["formName"];
      this.instKind = queryParams["instKind"] || "";
      this.id = queryParams["instid"];
      this.procID = queryParams["procID"];
      this.isHistoryActive = false;

      this.id &&
        this.formDataService.routeParamChanged.next({
          paramName: "id",
          paramValue: this.id
        });

      this.hideControls = true;
      this.getFormDataFromServer(null);
    }
  }

  submit(res: { value: any; isFinal: boolean }) {
    this.form.formGroup.markAsPristine();

    const queryParams = environment.production
      ? this.formDataService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    let operationType = this.path === Menu.EditProcedure ? ModeInt.update : ModeInt.create;

    operationType = queryParams.operationType ? +queryParams.operationType : operationType;

    this.isLoading = true;
    this.formDataService
      .submitForm({
        data: res.value,
        procedureName: this.formData.procedureName,
        operationType
      })
      .subscribe(
        (result: any) => {
          this.isLoading = false;
          result && (result = JSON.parse(result));

          if (result && result.length && result[0].schoolCreate) {
            const body = { institutionID: +result[0].codeNEISPUO };

            this.azureService.createSchool(body).subscribe(
              (createRes: any) =>
                createRes.status === 200 || createRes.status === 201
                  ? this.onSuccess()
                  : this.onAzureError(queryParams, operationType, createRes, body),
              (error: { status: string; message: string }) => this.onAzureError(queryParams, operationType, error, body)
            );
          } else if (result && result.length && result[0].codeNEISPUODeleted) {
            const body = { institutionID: +result[0].codeNEISPUODeleted };

            this.azureService.deleteSchool(body).subscribe(
              (deleteRes: any) =>
                deleteRes.status === 200 || deleteRes.status === 201
                  ? this.onSuccess()
                  : this.onAzureError(queryParams, operationType, deleteRes, body),
              (error: { status: string; message: string }) => this.onAzureError(queryParams, operationType, error, body)
            );
          } else {
            this.onSuccess();
          }
        },
        error => {
          this.isLoading = false;
          this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.error);
        }
      );
  }

  private onSuccess() {
    this.isLoading = false;
    this.snackbarService.openSuccessSnackbar(this.msgService.successMessages.saveSuccess);
    setTimeout(() => {
      this.disableButtons = true;
      this.router.navigate(["/", Menu.Home]);
    }, 500);
    this.isLoading = false;
  }

  private onAzureError(queryParams, operationType, error, payload) {
    const body = {
      data: {
        sysuserid: queryParams.sysuserid,
        region: queryParams.region,
        instid: queryParams.instid,
        error_number: error.status,
        error_message: error.message,
        error_procedure: environment.azureUrl + "/v1/azure-integrations/school/create",
        operationType,
        payload: JSON.stringify(payload)
      },
      procedureName: "logs.azureErrorLog",
      operationType: ModeInt.create
    };

    this.formDataService.submitForm(body).subscribe(
      res => this.onSuccess(),
      err => this.onSuccess()
    );
  }

  navigateAway(): Observable<boolean> {
    const dialogRef = this.dialog.open(ModalComponent, {
      width: "45%",
      data: {
        message: this.msgService.modalQuestions.unsavedChanges,
        confirmBtnLbl: "Да",
        cancelBtnLbl: "Не"
      }
    });

    return dialogRef.afterClosed();
  }

  historyChanged(isHistoryActive: boolean) {
    const prevHistoryMode = this.isHistoryActive;
    this.isHistoryActive = isHistoryActive;

    if (this.prevId != this.id && isHistoryActive) {
      this.isLoading = true;
      this.prevId = this.id;
      this.formDataService.getInstitutionHistory(this.id).subscribe(
        (res: any) => {
          this.isLoading = false;
          this.history = res.length ? res.reverse() : [];
          this.currentOption = res.length ? +this.history[0].code : +this.procID;
          this.instKind = (this.history[0].instKind || this.instKind || "") + "";
          this.getCurrentState();
        },
        err => (this.isLoading = false)
      );
    } else if (this.history.length > 0 && this.isHistoryActive !== prevHistoryMode && this.currentOption !== +this.history[0].code) {
      this.instKind = (this.history[0].instKind || this.instKind || "") + "";
      this.getCurrentState();
    }
  }

  private getCurrentState() {
    const prevOption = this.currentOption;
    this.currentOption = this.history.length ? +this.history[0].code : +this.procID;

    this.pos = 0;
    prevOption !== this.currentOption && this.getFormDataFromServer(this.currentOption);
  }

  private getFormDataFromServer(historyCode: number | string) {
    this.isLoading = true;
    this.formData = null;
    let queryParams = environment.production
      ? this.formDataService.decodeParams(this.route.snapshot.queryParams["q"] || "")
      : this.route.snapshot.queryParams;

    const params = this.route.snapshot.params || {};
    const body: any = { instid: this.id, ...queryParams };

    if (historyCode || queryParams["procID"]) {
      body.procID = historyCode || queryParams["procID"];
    }

    let operationType = historyCode && historyCode != queryParams["procID"] ? ModeInt.history : ModeInt.view;

    const instType = params.type ? FormTypeInt[params.type] + 1 : queryParams["instType"];

    if (this.canEdit) {
      this.path === Menu.EditProcedure ? (operationType = ModeInt.update) : (operationType = ModeInt.create);
      queryParams["operationType"] && (operationType = queryParams["operationType"]);
    }

    const isMon = this.authService.isMon();

    if (this.path === Menu.Active || this.path === Menu.Inactive) {
      this.formDataService.getEditPermissions(isMon).subscribe(
        permissions => {
          this.permissions = permissions;
          this.getFormData(body, operationType, instType);
        },
        err => (this.isLoading = false)
      );
    } else {
      this.getFormData(body, operationType, instType);
    }
  }

  private getFormData(body, operationType, instType) {
    this.formDataService.getFormData(this.formType, body, operationType, this.instKind, instType).subscribe(
      res => {
        this.formData = res;
        this.dataName = this.formData.dataName;
        this.findCertificateSubsection();
        this.isLoading = false;
        this.hideControls = false;

        this.dataTaken = false;
      },
      error => {
        !environment.production && console.log(error);
        this.isLoading = false;
        this.hideControls = false;
        this.router.navigate(["/", Menu.Home]);
        this.dataTaken = false;
      }
    );
  }

  private findCertificateSubsection() {
    this.certificateSubsection = null;
    for (const section of this.formData.sections) {
      for (const subsection of section.subsections) {
        if (subsection.canHaveCertificate) {
          this.certificateSubsection = subsection;
          break;
        } else {
          const sub = subsection.subsections.find(s => s.canHaveCertificate);
          sub && (this.certificateSubsection = sub);
        }
      }

      if (this.certificateSubsection) {
        break;
      }
    }

    this.showControlComponent = true;
  }

  onVersionChanged(historyCode: number) {
    this.currentOption = historyCode;
    this.pos = this.history.findIndex(opt => opt.code === this.currentOption);
    this.instKind = (this.history[this.pos].instKind || this.instKind || "") + "";

    this.getFormDataFromServer(historyCode);
  }

  certificateChange(values) {
    if (values) {
      this.getFormDataFromServer(null);
    }
  }
}
