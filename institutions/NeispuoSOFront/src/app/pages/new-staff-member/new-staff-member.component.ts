import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "../../auth/auth.service";
import { Menu } from "../../enums/menu.enum";
import { Mode, ModeInt } from "../../enums/mode.enum";
import { TRMessage } from "../../enums/tr";
import { Form } from "../../models/form.interface";
import { Subsection } from "../../models/subsection.interface";
import { FormDataService } from "../../services/form-data.service";
import { environment } from "../../../environments/environment";
import { HelperService } from "src/app/services/helpers.service";
import { Option } from "src/app/models/option.interface";
import { FormTypeInt } from "../../enums/formType.enum";
import { RegixService } from "../../services/regix.service";
import { SnackbarService } from "../../services/snackbar.service";
import { MessagesService } from "../../services/messages.service";
import { forkJoin } from "rxjs";

@Component({
  selector: "app-new-staff-member",
  templateUrl: "./new-staff-member.component.html",
  styleUrls: ["./new-staff-member.component.scss"]
})
export class NewStaffMemberComponent implements OnInit {
  @ViewChild("egnfld", { static: false }) egnFld;
  @ViewChild("lnchfld", { static: false }) lnchFld;
  @ViewChild("idnfld", { static: false }) idnFld;

  isLoading: boolean = false;
  egn;
  data: Form;
  formGroup: FormGroup;
  message: string = "";
  exists: boolean;
  idType = null;
  personalIdTypes: Option[] = [];
  extData;
  invalidEgn: boolean = false;
  invalidLnch: boolean = false;
  idnValidEgn: boolean = false;
  idnValidLnch: boolean = false;
  isForeignCitizen: boolean = false;
  regixError: boolean = false;

  private searchEgn;
  private searchIdType;

  get modes() {
    return Mode;
  }

  get trMessage() {
    return TRMessage;
  }

  constructor(
    private formDataService: FormDataService,
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private regixService: RegixService,
    private msgService: MessagesService,
    private snackbarService: SnackbarService
  ) {}

  get menu() {
    return Menu;
  }

  async ngOnInit() {
    this.isLoading = true;

    this.personalIdTypes = await this.formDataService.getDynamicNomenclature("/data/get/personalIDTypes", false).toPromise();

    this.isLoading = false;
  }

  getData() {
    this.message = "";
    this.data = null;
    this.isLoading = true;
    const body = { personalID: this.egn };
    this.searchEgn = this.egn;
    this.searchIdType = this.idType;

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    this.extData = queryParams.extdata ? +queryParams.extdata : 0;

    this.formDataService
      .personExistForInst({ pesonalid: this.searchEgn, instid: queryParams.instid, personalIdType: this.searchIdType })
      .subscribe(
        (res: any) => {
          res = res && res.length ? res[0] : res;
          if (res && res.isExist) {
            this.exists = true;
            this.message = this.trMessage.ExistForInst;
            this.isLoading = false;
          } else {
            this.formDataService.getTrData(body).subscribe(
              (data: any) => {
                if (data && !data.length) {
                  this.message = this.trMessage.MissingEGN;
                  this.exists = false;
                  this.isLoading = false;
                } else {
                  const type = FormTypeInt[this.authService.getType()] + 1;
                  this.exists = true;
                  this.formDataService.fillData(ModeInt.view, type, data[0], "personData", false).subscribe(
                    res => {
                      this.data = res;
                      this.message = this.extData ? this.trMessage.ExistExtData : this.trMessage.NEISPUOExists;
                      this.createFormGroup();
                      this.isLoading = false;
                    },
                    err => (this.isLoading = false)
                  );
                }
              },
              err => (this.isLoading = false)
            );
          }
        },
        err => (this.isLoading = false)
      );
  }

  private createFormGroup() {
    const group = this.fb.group({});
    if (this.data) {
      this.data.sections.forEach(section => {
        section.subsections.forEach(subsection => {
          this.helperService.createSubsectionGroup(subsection, group, section.hidden, section.disabled);
        });
      });
    }

    this.formGroup = group;
  }

  getFormGroup(subsection: Subsection) {
    return this.helperService.getFormGroup(subsection, this.formGroup);
  }

  navigate() {
    const egn = this.searchEgn ? this.searchEgn : this.egn;
    const personalIdType = this.searchIdType ? this.searchIdType : this.idType;

    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    queryParams.personalID = egn;
    queryParams.personalIdType = personalIdType;
    queryParams.exists = this.exists ? 1 : 0;
    environment.production && (queryParams = this.helperService.encodeParams(queryParams));

    this.router.navigate(["/", Menu.Create, "personData"], { queryParams });
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

  isInvalidId() {
    setTimeout(() => {
      this.invalidEgn = this.egn && this.egnFld && this.egnFld.control.errors;
      this.invalidLnch = this.egn && this.lnchFld && this.lnchFld.control.errors;
      this.idnValidEgn = this.egn && this.idnFld && this.idnFld.control.errors?.validEgn;
      this.idnValidLnch = this.egn && this.idnFld && this.idnFld.control.errors?.validLnch;
    });
  }

  getRegixData() {
    if ((this.isForeignCitizen && this.idType === 0) || this.idType === 1) {
      this.regixService
        .getRegixData({
          operation: "TechnoLogica.RegiX.MVRERChAdapter.APIService.IMVRERChAPI.GetForeignIdentityV2",
          xmlns: "http://egov.bg/RegiX/MVR/RCH/ForeignIdentityInfoRequest",
          requestName: "ForeignIdentityInfoRequest",
          params: JSON.stringify({ IdentifierType: this.idType === 1 ? "LNCh" : "EGN", Identifier: this.egn })
        })
        .subscribe(
          res => {
            if (res?.DeathDate) {
              this.message = this.trMessage.DeadPerson;
            } else {
              res.foreignCitizen = true;
              sessionStorage.setItem("regixData", res);
              this.navigate();
            }
          },
          () => this.regixErrorResponse()
        );
    } else if (this.idType === 0) {
      forkJoin([
        this.regixService.getRegixData({
          operation: "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.PersonDataSearch",
          xmlns: "http://egov.bg/RegiX/GRAO/NBD/PersonDataRequest",
          requestName: "PersonDataRequest",
          params: JSON.stringify({ EGN: this.egn })
        }),
        this.regixService.getRegixData({
          operation: "TechnoLogica.RegiX.GraoPNAAdapter.APIService.IPNAAPI.PermanentAddressSearch",
          xmlns: "http://egov.bg/RegiX/GRAO/PNA/PermanentAddressRequest",
          requestName: "PermanentAddressRequest",
          params: JSON.stringify({ EGN: this.egn, SearchDate: new Date().toISOString().split("T")[0] })
        }),
        this.regixService.getRegixData({
          operation: "TechnoLogica.RegiX.GraoPNAAdapter.APIService.IPNAAPI.TemporaryAddressSearch",
          xmlns: "http://egov.bg/RegiX/GRAO/PNA/TemporaryAddressRequest",
          requestName: "TemporaryAddressRequest",
          params: JSON.stringify({ EGN: this.egn, SearchDate: new Date().toISOString().split("T")[0] })
        })
      ]).subscribe(
        ([personData, permanentAddress, temporaryAddress]) => {
          if (personData?.DeathDate) {
            this.message = this.trMessage.DeadPerson;
          } else {
            sessionStorage.setItem(
              "regixData",
              JSON.stringify({
                ...personData.PersonDataResponse,
                PermanentAddress: { ...permanentAddress.PermanentAddressResponse },
                TemporaryAddress: { ...temporaryAddress.TemporaryAddressResponse }
              })
            );
            this.navigate();
          }
        },
        () => this.regixErrorResponse()
      );
    }
  }

  private regixErrorResponse() {
    this.regixError = true;
    this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.regixDown);
  }
}
