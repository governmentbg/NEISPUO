import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { MunicipalInstitution } from './municipal-institution.interface';

export interface MunicipalInstitutionState extends EntityState<MunicipalInstitution> {
  total: number;
}
@Injectable({
  providedIn: 'root',
})
@StoreConfig({ name: 'municipal-institutions', idKey: 'RIInstitutionID' })
export class MunicipalInstitutionStore extends EntityStore<MunicipalInstitutionState> {
  constructor() {
    super({ total: 0 });
  }
}
