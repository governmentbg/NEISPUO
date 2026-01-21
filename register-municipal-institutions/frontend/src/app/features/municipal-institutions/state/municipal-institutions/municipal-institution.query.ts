import { Injectable } from '@angular/core';
import { QueryEntity } from '@datorama/akita';
import { RouterQuery } from '@datorama/akita-ng-router-store';
import { MunicipalInstitutionState, MunicipalInstitutionStore } from './municipal-institution.store';

@Injectable({
  providedIn: 'root',
})
export class MunicipalInstitutionQuery extends QueryEntity<MunicipalInstitutionState> {
  institutions$ = this.selectAll();

  isLoading$ = this.selectLoading();

  total$ = this.select('total');

  selectFirst$ = this.selectFirst();

  selectedRIInstitutionID$ = this.routerQuery.selectParams('id');

  constructor(protected store: MunicipalInstitutionStore, private routerQuery: RouterQuery) {
    super(store);
  }
}
