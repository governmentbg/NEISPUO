import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { RoleEnum } from '@authentication/models/role.enum';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { IStatePage } from '@municipal-institutions/services/state-page.inteface';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { MunicipalInstitutionQuery } from '@municipal-institutions/state/municipal-institutions/municipal-institution.query';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { UtilsService } from '@shared/services/utils/utils.service';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

import { combineLatest } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';
import { BudgetingInstitutionEnum } from '@procedures/models/bugeting-institution.enum';
import { FinancialSchoolTypeEnum } from '@procedures/models/financial-school-type.enum';

@Component({
  selector: 'app-municipal-institution-detail-modal',
  templateUrl: './municipal-institution-detail-modal.page.html',
  styleUrls: ['./municipal-institution-detail-modal.page.scss'],
})
export class MunicipalInstitutionDetailModalPage implements OnInit, IStatePage {
  form: FormGroup;

  config: IMIFormComponentState = {} as IMIFormComponentState;

  submitLabel = 'Добави';

  _resp: MunicipalInstitution;

  @Input() set resp(v: MunicipalInstitution) {
    if (!v || v === this.resp) {
      return;
    }
    this._resp = v;
  }

  get resp() {
    return this._resp;
  }

  @Input() clickCallback = (institution) => { };

  isUserMunicipality = false;

  BaseSchoolTypes$ = this.nmQuery.BaseSchoolTypes$
    .pipe(
      filter((baseSchoolTypes) => !!baseSchoolTypes),
      map(
        (baseSchoolTypes) => baseSchoolTypes
          .filter((bst) => bst.BaseSchoolTypeID === EnumBaseSchoolType.KINDERGARTEN || bst.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE),

      ),

    );

  constructor(
    public miQuery: MunicipalInstitutionQuery,
    private cfgs: MICommonFormGroupService,
    public ref: DynamicDialogRef,
    public dconfig: DynamicDialogConfig,
    private nmQuery: NomenclatureQuery,
    private authQuery: AuthQuery,
    private utilsService: UtilsService,
    private scrollService: ScrollService,
  ) { }

  ngOnInit() {
    this.resp = this.dconfig.data.mi as MunicipalInstitution;
    this.clickCallback = this.dconfig.data.clickCallback;
    this.initForm();
    this.nmQuery.BudgetingInstitutions$.pipe(filter((bis) => !!bis)).subscribe((bis) => {
      this.form.patchValue({ BudgetingInstitution: bis.find((bi) => bi.BudgetingInstitutionID === BudgetingInstitutionEnum.MUNICIPALITY) });
    });
    this.nmQuery.FinancialSchoolTypes$.pipe(filter((fsts) => !!fsts)).subscribe((fsts) => {
      this.form.patchValue({ FinancialSchoolType: fsts.find((fst) => fst.FinancialSchoolTypeID === FinancialSchoolTypeEnum.MUNICIPAL) });
    });
    combineLatest([this.authQuery.selectedRole$, this.nmQuery.Municipalities$])
      .pipe(filter(([sr, mun]) => !!sr && !!mun))
      .subscribe(([sr, municipalities]) => {
        if (sr.SysRoleID === RoleEnum.MUNICIPALITY) {
          this.isUserMunicipality = true;

          const userMunicipality = municipalities.find((m) => m.MunicipalityID === sr.MunicipalityID);
          this.form.patchValue({
            Region: userMunicipality.Region,
            Municipality: userMunicipality,
          });
        }
        /**
         * Ugly hack fix in rmt-selection component
         */
        setTimeout(() => {
          this.form.get('Municipality').disable({ emitEvent: false, onlySelf: true });
          this.form.get('Region').disable({ emitEvent: false, onlySelf: true });
        }, 200);
      });
  }

  addMi() {
    this.utilsService.markAllAsDirty(this.form);

    if (this.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }

    this.clickCallback(this.form.getRawValue());
  }

  private initForm() {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response: this.resp,
    });
    this.form.patchValue(this.resp);
  }

  setFormConfig() {
    /** set all to readonly */
    this.config = {
      NameDisabled: false,
      InstitutionIDDisabled: true,
      AbbreviationDisabled: false,
      BulstatDisabled: false,
      TownDisabled: false,
      MunicipalityDisabled: this.isUserMunicipality,
      RegionDisabled: this.isUserMunicipality,
      LocalAreaDisabled: false,
      FinancialSchoolType: true,
      BaseSchoolTypeDisabled: false,
      CPLRAreaTypeDisabled: true,
      BudgetingInstitutionDisabled: true,
      DetailedSchoolTypeDisabled: false,
      HeadFirstNameDisabled: false,
      HeadMiddleNameDisabled: false,
      HeadLastNameDisabled: false,
      TRPostCodeDisabled: false,
      TRAddressDisabled: false,
      RIDocumentDateDisabled: false,
      RIDocumentFileDisabled: false,
      RIDocumentNoDisabled: false,
    };
  }
}
