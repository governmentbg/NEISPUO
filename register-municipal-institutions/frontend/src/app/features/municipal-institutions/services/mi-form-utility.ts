import { FormArray, FormGroup } from '@angular/forms';
import { AuthQuery } from '@core/authentication/auth-state-manager/auth.query';
import { BudgetingInstitutionEnum } from '@procedures/models/bugeting-institution.enum';
import { FinancialSchoolTypeEnum } from '@procedures/models/financial-school-type.enum';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { combineLatest } from 'rxjs';
import {
  filter,
} from 'rxjs/operators';
import { SubSink } from 'subsink';

export abstract class MiFormUtility {
  form: FormGroup;

  subs: SubSink;

  nmQuery: NomenclatureQuery;

  authQuery: AuthQuery;

  constructor(nmQuery: NomenclatureQuery, authQuery: AuthQuery) {
    this.subs = new SubSink();
    this.nmQuery = nmQuery;
    this.authQuery = authQuery;
  }

  presetMunicipality() {
    this.subs.sink = combineLatest([this.authQuery.selectedRole$, this.nmQuery.Municipalities$])
      .pipe(filter(([sr, mun]) => !!sr && !!mun)).subscribe(([sr, muns]) => {
        const userMunicipality = muns.find((m) => m.MunicipalityID === sr.MunicipalityID);
        this.form.patchValue({
          Region: userMunicipality.Region,
          Municipality: userMunicipality,
        });
      });
  }

  presetBudgetingInstitution() {
    this.subs.sink = this.nmQuery.BudgetingInstitutions$.pipe(filter((bis) => !!bis)).subscribe((bis) => {
      this.form.patchValue({ BudgetingInstitution: bis.find((bi) => bi.BudgetingInstitutionID === BudgetingInstitutionEnum.MUNICIPALITY) });
    });
  }

  presetFinancialSchoolType() {
    this.subs.sink = this.nmQuery.FinancialSchoolTypes$.pipe(filter((fsts) => !!fsts)).subscribe((fsts) => {
      this.form.patchValue({ FinancialSchoolType: fsts.find((fst) => fst.FinancialSchoolTypeID === FinancialSchoolTypeEnum.MUNICIPAL) });
    });
  }

  presetProcedureAndTransformType(procedureType: ProcedureTypeEnum, transformType: TransformTypeEnum) {
    this.subs.sink = this.nmQuery.ProcedureTypes$.pipe(filter((pts) => !!pts)).subscribe((pt) => {
      this.form.patchValue({ RIProcedure: { ProcedureType: pt.find((pts) => pts.ProcedureTypeID === procedureType) } });
    });
    this.subs.sink = this.nmQuery.TransformTypes$.pipe(filter((tts) => !!tts)).subscribe((tt) => {
      this.form.patchValue({ RIProcedure: { TransformType: tt.find((tts) => tts.TransformTypeID === transformType) } });
    });
  }

  disableRIFlexFieldArray(form: FormGroup) {
    for (const flexField of (form.get('RIFlexFieldValues') as FormArray).controls) {
      flexField.disable();
    }
  }
}
