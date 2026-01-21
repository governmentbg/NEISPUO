import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';
import { BaseSchoolType } from '@municipal-institutions/models/base-school-type';
import { BudgetingInstitution } from '@municipal-institutions/models/budgeting-institution';
import { DetailedSchoolType } from '@municipal-institutions/models/detailed-school-type';
import { FinancialSchoolType } from '@municipal-institutions/models/financial-school-type';
import { LocalArea } from '@municipal-institutions/models/local-area';
import { Municipality } from '@municipal-institutions/models/municipality';
import { ProcedureType } from '@municipal-institutions/models/procedure-type';
import { Region } from '@municipal-institutions/models/region';
import { Town } from '@municipal-institutions/models/town';
import { TransformType } from '@municipal-institutions/models/transform-type';
import { CPLRAreaType } from '@municipal-institutions/models/cplr-area-type';
import { BackendVersion } from '@municipal-institutions/models/backend-version';
import { CurrentYear } from '@municipal-institutions/models/current-year';

export interface NomenclatureState extends Store {
  Towns: Town[];
  Municipalities: Municipality[];
  Regions: Region[];
  FinancialSchoolTypes: FinancialSchoolType[];
  BaseSchoolTypes: BaseSchoolType[];
  BudgetingInstitutions: BudgetingInstitution[];
  DetailedSchoolTypes: DetailedSchoolType[];
  LocalAreas: LocalArea[];
  ProcedureTypes: ProcedureType[];
  TransformTypes: TransformType[];
  CPLRAreaTypes: CPLRAreaType[];
  BackendVersion:BackendVersion;
  CurrentYear: CurrentYear[]
}

export function initiateState() {
  return {} as NomenclatureState;
}

@Injectable({
  providedIn: 'root',
})
@StoreConfig({ name: 'nomenclatures' })
export class NomenclatureStore extends Store<NomenclatureState> {
  constructor() {
    super(initiateState());
  }
}
