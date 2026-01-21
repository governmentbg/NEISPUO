import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { AuthService } from "src/app/auth/auth.service";
import { FormTypeInt } from "../../enums/formType.enum";
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
import { Tabs } from "src/app/enums/tabs.enum";
import { Option } from "src/app/models/option.interface";
import { AzureService } from "src/app/services/azure.service";
import { ElectronicSignatureService } from "src/app/services/electronic-signature.service";
import { FileService } from "src/app/services/file.service";
import * as html2pdf from "html2pdf.js";
import { FieldConfig, RegixData } from "src/app/models/field.interface";
import { RegixService } from "../../services/regix.service";

declare var x509;

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
  id: string | number;
  formName: string = null;
  mode: Mode;
  canEdit: boolean;
  disableButtons = false;
  isLocked: boolean;
  dataInit: boolean = false;

  submissionPeriod: { isOpenPeriod: boolean; period: number; schoolYear: number };

  private hasAddress: boolean;
  private hasPeriodFilters: boolean;
  private addressData: Option[] = null;
  private yearData: Option[] = null;
  private periodData: Option[] = null;
  private currentTab: string = null;

  private routeSubscription: Subscription;
  private queryParamSubscription: Subscription;
  private dataTaken = false;
  private addressTaken = false;
  private periodTaken = false;

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
    private helperService: HelperService,
    private azureService: AzureService,
    private electronicSignatureService: ElectronicSignatureService,
    private fileService: FileService,
    private regixService: RegixService
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

    this.isLocked = body.isLocked === "true" && this.route.snapshot.params.tab;

    const params = this.route.snapshot.params;
    this.hasAddress = params.tab === Tabs.physical && params.menuItem === this.formDataService.physicalMenuData[0].path;
    this.hasPeriodFilters = params.tab === Tabs.history;

    this.isLoading = true;

    const res: any = await this.formDataService.getExtData(body.instid).toPromise();
    body.extdata = res && res.length ? res[0].extdata : null;
    body.extAlldata = res && res.length ? res[0].extAlldata : null;

    if (!this.formDataService.schoolYear) {
      let schoolYearRes: any = await this.formDataService.getSchoolYear(body.instid).toPromise();
      schoolYearRes && schoolYearRes.length && (schoolYearRes = schoolYearRes[0]);
      this.formDataService.schoolYear = schoolYearRes.schoolYear;
    }

    body.schoolYear = this.formDataService.schoolYear;

    let copyBody = environment.production ? this.helperService.encodeParams(body) : { ...body };

    if (this.route.snapshot.queryParams.extdata != body.extdata || body.extAlldata != this.route.snapshot.queryParams.extAlldata) {
      this.router.navigate(["."], { relativeTo: this.route, queryParams: copyBody }).then(() => this.init());
    } else {
      this.init();
    }
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
      if (this.currentTab !== params.tab) {
        this.addressData = null;
        this.yearData = null;
        this.periodData = null;
        this.currentTab = params.tab;
      }

      if (this.formName && this.formName !== "personData") {
        this.authService.removeRegixData();
      }

      if (params.formName != this.formName) {
        const queryParams = { ...this.route.snapshot.queryParams };
        let body = environment.production ? this.helperService.decodeParams(queryParams["q"]) : { ...queryParams };
        const res: any = await this.formDataService.getExtData(body.instid).toPromise();
        body.extdata = res && res.length ? res[0].extdata : null;
        body.extAlldata = res && res.length ? res[0].extAlldata : null;
        environment.production && (body = this.helperService.encodeParams(body));

        if (queryParams.extdata != body.extdata || body.extAlldata != queryParams.extAlldata) {
          this.router
            .navigate(["."], { relativeTo: this.route, queryParams: body })
            .then(() => this.getData(this.route.snapshot.queryParams, params));
        } else {
          this.getData(this.route.snapshot.queryParams, params);
        }
      }
    });

    this.queryParamSubscription = this.route.queryParams.subscribe(async queryParams => {
      let body = environment.production ? this.helperService.decodeParams(queryParams["q"]) : { ...queryParams };

      if (this.id !== body.instid) {
        this.addressData = null;
        this.yearData = null;
        this.periodData = null;
        body = { ...body, address: null, period: null, year: null };
        this.id = body.instid;
      }

      if (body.extdata === undefined) {
        const res: any = await this.formDataService.getExtData(body.instid).toPromise();
        body.extdata = res && res.length ? res[0].extdata : null;
        body.extAlldata = res && res.length ? res[0].extAlldata : null;
        environment.production && (body = this.helperService.encodeParams(body));
        this.router
          .navigate(["."], { relativeTo: this.route, queryParams: body })
          .then(() => this.getData(queryParams, this.route.snapshot.params));
      } else {
        this.getData(queryParams, this.route.snapshot.params);
      }
    });
  }

  private getData(queryParams, params) {
    this.hasAddress = params.tab === Tabs.physical && params.menuItem === this.formDataService.physicalMenuData[0].path;
    this.hasPeriodFilters = params.tab === Tabs.history;

    if (environment.production) {
      queryParams = this.helperService.decodeParams(queryParams["q"]);
    }

    this.isLocked = (queryParams.isLocked === "true" || this.isExtDataLocked(params, queryParams.extdata)) && !!params.tab;

    if (this.hasAddress && !this.addressTaken && !this.addressData) {
      this.addressTaken = true;
      this.formDataService.getAddressData(queryParams).subscribe((addressData: Option[]) => {
        this.addressData = addressData;
        this.addressData && this.addressData.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
        this.helperService.addressDataGathered.next(this.addressData);
        this.addressTaken = false;
      });
    } else if (this.hasPeriodFilters && !this.periodTaken && (!this.periodData || !this.yearData)) {
      this.periodTaken = true;
      this.formDataService.getPeriodData(queryParams).subscribe((periodData: Option[]) => {
        this.periodData = periodData;
        this.periodData && this.periodData.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
        this.formDataService.getYearData(queryParams).subscribe((yearData: Option[]) => {
          this.yearData = yearData;
          this.yearData && this.yearData.forEach(option => option.isValid !== undefined && (option.disabled = !option.isValid));
          this.helperService.historyDataGathered.next({ periodData: this.periodData, yearData: this.yearData });
          this.periodTaken = false;
        });
      });
    }

    if (this.helperService.errorOccured) {
      this.dataTaken = false;
      this.helperService.errorOccured = false;
    }

    // dataTaken -> if change in both params and query params exist, data should not be taken twice, but two events are fired by subscribes
    if (!this.dataTaken) {
      if (
        (!this.hasAddress && !this.hasPeriodFilters) ||
        (this.hasAddress && queryParams.address !== undefined) ||
        (this.hasPeriodFilters && queryParams.year !== undefined && queryParams.period !== undefined)
      ) {
        this.dataTaken = true;

        this.formName = params["formName"] || "soSubmissionData";
        this.id = queryParams.instid;

        this.getFormDataFromServer(queryParams);
      } else {
        this.dataTaken = true;
        this.formData = null;

        setTimeout(() => {
          this.dataTaken = false;
        }, 100);
      }
    }
  }

  private isExtDataLocked(params, extdata) {
    if (this.canEdit) {
      return false;
    }

    if (this.path === Menu.Preview) {
      return true;
    }

    let gp;
    let parent;
    let current;
    const menu = params.tab === Tabs.physical ? this.formDataService.physicalMenuData : this.formDataService.mainMenuData;
    const main = menu.find(tab => tab.path === params.menuItem);

    if (params.grandParentForm) {
      gp = main?.children.find(tab => tab.path === params.grandParentForm);
      parent = gp?.children.find(tab => tab.path === params.parentForm);
      current = parent?.children.find(item => item.path === params.formName);
    } else if (params.parentForm) {
      parent = main?.children.find(tab => tab.path === params.parentForm);
      current = parent?.children.find(item => item.path === params.formName);
    } else {
      current = main?.children.find(item => item.path === params.formName);
    }

    return !!(extdata == 1 && current?.extData);
  }

  submit(submissionData: { value: any; isFinal: boolean }) {
    this.isLoading = true;
    this.form.formGroup.markAsPristine();

    const operationType = this.path === Menu.Edit ? ModeInt.update : ModeInt.create;

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;

    for (const key in queryParams) {
      if (submissionData.value[key] === undefined) {
        submissionData.value[key] = queryParams[key] == "null" ? null : queryParams[key];
      }
    }

    let path = "/";
    let params = {};

    if (this.authService.getPrevUrlData()) {
      [path, params] = this.authService.getPrevUrlData();
    }

    this.disableButtons = true;

    const forceOperation = this.form.form.forceOperation || 0;
    const data = operationType === ModeInt.update ? this.convertLists({ ...submissionData.value, forceOperation }) : submissionData.value;

    // pass forceOperation to create of changeYearData
    if (this.formName === "changeYearData") {
      data.forceOperation = forceOperation;
    }

    this.formDataService
      .submitForm({
        data,
        procedureName: this.formData.procedureName,
        operationType
      })
      .subscribe(
        async (res: any) => {
          res && (res = JSON.parse(res));
          const formName = this.route.snapshot.params.formName;

          if (formName === "changeYearData") {
            let schoolYearRes: any = await this.formDataService.getSchoolYear(queryParams.instid).toPromise();
            schoolYearRes && schoolYearRes.length && (schoolYearRes = schoolYearRes[0]);
            this.formDataService.schoolYear = schoolYearRes.schoolYear;
          }

          if ((res && res.length && res[0].OperationResultType === 1) || (res && res.OperationResultType === 1)) {
            if (
              [
                "curriculumSubject",
                "personData",
                "person",
                "studentMainClassGroup",
                "studentOtherClassGroup",
                "staffPositionData",
                "teachingNormData",
                "changeYearData",
                "classGroupList"
              ].includes(formName)
            ) {
              const result = res && res.length ? res[0] : res;
              this.azureCalls(formName, operationType, data, result, path, params);
            }

            this.onSuccess(path, params);
          } else if ((res && res.length && res[0].OperationResultType === 0) || (res && res.OperationResultType === 0)) {
            this.formDataService.getMessages().subscribe(messages => {
              const index = res.MessageCode || res[0].MessageCode;
              this.confirmUpdate(data, this.formData.procedureName, operationType, messages[index], path, params, formName);
            });
          } else if ((res && res.length && res[0].OperationResultType === 2) || (res && res.OperationResultType === 2)) {
            this.formDataService.getMessages().subscribe(messages => {
              const index = res.MessageCode || res[0].MessageCode;
              this.isLoading = false;
              this.snackbarService.openErrorSnackbar(messages[index]);
            });
          } else {
            if (
              [
                "curriculumSubject",
                "personData",
                "person",
                "studentMainClassGroup",
                "studentOtherClassGroup",
                "staffPositionData",
                "teachingNormData",
                "changeYearData",
                "classGroupList"
              ].includes(formName)
            ) {
              const result = res && res.length ? res[0] : res;
              this.azureCalls(formName, operationType, data, result, path, params);
            }

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
        if (res) {
          this.snackbarService.openSuccessSnackbar(this.msgService.successMessages.saveSuccess);
          this.authService.removeUrl();
        }
      });
    }, 500);
  }

  private collectPersonIds(values, additionalValues = []) {
    const idsToCreate = [...(values.curriculumTeachersCreated || []), ...(values.curriculumStudentsCreated || [])]
      .map(elem => +elem.personID || null)
      .filter(id => id);
    const idsToDelete = [...(values.curriculumTeachersDeleted || []), ...(values.curriculumStudentsDeleted || [])]
      .map(elem => +elem.personID || null)
      .filter(id => id);

    const updatedStudentsIds = (values.curriculumStudentsUpdated || []).map(elem => +elem.personID || null).filter(id => id);
    idsToCreate.push(...additionalValues.filter(id => !updatedStudentsIds.includes(id)));

    return { idsToCreate: idsToCreate.filter(this.onlyUnique), idsToDelete };
  }

  private collectCurriculumIds(values, result) {
    let idsToCreate = [];
    let idsToDelete = [];

    for (let key of [
      "staffPositionHorariumCreated",
      "studentCurriculumACreated",
      "studentCurriculumBCreated",
      "studentCurriculumVCreated",
      "studentCurriculumGCreated",
      "studentCurriculumDCreated",
      "studentCurriculumECreated",
      "studentCurriculumFCreated"
    ]) {
      if (values[key]) {
        idsToCreate.push(...values[key].map(elem => +elem.curriculumID || null).filter(id => id));
      }
    }

    for (let key of [
      "staffPositionHorariumDeleted",
      "studentCurriculumADeleted",
      "studentCurriculumBDeleted",
      "studentCurriculumVDeleted",
      "studentCurriculumGDeleted",
      "studentCurriculumDDeleted",
      "studentCurriculumEDeleted",
      "studentCurriculumFDeleted"
    ]) {
      if (values[key]) {
        idsToDelete.push(...values[key].map(elem => +elem.curriculumID || null).filter(id => id));
      }
    }

    if (result.curriculumDeletedString) {
      idsToDelete.push(
        ...result.curriculumDeletedString
          .split(",")
          .map(elem => +elem || null)
          .filter(id => id)
      );
    }

    return { idsToCreate, idsToDelete };
  }

  private azureCalls(formName: string, operationType: ModeInt, data: any, result: any, path: string, params) {
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : this.route.snapshot.queryParams;
    const exists = queryParams.exists;

    if (formName === "curriculumSubject") {
      let personids: { idsToCreate: number[]; idsToDelete: number[] } = this.collectPersonIds(
        operationType === ModeInt.update ? data : this.convertLists(data)
      );

      if (data.isWholeClass) {
        const getCurriculumStudentsBody = {
          data: {
            curriculumIDs: +result.id,
            instid: queryParams.instid,
            schoolYear: queryParams.schoolYear
          },
          procedureName: "inst_year.CurriculumAddedStudents",
          operationType: ModeInt.create
        };

        this.formDataService.performProcedure(getCurriculumStudentsBody).subscribe((curriculumStudents: any) => {
          try {
            curriculumStudents && (curriculumStudents = JSON.parse(curriculumStudents));
          } catch (err) {}

          personids = this.collectPersonIds(
            operationType === ModeInt.update ? data : this.convertLists(data),
            curriculumStudents.map(student => student.PersonID)
          );

          if (operationType === ModeInt.create) {
            this.classCreate(queryParams, operationType, +result.id, personids.idsToCreate, +data.instid);
          } else {
            this.updateClass(queryParams, data, operationType, personids);
          }
        });
      } else if (operationType === ModeInt.create) {
        this.classCreate(queryParams, operationType, +result.id, personids.idsToCreate, +data.instid);
      } else {
        this.updateClass(queryParams, data, operationType, personids);
      }
    } else if (formName === "personData" && operationType === ModeInt.create && exists == 0) {
      const bodyTeacher = { personID: +result.personid };
      const reqUrlTeacher = "/v1/azure-integrations/teacher/create";
      this.azureService.createTeacher(bodyTeacher).subscribe(
        (res: any) => {
          this.onAzureCall(queryParams, operationType, { status: res.status }, reqUrlTeacher, bodyTeacher, 0);
          !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, reqUrlTeacher, bodyTeacher, 1);
        },
        error => {
          this.onAzureCall(queryParams, operationType, { status: error.status }, reqUrlTeacher, bodyTeacher, 0);
          this.onAzureCall(queryParams, operationType, error, reqUrlTeacher, bodyTeacher, 1);
        }
      );

      const bodyInstTeacher = { personID: +result.personid, institutionID: +data.instid };
      const reqUrl = "/v1/azure-integrations/teacher/enrollment-school-create";
      this.azureService.createInstitutionTeacher(bodyInstTeacher).subscribe(
        (res: any) => {
          this.onAzureCall(queryParams, operationType, { status: res.status }, reqUrl, bodyInstTeacher, 0);
          !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, reqUrl, bodyInstTeacher, 1);
        },
        error => {
          this.onAzureCall(queryParams, operationType, { status: error.status }, reqUrl, bodyInstTeacher, 0);
          this.onAzureCall(queryParams, operationType, error, reqUrl, bodyInstTeacher, 1);
        }
      );
    } else if (formName === "personData" && operationType === ModeInt.create && exists == 1) {
      const body = { personID: +result.personid, institutionID: +data.instid };
      const reqUrl = "/v1/azure-integrations/teacher/enrollment-school-create";
      this.azureService.createInstitutionTeacher(body).subscribe(
        (res: any) => {
          this.onAzureCall(queryParams, operationType, { status: res.status }, reqUrl, body, 0);
          !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, reqUrl, body, 1);
        },
        error => {
          this.onAzureCall(queryParams, operationType, { status: error.status }, reqUrl, body, 0);
          this.onAzureCall(queryParams, operationType, error, reqUrl, body, 1);
        }
      );
    } else if (formName === "person" && operationType === ModeInt.update) {
      if (data.isTerminatedContract) {
        const body = { personID: +data.personid, institutionID: +data.instid };
        const reqUrl = "/v1/azure-integrations/teacher/enrollment-school-delete";
        this.azureService.deleteTeacher(body).subscribe(
          (res: any) => {
            this.onAzureCall(queryParams, operationType, { status: res.status }, reqUrl, body, 0);
            !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, reqUrl, body, 1);
          },
          error => {
            this.onAzureCall(queryParams, operationType, { status: error.status }, reqUrl, body, 0);
            this.onAzureCall(queryParams, operationType, error, reqUrl, body, 1);
          }
        );
      } else {
        const body = { personID: +data.personid };
        const reqUrl = "/v1/azure-integrations/teacher/update";
        this.azureService.updateTeacher(body).subscribe(
          (res: any) => {
            this.onAzureCall(queryParams, operationType, { status: res.status }, reqUrl, body, 0);
            !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, reqUrl, body, 1);
          },
          error => {
            this.onAzureCall(queryParams, operationType, { status: error.status }, reqUrl, body, 0);
            this.onAzureCall(queryParams, operationType, error, reqUrl, body, 1);
          }
        );
      }
    } else if (["studentMainClassGroup", "studentOtherClassGroup", "staffPositionData", "teachingNormData"].includes(formName)) {
      const { idsToCreate, idsToDelete } = this.collectCurriculumIds(data, result);
      const personid = +queryParams.personid;

      if (idsToCreate && idsToCreate.length) {
        const createBody = { personID: personid, curriculumIDs: idsToCreate };
        const createUrl =
          formName === "staffPositionData" || "teachingNormData"
            ? "/v1/azure-integrations/teacher/enrollment-class-create"
            : "/v1/azure-integrations/student/enrollment-class-create";
        const createFunc = formName === "staffPositionData" || "teachingNormData" ? "teacherClassesEnroll" : "studentClassesEnroll";
        this.azureService[createFunc](createBody).subscribe(
          (res: any) => {
            this.onAzureCall(queryParams, operationType, { status: res.status }, createUrl, createBody, 0);
            !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, createUrl, createBody, 1);
          },
          error => {
            this.onAzureCall(queryParams, operationType, { status: error.status }, createUrl, createBody, 0);
            this.onAzureCall(queryParams, operationType, error, createUrl, createBody, 1);
          }
        );
      }

      if (idsToDelete && idsToDelete.length) {
        const deleteBody = { personID: personid, curriculumIDs: idsToDelete };
        const deleteUrl =
          formName === "staffPositionData" || "teachingNormData"
            ? "/v1/azure-integrations/teacher/enrollment-class-delete"
            : "/v1/azure-integrations/student/enrollment-class-delete";
        const deleteFunc = formName === "staffPositionData" || "teachingNormData" ? "teacherClassesDelete" : "studentClassesDelete";
        this.azureService[deleteFunc](deleteBody).subscribe(
          (res: any) => {
            this.onAzureCall(queryParams, operationType, { status: res.status }, deleteUrl, deleteBody, 0);
            !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, deleteUrl, deleteBody, 1);
          },
          error => {
            this.onAzureCall(queryParams, operationType, { status: error.status }, deleteUrl, deleteBody, 0);
            this.onAzureCall(queryParams, operationType, error, deleteUrl, deleteBody, 1);
          }
        );
      }

      if ((!idsToCreate && !idsToDelete) || (!idsToCreate.length && !idsToDelete.length)) {
        this.onSuccess(path, params);
      }
    } else if (formName === "changeYearData" && data.changeYearStatus == 1) {
      const idsToCreate = data.curriculumCreatedUpdated?.map(elem => elem.id);
      const curriculumCreated = idsToCreate || [];

      const getCurriculumStudentsBody = {
        data: {
          curriculumIDs: curriculumCreated.join(","),
          instid: queryParams.instid,
          schoolYear: queryParams.schoolYear
        },
        procedureName: "inst_year.CurriculumAddedStudents",
        operationType: ModeInt.create
      };

      this.formDataService.performProcedure(getCurriculumStudentsBody).subscribe((curriculumStudents: any) => {
        try {
          curriculumStudents && (curriculumStudents = JSON.parse(curriculumStudents));
        } catch (err) {}
        for (let id of curriculumCreated) {
          const body = {
            curriculumID: +id,
            personIDs: curriculumStudents.filter(record => record.CurriculumID === id).map(record => record.PersonID),
            institutionID: +data.instid
          };
          const reqUrl = "/v1/azure-integrations/class/create";
          this.azureService.createClass(body).subscribe(
            (res: any) => {
              this.onAzureCall(queryParams, operationType, { status: res.status }, reqUrl, body, 0);
              !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, reqUrl, body, 1);
            },
            error => {
              this.onAzureCall(queryParams, operationType, { status: error.status }, reqUrl, body, 0);
              this.onAzureCall(queryParams, operationType, error, reqUrl, body, 1);
            }
          );
        }
      });
    } else if (formName === "classGroupList" && data["createRUP"]) {
      const getCurriculumsAndStudentsBody = {
        data: {
          classid: queryParams.parentclassid,
          instid: queryParams.instid,
          schoolYear: queryParams.schoolYear
        },
        procedureName: "inst_year.CurriculumAddedStudentsRUP",
        operationType: ModeInt.create
      };

      this.formDataService.performProcedure(getCurriculumsAndStudentsBody).subscribe((curriculumsAndStudentsBody: any) => {
        try {
          curriculumsAndStudentsBody && (curriculumsAndStudentsBody = JSON.parse(curriculumsAndStudentsBody));
        } catch (err) {}

        const curriculums = [];

        for (let curriculum of curriculumsAndStudentsBody) {
          const existingCurriculum = curriculums.find(elem => elem.curriculumId === curriculum.CurriculumID);
          if (existingCurriculum) {
            existingCurriculum.personIds.push(curriculum.PersonID);
          } else {
            curriculums.push({
              curriculumId: curriculum.CurriculumID,
              personIds: [curriculum.PersonID]
            });
          }
        }

        for (let curriculum of curriculums) {
          this.classCreate(queryParams, 1, curriculum.curriculumId, curriculum.personIds, +data.instid);
        }
      });
    }
  }

  private onSuccess(path, params) {
    this.successfulSubmit(path, params);
    this.isLoading = false;
  }

  private onAzureCall(queryParams, operationType, error, apiCall: string, payload, isError) {
    const body = {
      data: {
        sysuserid: queryParams.sysuserid,
        instid: queryParams.instid,
        schoolYear: queryParams.schoolYear,
        error_number: error?.error?.status || error?.status,
        error_message: error?.error?.message || error?.message,
        error_procedure: environment.azureUrl + apiCall,
        operationType,
        curriculumID: payload.curriculumID,
        personID: payload.personID,
        payload: JSON.stringify(payload),
        isError
      },
      procedureName: "logs.azureErrorLog",
      operationType: ModeInt.create
    };

    this.formDataService.performProcedure(body).subscribe();
  }

  private confirmUpdate(
    data,
    procedureName: string,
    operationType: number,
    message: string,
    path: string,
    params: Object,
    formName: string
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
              if (
                [
                  "curriculumSubject",
                  "personData",
                  "person",
                  "studentMainClassGroup",
                  "studentOtherClassGroup",
                  "staffPositionData",
                  "teachingNormData",
                  "classGroupList"
                ].includes(formName)
              ) {
                const result = res && res.length ? res[0] : res;
                this.azureCalls(formName, operationType, data, result, path, params);
              }

              this.onSuccess(path, params);
            } else if ((res && res.length && res[0].OperationResultType === 2) || (res && res.OperationResultType === 2)) {
              this.formDataService.getMessages().subscribe(messages => {
                const index = res.MessageCode || res[0].MessageCode;
                this.isLoading = false;
                this.snackbarService.openErrorSnackbar(messages[index]);
              });
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

  getFormDataFromServer(queryParams = null) {
    this.isLoading = true;
    this.formData = null;
    this.dataInit = false;

    if (queryParams === null) {
      queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;
    }

    const params = this.route.snapshot.params || {};
    const body: any = { ...queryParams };

    // for (const key in body) {
    //   if (body[key] === "null") {
    //     delete body[key];
    //   }
    // }

    let operationType = ModeInt.view;

    const instType = params.type ? FormTypeInt[params.type] + 1 : FormTypeInt[this.authService.getType()] + 1;

    if (this.canEdit) {
      this.path === Menu.Edit ? (operationType = ModeInt.update) : (operationType = ModeInt.create);
      queryParams["operationType"] && (operationType = queryParams["operationType"]);
    }

    this.getFormData(body, operationType, instType);
  }

  private getFormData(body, operationType, instType) {
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };
    const isHistory = this.route.snapshot.params.tab === Tabs.history || queryParams.year !== undefined;

    if (this.formName === "soSubmissionData") {
      this.formDataService.submissionCheckPeriod({ ...body, instType }).subscribe((res: any) => {
        this.submissionPeriod = res && res.length ? res[0] : res;
        this.formDataHelper(body, operationType, instType, isHistory);
      });
    } else {
      this.formDataHelper(body, operationType, instType, isHistory);
    }
  }

  private formDataHelper(body, operationType, instType, isHistory) {
    this.formDataService
      .getFormData(this.formName, body, operationType, instType, isHistory, this.mode, true, this.path === this.menu.Preview)
      .subscribe(
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

          if (this.formName !== "institution") {
            this.router.navigate(["/", Menu.Home]);
            this.authService.removeUrl();
          }

          if (!environment.production) {
            console.log(error);
          }
        }
      );
  }

  async procedurePerformed(event: {
    data: Object;
    procName: string;
    canSign: boolean;
    searchByEik: boolean;
    requiredFields: string[];
    groupValues: Object;
    generateCertificate: boolean;
    button: FieldConfig;
    subsection: { name: string; id: number | string };
  }) {
    let requiredFieldNames = "";

    if (event.requiredFields) {
      for (let fldName of event.requiredFields) {
        if (!event.groupValues[fldName]) {
          const allFields = this.formDataService.getAllFields(fldName, null, null, this.form.form);
          requiredFieldNames += allFields[0][0].label + ", ";
        }
      }
    }

    if (event.procName && !requiredFieldNames) {
      if (event.canSign) {
        this.isLoading = true;
        this.electronicSignatureService.getVersion().subscribe(
          res => {
            this.isLoading = false;
            const eik = event.searchByEik ? event.data["eik"] : null;
            this.signData(event, eik);
          },
          err => {
            this.isLoading = false;
            this.dialog.open(ModalComponent, {
              width: "45%",
              data: {
                hasLocalServerLink: true,
                cancelBtnLbl: "Затвори"
              }
            });
          }
        );
      } else if (event.generateCertificate) {
        if (!event.button.value) {
          this.isLoading = true;
          const certificateDataTmp: any = await this.fileService.getCertificateData(event.data["submissionDataID"]).toPromise();

          const certificateData =
            certificateDataTmp && typeof certificateDataTmp === "object" && certificateDataTmp.length
              ? certificateDataTmp[0]
              : certificateDataTmp;

          const now = new Date();
          certificateData.certificateDate = now.toLocaleDateString("bg");
          certificateData.certificateTime = now.toLocaleTimeString("bg");

          if (!certificateData.certificateTime.includes("ч")) {
            certificateData.certificateTime += " ч.";
          }

          const html = await this.fileService.getCertificateTemplate().toPromise();

          let fileReader: FileReader = new FileReader();
          fileReader.onloadend = async () => {
            let htmlTemplate = fileReader.result;

            htmlTemplate = this.fileService.substituteTemplateInfo(<string>htmlTemplate, certificateData);

            const opt = {
              filename: `certificate.pdf`,
              image: { type: "jpeg", quality: 1 },
              html2canvas: { scale: 2 },
              jsPDF: { unit: "in", format: "letter", orientation: "portrait" }
            };

            const blob = await html2pdf().from(htmlTemplate).set(opt).toPdf().get("pdf").output("blob");

            const file = new File([blob], `certificate.pdf`, { type: "application/pdf" });
            this.fileService.uploadFile(file).subscribe(
              (res: { name: string; size: number; blobId: number; location: string }) => {
                event.button.value = res.blobId;
                this.procedureHelper(event, file);
                this.isLoading = false;
              },
              err => {
                this.snackbarService.openErrorSnackbar(this.msgService.fileMessages.fileUploadErr);
                this.isLoading = false;
              }
            );
          };

          fileReader.readAsText(html);
        } else {
          const queryParams = environment.production
            ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
            : { ...this.route.snapshot.queryParams };

          this.isLoading = true;
          this.fileService.getCertificateFile(event.button.value, queryParams.submissionDataID).subscribe(
            blob => {
              const file = new File([blob], `certificate.pdf`, { type: ".pdf" });
              this.downloadFile(file);
              this.isLoading = false;
            },
            err => (this.isLoading = false)
          );
        }
      } else if (event.procName) {
        this.procedureHelper({ data: event.data, procName: event.procName });
      }
    } else if (event.procName) {
      const msg = "Трябва да попълните следните полета преди да продължите: " + requiredFieldNames.slice(0, -2);
      this.snackbarService.openErrorSnackbar(msg);
    }
  }

  private downloadFile(file: File) {
    const anchor = document.createElement("a");
    anchor.href = window.URL.createObjectURL(file);
    anchor.download = `certificate.pdf`;
    anchor.click();
  }

  private procedureHelper(
    event: { data: Object; procName: string; button?: FieldConfig; subsection?: { name: string; id: number | string } },
    file: File = null
  ) {
    this.isLoading = true;
    this.form.formGroup.markAsPristine();

    if (event.button) {
      if (event.subsection && event.data[event.subsection.name] && (event.subsection.id + "").includes("new")) {
        const index = (event.subsection.id + "").replace("new", "");
        event.data[event.subsection.name].length
          ? (event.data[event.subsection.name][index]["blobID"] = event.button.value)
          : event.data[event.subsection.name].push({ id: "", blobID: event.button.value });
      } else if (event.subsection && event.data[event.subsection.name]) {
        const elem = event.data[event.subsection.name].find(elem => elem.id === event.subsection.id);
        elem ? (elem["blobID"] = event.button.value) : event.data[event.subsection.name].push({ id: "", blobID: event.button.value });
      } else {
        event.data["blobID"] = event.button.value;
      }
    }

    this.formDataService
      .performProcedure({
        data: this.convertLists(event.data),
        procedureName: event.procName,
        operationType: ModeInt.update
      })
      .subscribe(
        (res: any) => {
          try {
            res && (res = JSON.parse(res));
            res && res.length && (res = res[0]);
          } catch (err) {}

          this.isLoading = false;

          if (!event.button || !event.button.value) {
            let path = "/";
            let params = {};

            if (this.authService.getPrevUrlData()) {
              [path, params] = this.authService.getPrevUrlData();
              this.authService.removeUrl();
            }

            if (res.submissionStatusID != 98 || this.authService.isMon() || this.authService.isRuo()) {
              this.snackbarService.openSuccessSnackbar(this.msgService.successMessages.saveSuccess);
              setTimeout(() => {
                this.disableButtons = true;
                this.router.navigate([path], { queryParams: params });
              }, 500);
            } else {
              environment.production && (params = this.helperService.decodeParams(this.route.snapshot.queryParams.q));
              params["check"] = 1;
              environment.production && (params = this.helperService.encodeParams(params));

              this.router.navigate([`home/${this.authService.getType()}/list/validate`], { queryParams: params });
            }
          } else if (file) {
            this.downloadFile(file);
          }
        },
        err => (this.isLoading = false)
      );
  }

  private signData(event: { data: Object; procName: string; canSign: boolean }, eik) {
    this.electronicSignatureService.sign(event.data, eik).subscribe(
      (res: { addititionalInformation; contents: string; isError: boolean; message: string }) => {
        if (res.isError) {
          this.isLoading = false;
          this.snackbarService.openErrorSnackbar(res.message);
        } else if (res.contents) {
          this.snackbarService.openSuccessSnackbar(this.msgService.successMessages.saveSuccess);
          const eSignature = new x509.X509Certificate(
            res.contents.match(/<X509Certificate>(.*?)<\/X509Certificate>/g)[0].replace(/<\/?X509Certificate>/g, "")
          ).subject;

          const tzoffset = new Date().getTimezoneOffset() * 60000; //offset in milliseconds
          const localISOTime = new Date(Date.now() - tzoffset).toISOString().slice(0, -1);

          const certificate = this.castCertificate(eSignature);
          this.procedureHelper({
            data: {
              ...event.data,
              signedData: btoa(res.contents),
              signer: certificate["CN"] || certificate["O"],
              signerDate: localISOTime.split(".")[0]
            },
            procName: event.procName
          });
        }
      },
      err => {
        this.isLoading = false;
        this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.error);
      }
    );
  }

  private castCertificate(certificate: string) {
    const fields = certificate.split(", ");
    const certificateObj: any = {};
    fields.forEach(fld => (certificateObj[fld.split("=")[0]] = fld.split("=")[1]));

    return certificateObj;
  }

  private convertLists(data: Object) {
    const convertedData: any = {};
    for (let key in data) {
      if (data[key] && typeof data[key] === "object" && data[key].length !== undefined) {
        const currentKeys = data[key].map(elem => elem.id + "");
        const previousArr = this.formDataService.formValues[key] || [];
        const previousKeys = previousArr.map(elem => elem.id + "");
        convertedData[key + "Deleted"] = previousArr.filter(elem => !currentKeys.includes(elem.id + ""));
        convertedData[key + "Created"] = data[key].filter(elem => !previousKeys.includes(elem.id + ""));
        convertedData[key + "Updated"] = data[key].filter(elem => previousKeys.includes(elem.id + ""));
      } else {
        convertedData[key] = data[key];
      }
    }

    return convertedData;
  }

  onPerformRegixProc(data: { regixData: RegixData; groupValues: any }) {
    const params: any = {};

    for (let elem of data.regixData.requestMap) {
      params[elem.regixField] = data.groupValues[elem.neispuoField] || this.form.formGroup.value[elem.neispuoField];
    }

    params.SearchDate = new Date().toISOString().split("T")[0];

    this.isLoading = true;
    this.regixService
      .getRegixData({
        operation: data.regixData.operation,
        xmlns: data.regixData.xmlns,
        requestName: data.regixData.requestName,
        params: JSON.stringify(params)
      })
      .subscribe(
        async res => {
          const regixData = res[data.regixData.responseName]; //eval

          for (let field of data.regixData.responseMap) {
            const formControl = this.form.formGroup.controls[field.neispuoField];

            let fieldVal;

            if (field.regixField) {
              fieldVal = eval("regixData?." + field.regixField.replace(/\./g, "?.")) || null;
              fieldVal && !isNaN(+fieldVal) && (fieldVal = +fieldVal);
              this.helperService.regixValueSet.next({ fldName: field.neispuoField, value: fieldVal });
            } else if (field.regixFields) {
              fieldVal = "";
              for (let row of field.regixFields) {
                const regixVal = eval("regixData?." + row.regixField.replace(/\./g, "?."));
                fieldVal += regixVal ? row.prefix + regixVal : "";
              }
            }

            if (!field.optionsPath) {
              formControl.setValue(fieldVal);
            } else {
              let res = await this.formDataService
                .getDynamicNomenclature(field.optionsPath, false, {
                  filterValue: fieldVal
                })
                .toPromise();

              if (res && res.length) {
                const mappedValue =
                  res.find(row => (row.regixValue + "").toUpperCase() === (fieldVal + "").toUpperCase())?.neispuoValue || null;

                this.helperService.regixValueSet.next({ fldName: field.neispuoField, value: mappedValue });

                formControl.setValue(mappedValue);
              }
            }
          }

          this.isLoading = false;
        },
        err => {
          this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.regixDown);
          this.isLoading = false;
        }
      );
  }

  private classCreate(queryParams, operationType, curriculumID, personIDs, institutionID) {
    const body = {
      curriculumID, // curriculumid or id???
      personIDs,
      institutionID
    };

    const reqUrl = "/v1/azure-integrations/class/create";
    this.azureService.createClass(body).subscribe(
      (res: any) => {
        this.onAzureCall(queryParams, operationType, { status: res.status }, reqUrl, body, 0);
        !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, reqUrl, body, 1);
      },
      error => {
        this.onAzureCall(queryParams, operationType, { status: error.status }, reqUrl, body, 0);
        this.onAzureCall(queryParams, operationType, error, reqUrl, body, 1);
      }
    );
  }

  private updateClass(queryParams, data, operationType, personids) {
    const body = {
      curriculumID: +data.curriculumid,
      personIDsToCreate: personids.idsToCreate,
      personIDsToDelete: personids.idsToDelete,
      institutionID: +data.instid
    };
    const reqUrl = "/v1/azure-integrations/class/update";
    this.azureService.updateClass(body).subscribe(
      (res: any) => {
        this.onAzureCall(queryParams, operationType, { status: res.status }, reqUrl, body, 0);
        !(res.status === 200 || res.status === 201) && this.onAzureCall(queryParams, operationType, res, reqUrl, body, 1);
      },
      error => {
        this.onAzureCall(queryParams, operationType, { status: error.status }, reqUrl, body, 0);
        this.onAzureCall(queryParams, operationType, error, reqUrl, body, 1);
      }
    );
  }

  private onlyUnique(value, index, array) {
    return array.indexOf(value) === index;
  }
}
