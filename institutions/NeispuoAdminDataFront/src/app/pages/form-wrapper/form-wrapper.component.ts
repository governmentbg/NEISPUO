import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { AuthService } from "src/app/auth/auth.service";
import { Menu } from "../../enums/menu.enum";
import { Mode, ModeInt } from "../../enums/mode.enum";
import { Form } from "../../models/form.interface";
import { FormDataService } from "../../services/form-data.service";
import { MessagesService } from "../../services/messages.service";
import { SnackbarService } from "../../services/snackbar.service";
import { ModalComponent } from "../../shared/modal/modal.component";
import { CreateEditFormComponent } from "./create-edit-form/create-edit-form.component";
import { environment } from "../../../environments/environment";
import { HelperService } from "src/app/services/helpers.service";

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
  formName: string = null;
  mode: Mode;
  canEdit: boolean;
  formGroup: FormGroup;
  disableButtons = false;
  dataInit: boolean = false;

  private routeSubscription: Subscription;
  private queryParamSubscription: Subscription;
  private dataTaken = false;

  @ViewChild("f", { static: false }) set f(form: CreateEditFormComponent) {
    if (form) {
      this.form = form;
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
    private helperService: HelperService
  ) {}

  async ngOnInit() {
    const parent = this.route.parent;
    const url = parent.snapshot.url.length > 0 ? parent.snapshot.url : parent.parent.snapshot.url;

    this.path = url[0].path;
    this.canEdit = this.path === Menu.Edit || this.path === Menu.Create;
    this.mode = this.canEdit ? Mode.Edit : Mode.View;

    let body = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    this.isLoading = true;

    let copyBody = environment.production ? this.helperService.encodeParams(body) : { ...body };
    this.router.navigate(["."], { relativeTo: this.route, queryParams: copyBody });

    this.init();
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.queryParamSubscription && this.queryParamSubscription.unsubscribe();
  }

  private init() {
    this.isLoading = false;
    this.subscribe();
  }

  private subscribe() {
    this.routeSubscription = this.route.params.subscribe(async params => {
      if (params.formName != this.formName) {
        const queryParams = { ...this.route.snapshot.queryParams };
        let body = environment.production ? this.helperService.decodeParams(queryParams["q"]) : { ...queryParams };
        environment.production && (body = this.helperService.encodeParams(body));
      }

      this.getData(this.route.snapshot.queryParams, params);
    });

    this.queryParamSubscription = this.route.queryParams.subscribe(async queryParams => {
      this.getData(queryParams, this.route.snapshot.params);
    });
  }

  private getData(queryParams, params) {
    if (environment.production) {
      queryParams = this.helperService.decodeParams(queryParams["q"]);
    }

    if (this.helperService.errorOccured) {
      this.dataTaken = false;
      this.helperService.errorOccured = false;
    }

    if (!this.dataTaken) {
      this.dataTaken = true;

      this.formName = params["formName"] || "soSubmissionData";

      this.getFormDataFromServer();
    }
  }

  submit(submissionData: { value: any; isFinal: boolean }) {
    this.isLoading = true;
    this.form.formGroup.markAsPristine();

    const operationType = this.path === Menu.Edit ? ModeInt.update : ModeInt.create;

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    for (const key in queryParams) {
      submissionData.value[key] = queryParams[key] == "null" ? null : queryParams[key];
    }

    let path = "/";
    let params = {};

    if (this.authService.getPrevUrlData()) {
      [path, params] = this.authService.getPrevUrlData();
      this.authService.removeUrl();
    }

    this.isLoading = true;
    this.disableButtons = true;

    const forceOperation = this.form.form.forceOperation || 0;
    const data = operationType === ModeInt.update ? { ...submissionData.value, forceOperation } : submissionData.value;

    this.formDataService
      .submitForm({
        data,
        procedureName: this.formData.procedureName,
        operationType
      })
      .subscribe(
        (res: any) => {
          res && (res = JSON.parse(res));
          if ((res && res.length && res[0].OperationResultType === 1) || (res && res.OperationResultType === 1)) {
            this.onSuccess(path, params);
          } else if (
            (res && res.length && res[0].OperationResultType === 0) ||
            (res && res.OperationResultType === 0)
          ) {
            this.formDataService.getMessages().subscribe(messages => {
              const index = res.MessageCode || res[0].MessageCode;
              this.confirmUpdate(data, this.formData.procedureName, operationType, messages[index], path, params);
            });
          } else if (
            (res && res.length && res[0].OperationResultType === 2) ||
            (res && res.OperationResultType === 2)
          ) {
            this.formDataService.getMessages().subscribe(messages => {
              const index = res.MessageCode || res[0].MessageCode;
              this.isLoading = false;
              this.snackbarService.openErrorSnackbar(messages[index]);
            });
          } else {
            this.onSuccess(path, params);
          }
        },
        errorCode => {
          if (errorCode !== -1) {
            this.formDataService.getMessages().subscribe(messages => {
              this.snackbarService.openErrorSnackbar(messages[errorCode]);
              this.isLoading = false;
            });
          }
        }
      );
  }

  private successfulSubmit(path, params) {
    this.isLoading = false;
    setTimeout(() => {
      this.disableButtons = true;
      this.router.navigate([path], { queryParams: params }).then(res => {
        if (res) this.snackbarService.openSuccessSnackbar(this.msgService.successMessages.saveSuccess);
      });
    }, 500);
  }

  private onSuccess(path, params) {
    this.successfulSubmit(path, params);
    this.isLoading = false;
  }

  private confirmUpdate(
    data,
    procedureName: string,
    operationType: number,
    message: string,
    path: string,
    params: Object
  ) {
    this.isLoading = false;
    const dialogRef = this.dialog.open(ModalComponent, {
      width: "45%",
      data: {
        message,
        confirmBtnLbl: "Да",
        cancelBtnLbl: "Не"
      }
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.isLoading = true;
        data.forceOperation = 1;
        this.formDataService.submitForm({ data, procedureName, operationType }).subscribe(
          (res: any) => {
            res && (res = JSON.parse(res));
            if ((res && res.length && res[0].OperationResultType === 1) || (res && res.OperationResultType === 1)) {
              this.onSuccess(path, params);
            }

            this.isLoading = false;
          },
          errorCode => {
            this.formDataService.getMessages().subscribe(messages => {
              this.snackbarService.openErrorSnackbar(messages[errorCode]);
              this.isLoading = false;
            });
          }
        );
      }
    });
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

  getFormDataFromServer() {
    this.isLoading = true;
    this.formData = null;
    this.dataInit = false;
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;
    const body: any = { ...queryParams };

    for (const key in body) {
      if (body[key] === "null") {
        delete body[key];
      }
    }

    let operationType = ModeInt.view;

    if (this.canEdit) {
      this.path === Menu.Edit ? (operationType = ModeInt.update) : (operationType = ModeInt.create);
      queryParams["operationType"] && (operationType = queryParams["operationType"]);
    }

    this.formDataHelper(body, operationType);
  }

  private formDataHelper(body, operationType) {
    this.formDataService.getFormData(this.formName, body, operationType, this.mode).subscribe(
      res => {
        this.formData = res;
        this.isLoading = false;
        this.dataTaken = false;
        this.dataInit = true;
      },
      error => {
        this.isLoading = false;
        this.dataTaken = false;
        this.dataInit = true;
        this.helperService.routeParamChanged.next({ paramName: "type", paramValue: "" });
        this.helperService.errorOccured = true;
        this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.error);

        if (!environment.production) {
          console.log(error);
        }
      }
    );
  }
}
