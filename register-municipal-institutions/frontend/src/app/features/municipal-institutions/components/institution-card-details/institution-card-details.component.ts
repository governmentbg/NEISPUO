import {
  Component, Input, OnInit, Output,
} from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { RoleEnum } from '@authentication/models/role.enum';
import { RIInstitution } from '@municipal-institutions/models/ri-institution';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';
import { BudgetingInstitutionEnum } from '@procedures/models/bugeting-institution.enum';
import { FinancialSchoolTypeEnum } from '@procedures/models/financial-school-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { combineLatest } from 'rxjs';
import { filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-institution-card-details',
  templateUrl: './institution-card-details.component.html',
  styleUrls: ['./institution-card-details.component.scss'],
})
export class InstitutionCardDetailsComponent implements OnInit {
  @Output() public form: FormGroup | AbstractControl;

  config: IMIFormComponentState = {} as IMIFormComponentState;

  @Input() resp: RIInstitution;

  @Input() procedureType: string;

  @Input() displayRIPremInstitution: boolean = true;

  @Input() disableProcedureFields: boolean = true;

  @Input() disableRIInstitutionFields: boolean = true;

  BaseSchoolTypes$ = this.nmQuery.BaseSchoolTypes$
    .pipe(
      filter((baseSchoolTypes) => !!baseSchoolTypes),
      map(
        (baseSchoolTypes) => baseSchoolTypes
          .filter((bst) => bst.BaseSchoolTypeID === EnumBaseSchoolType.KINDERGARTEN || bst.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE),

      ),
    );

  constructor(private cfgs: MICommonFormGroupService, private nmQuery: NomenclatureQuery, private authQuery: AuthQuery) { }

  ngOnInit(): void {
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
        });
      });
  }

  private initForm() {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response: this.resp,
    });
  }

  setFormConfig() {
    /** set all to readonly */
    this.config = {
      NameDisabled: this.disableRIInstitutionFields,
      InstitutionIDDisabled: true,
      AbbreviationDisabled: this.disableRIInstitutionFields,
      BulstatDisabled: this.disableRIInstitutionFields,
      LocalAreaDisabled: this.disableRIInstitutionFields,
      FinancialSchoolType: true,
      BaseSchoolTypeDisabled: this.disableRIInstitutionFields,
      BudgetingInstitutionDisabled: true,
      DetailedSchoolTypeDisabled: this.disableRIInstitutionFields,
      CPLRAreaTypeDisabled: this.disableRIInstitutionFields,
      HeadFirstNameDisabled: this.disableRIInstitutionFields,
      HeadMiddleNameDisabled: this.disableRIInstitutionFields,
      HeadLastNameDisabled: this.disableRIInstitutionFields,
      TRPostCodeDisabled: this.disableRIInstitutionFields,
      TRAddressDisabled: this.disableRIInstitutionFields,
      ProcedureDateDisabled: this.disableProcedureFields,
      YearDueDisabled: this.disableProcedureFields,
      TransformDetailsDisabled: this.disableProcedureFields,
      RIDocumentFileDisabled: this.disableProcedureFields,
      RIDocumentNoDisabled: this.disableProcedureFields,
    };
  }
}
