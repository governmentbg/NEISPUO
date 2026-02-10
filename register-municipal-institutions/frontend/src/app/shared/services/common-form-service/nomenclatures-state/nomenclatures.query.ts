import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { NomenclatureState, NomenclatureStore } from './nomenclatures.store';

@Injectable({ providedIn: 'root' })
export class NomenclatureQuery extends Query<NomenclatureState> {
  Towns$ = this.select('Towns');

  Municipalities$ = this.select('Municipalities');

  Regions$ = this.select('Regions');

  LocalAreas$ = this.select('LocalAreas');

  FinancialSchoolTypes$ = this.select('FinancialSchoolTypes');

  BaseSchoolTypes$ = this.select('BaseSchoolTypes');

  BudgetingInstitutions$ = this.select('BudgetingInstitutions');

  DetailedSchoolTypes$ = this.select('DetailedSchoolTypes');

  CPLRAreaTypes$ = this.select('CPLRAreaTypes');

  ProcedureTypes$ = this.select('ProcedureTypes');

  TransformTypes$ = this.select('TransformTypes');

  BackendVersion$ = this.select('BackendVersion');

  CurrentYear$= this.select('CurrentYear')

  constructor(protected store: NomenclatureStore) {
    super(store);
  }
}
