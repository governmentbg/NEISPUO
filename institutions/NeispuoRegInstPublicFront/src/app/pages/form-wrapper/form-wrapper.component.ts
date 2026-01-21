import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { FormType } from "../../enums/formType.enum";
import { Menu } from "../../enums/menu.enum";
import { Mode } from "../../enums/mode.enum";
import { Form } from "../../models/form.interface";
import { Option } from "../../models/option.interface";
import { FormDataService } from "../../services/form-data.service";
import { MessagesService } from "../../services/messages.service";
import { ModalComponent } from "../../shared/modal/modal.component";
import { CreateEditFormComponent } from "./create-edit-form/create-edit-form.component";
import { environment } from "../../../environments/environment";

@Component({
  selector: "app-form-wrapper",
  templateUrl: "./form-wrapper.component.html",
  styleUrls: ["./form-wrapper.component.scss"]
})
export class FormWrapperComponent implements OnInit, OnDestroy {
  form: CreateEditFormComponent;
  formData: Form;
  isLoading: boolean = false;
  isHistoryActive: boolean = false;
  history: Option[] = [];
  currentOption: number;
  pos: number;
  id: string | number;
  formType: FormType;
  mode: Mode;
  instKind: string;

  private routeSubscription: Subscription;

  @ViewChild("f", { static: false }) set f(form: CreateEditFormComponent) {
    if (form) {
      this.form = form;
      this.cdr.detectChanges();
    }
  }

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private formDataService: FormDataService,
    private dialog: MatDialog,
    private msgService: MessagesService
  ) {}

  ngOnInit() {
    this.mode = Mode.View;

    if (!this.msgService.modalQuestions || !this.msgService.successMessages) {
      this.isLoading = true;
      this.msgService.getMessages().subscribe(res => {
        this.isLoading = false;
        this.subscribe();
      });
    } else {
      this.subscribe();
    }
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
  }

  private subscribe() {
    this.routeSubscription = this.route.params.subscribe(params => {
      this.formType = params["formName"];
      this.id = params["id"];
      this.instKind = this.route.snapshot.queryParams["instKind"] || "";

      this.getFormDataFromServer();
      this.pos = 0;
    });
  }

  navigateAway(): Observable<boolean> {
    const dialogRef = this.dialog.open(ModalComponent, {
      width: "45%",
      data: { message: this.msgService.modalQuestions.unsavedChanges }
    });

    return dialogRef.afterClosed();
  }

  historyChanged(isHistoryActive: boolean) {
    this.isHistoryActive = isHistoryActive;

    if (this.history.length === 0) {
      this.isLoading = true;
      this.formDataService.getInstitutionHistory(<number>this.id).subscribe((res: Option[]) => {
        this.isLoading = false;
        this.history = res.reverse();
        this.currentOption = this.history[0].code;
        this.instKind = (this.history[0].instKind || this.instKind || "") + "";
        this.getFirstState();
      });
    } else {
      this.instKind = (this.history[0].instKind || this.instKind || "") + "";
      this.getFirstState();
    }
  }

  private getFirstState() {
    const prevOption = this.currentOption;
    this.currentOption = this.history[0].code;
    this.pos = 0;
    prevOption !== this.currentOption && this.getFormDataFromServer(this.currentOption);
  }

  private getFormDataFromServer(historyCode: number = null) {
    this.isLoading = true;
    this.formData = null;
    this.currentOption = +historyCode;

    const queryParams = this.route.snapshot.queryParams || {};
    const body: any = { instid: this.id };

    if (historyCode || queryParams["procID"]) {
      body["procID"] = historyCode || queryParams["procID"];
    }

    const isHistory = historyCode && historyCode != queryParams["procID"];

    this.formDataService.getFormData(this.formType, body, this.instKind, isHistory).subscribe(
      res => {
        this.formData = res;
        this.isLoading = false;
      },
      error => {
        !environment.production && console.log(error);
        this.isLoading = false;
        this.router.navigate(["/", Menu.Home]);
      }
    );
  }

  onVersionChanged(historyCode: number) {
    this.currentOption = historyCode;
    this.pos = this.history.findIndex(opt => opt.code === this.currentOption);
    this.instKind = (this.history[this.pos].instKind || this.instKind || "") + "";
    this.getFormDataFromServer(historyCode);
  }
}
