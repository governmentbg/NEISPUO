import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthService } from "../../auth/auth.service";
import { Menu } from "../../enums/menu.enum";
import { Mode, ModeInt } from "../../enums/mode.enum";
import { TRMessage, TRStatus } from "../../enums/tr";
import { Form } from "../../models/form.interface";
import { Option } from "../../models/option.interface";
import { Subsection } from "../../models/subsection.interface";
import { FormDataService } from "../../services/form-data.service";
import { environment } from "../../../environments/environment";
import { MessagesService } from "../../services/messages.service";
import { SnackbarService } from "../../services/snackbar.service";

@Component({
  selector: "app-new-institution",
  templateUrl: "./new-institution.component.html",
  styleUrls: ["./new-institution.component.scss"]
})
export class NewInstitutionComponent implements OnInit {
  institutionArr: Option[];
  institutionType: { value: string; eikObligatory: boolean } = { value: null, eikObligatory: null };
  isLoading: boolean = true;
  eik;
  data: Form;
  formGroup: FormGroup;
  message: string = "";
  regixError: boolean = false;

  private searchEik;
  private searchInstitutionType: { value: string; eikObligatory: boolean } = { value: null, eikObligatory: null };

  get modes() {
    return Mode;
  }

  get trMessage() {
    return TRMessage;
  }

  constructor(
    private formDataService: FormDataService,
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private msgService: MessagesService,
    private snackbarService: SnackbarService
  ) {}

  get menu() {
    return Menu;
  }

  ngOnInit() {
    const role = this.authService.getRole();
    this.formDataService.getInstitutionCombo(role).subscribe(
      (res: any) => {
        this.isLoading = false;
        this.institutionArr = res;
      },
      () => (this.isLoading = false)
    );
  }

  getData() {
    this.message = "";
    this.data = null;
    this.regixError = null;
    this.isLoading = true;

    this.getTrData();
  }

  onSelectionChange(value) {
    this.institutionType.value = value;
    this.institutionType.eikObligatory = this.institutionArr.find(option => option.code === value).eikObligatory;
    !this.institutionType.eikObligatory && (this.eik = null);
  }

  private createFormGroup() {
    const group = this.fb.group({});
    if (this.data) {
      this.data.sections.forEach(section => {
        section.subsections.forEach(subsection => {
          this.formDataService.createSubsectionGroup(subsection, group, section.hidden);
        });
      });
    }

    this.formGroup = group;
  }

  getFormGroup(subsection: Subsection) {
    return this.formDataService.getFormGroup(subsection, this.formGroup);
  }

  navigate() {
    const instKind = this.searchInstitutionType.value ? this.searchInstitutionType : this.institutionType;
    const eik = this.searchEik ? this.searchEik : this.eik;
    const option: Option = this.institutionArr.find(option => option.code === instKind.value);

    let queryParams: any = {
      instKind: instKind.value,
      procType: 1,
      instType: option.instType,
      sysuserid: this.authService.getSysUserId(),
      region: this.authService.getRegion()
    };
    instKind.eikObligatory && (queryParams.instid = eik);

    environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

    this.router.navigate(["/", Menu.CreateProcedure, option.formName], { queryParams });
  }

  syncWithRegix() {
    this.isLoading = true;

    const eik = this.searchEik ? this.searchEik : this.eik;

    this.formDataService
      .getRegixData({
        operation: "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualState",
        params: JSON.stringify({ UIC: eik }),
        requestName: "ActualStateRequest",
        xmlns: "http://egov.bg/RegiX/AV/TR/ActualStateRequest"
      })
      .subscribe(
        async res => {
          if (!res?.ActualStateResponse?.Company) {
            this.getBulstatData(eik);
          } else {
            const mappedRes = await this.parseRegixTRData(res.ActualStateResponse);
            this.saveMappedData(mappedRes);
          }
        },
        () => {
          this.getBulstatData(eik);
        }
      );
  }

  private getBulstatData(eik) {
    this.formDataService
      .getRegixData({
        operation: "TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.GetStateOfPlay",
        params: JSON.stringify({ UIC: eik }),
        requestName: "GetStateOfPlayRequest",
        xmlns: "http://www.bulstat.bg/GetStateOfPlayRequest"
      })
      .subscribe(
        async res => {
          if (!res?.StateOfPlay?.Subject?.LegalEntitySubject?.CyrillicFullName) {
            this.regixError = true;
            this.isLoading = false;
            this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.regixNotFound);
          } else {
            const mappedRes = await this.parseRegixBulstatData(res.StateOfPlay);
            this.saveMappedData(mappedRes);
            this.getTrData();
          }
        },
        () => {
          this.isLoading = false;
          this.regixError = true;
          this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.regixError);
        }
      );
  }

  private async parseRegixBulstatData(regixData) {
    const mappedRes: any = {};

    if (regixData.Subject?.LegalEntitySubject?.CyrillicFullName) {
      mappedRes.name = regixData.Subject.LegalEntitySubject.CyrillicFullName;
    }

    if (regixData.Subject?.LegalEntitySubject?.CyrillicShortName) {
      mappedRes.abbreviation = regixData.Subject.LegalEntitySubject.CyrillicShortName;
    }

    if (regixData.ScopeOfActivity?.Description) {
      mappedRes.subjectOfActivity = regixData.ScopeOfActivity.Description;
    }

    let correspondenceAddress, seatAddress;
    if (regixData.Subject?.Addresses && regixData.Subject.Addresses.length) {
      seatAddress = regixData.Subject.Addresses.find(address => address.AddressType?.Code == 718);
      correspondenceAddress = regixData.Subject.Addresses.find(address => address.AddressType?.Code == 719);
    } else if (regixData.Subject?.Addresses) {
      seatAddress = regixData.Subject.Addresses.AddressType?.Code == 718 ? regixData.Subject.Addresses : null;
      correspondenceAddress = regixData.Subject.Addresses.AddressType?.Code == 719 ? regixData.Subject.Addresses : null;
    }

    if (seatAddress) {
      const code = seatAddress.Location?.Code || null;
      if (code) {
        [mappedRes.seatAddressRegion, mappedRes.seatAddressMunicipality, mappedRes.seatAddressTown, mappedRes.seatAddress] =
          await this.getAddressData(code, seatAddress);
      }
    }

    if (correspondenceAddress) {
      const code = correspondenceAddress.Location?.Code || null;
      if (code) {
        [
          mappedRes.correspondenceAddressRegion,
          mappedRes.correspondenceAddressMunicipality,
          mappedRes.correspondenceAddressTown,
          mappedRes.correspondenceAddress
        ] = await this.getAddressData(code, correspondenceAddress);
      }
    }

    return mappedRes;
  }

  private async parseRegixTRData(regixData) {
    const mappedRes: any = {};

    if (regixData.Company) {
      mappedRes.name = regixData.Company;
    }

    if (regixData.LegalForm?.LegalFormName) {
      mappedRes.LegalForm = regixData.LegalForm.LegalFormName;
    }

    if (regixData.SubjectOfActivity?.Subject) {
      mappedRes.subjectOfActivity = regixData.SubjectOfActivity?.Subject;
    }

    if (regixData.DataValidForDate) {
      mappedRes.statusDate = regixData.DataValidForDate;
    }

    const seatAddress = regixData.Seat?.Address;
    const correspondenceAddress = regixData.SeatForCorrespondence;

    if (seatAddress) {
      const code = seatAddress.SettlementEKATTE || null;
      if (code) {
        [mappedRes.seatAddressRegion, mappedRes.seatAddressMunicipality, mappedRes.seatAddressTown, mappedRes.seatAddress] =
          await this.getAddressData(code, seatAddress);
      }
    }

    if (correspondenceAddress) {
      const code = correspondenceAddress.SettlementEKATTE || null;
      if (code) {
        [
          mappedRes.correspondenceAddressRegion,
          mappedRes.correspondenceAddressMunicipality,
          mappedRes.correspondenceAddressTown,
          mappedRes.correspondenceAddress
        ] = await this.getAddressData(code, correspondenceAddress);
      }
    }

    return mappedRes;
  }

  private async getAddressData(townCode: string, address) {
    const addressData = await this.formDataService
      .getDynamicNomenclature("/data/get/mapMunicipality", { filterValue: +townCode })
      .toPromise();
    const region = addressData && addressData.length ? addressData[0].region : addressData.region || null;
    const municipality = addressData && addressData.length ? addressData[0].municipality : addressData.municipality || null;
    const town = +townCode;

    let street, streetNumber, entrance, floor, apartment;
    street = address.Street || "";
    streetNumber = address.StreetNumber ? " № " + address.StreetNumber : "";
    entrance = address.Entrance ? ", Вх. " + address.Entrance : "";
    floor = address.Floor ? ", Ет. " + address.Floor : "";
    apartment = address.Apartment ? ", Ап. " + address.Apartment : "";
    return [region, municipality, town, street ? street + streetNumber + entrance + floor + apartment : ""];
  }

  private getTrData() {
    const body = {
      eik: this.eik,
      instKind: this.institutionType.value,
      sysuserid: this.authService.getSysUserId(),
      region: this.authService.getRegion()
    };
    this.searchEik = this.eik;
    this.searchInstitutionType = { ...this.institutionType };

    this.formDataService.getTrData(body).subscribe(
      (message: any) => {
        if (message === TRStatus.ExistEIK || message === TRStatus.MissingEIK) {
          this.message = message === TRStatus.ExistEIK ? this.trMessage.ExistEIK : this.trMessage.MissingEIK;
          this.data = null;
          this.regixError = null;
          this.isLoading = false;
        } else {
          const option: Option = this.institutionArr.find(option => option.code === this.institutionType.value);
          this.formDataService.getFormData("tr-result", body, ModeInt.view, this.institutionType.value, option.instType).subscribe(
            res => {
              this.data = res;
              this.message = null;
              this.regixError = null;
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

  private saveMappedData(mappedRes) {
    this.formDataService
      .submitForm({
        data: { eik: this.eik, ...mappedRes },
        procedureName: "reginst_basic.institutionCreateRITRData",
        operationType: ModeInt.create
      })
      .subscribe(
        () => this.getTrData(),
        () => {
          this.isLoading = false;
          this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.error);
        }
      );
  }
}
