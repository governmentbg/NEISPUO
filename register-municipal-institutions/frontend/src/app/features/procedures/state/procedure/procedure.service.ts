import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RIInstitution } from '@municipal-institutions/models/ri-institution';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { tap } from 'rxjs/operators';
import * as lodash from 'lodash';
import { EnvironmentService } from '@core/services/environment.service';
import { ProcedureState, ProcedureStore } from './procedure.store';

export interface GetManyDefaultResponse<MunicipalInstitution> {
  data: MunicipalInstitution[];
  count: number;
  total: number;
  page: number;
  pageCount: number;
}

type PaginatedMunicipalInstitution = GetManyDefaultResponse<MunicipalInstitution>;

@Injectable()
export class ProcedureService {
  private baseEndpoint = this.envService.environment.BACKEND_URL;

  constructor(private httpClient: HttpClient, private procedureStore: ProcedureStore, private envService: EnvironmentService) { }

  setAllInstitutions() {
    return this.httpClient.get<PaginatedMunicipalInstitution>(`${this.baseEndpoint}/v1/ri-institution-latest-active`)
      .pipe(
        tap(
          ((institutionsResponse: PaginatedMunicipalInstitution) => {
            this.procedureStore.update({ availableMIs: institutionsResponse.data });
          }),
        ),
      ).toPromise();
  }

  setStoreProperty(key: keyof ProcedureState, mi: MunicipalInstitution) {
    const obj = {};
    obj[key] = mi;
    this.procedureStore.update(obj);
  }

  updateEntryArrayProperty(key: keyof ProcedureState, mi: MunicipalInstitution) {
    const index = (this.procedureStore.getValue()[key] as Array<MunicipalInstitution>).findIndex((i) => i.RIInstitutionID === mi.RIInstitutionID);
    this.procedureStore.getValue()[key][index] = mi;
  }

  addInArrayProperty(key: keyof ProcedureState, mi: MunicipalInstitution) {
    const existingArrayPropertyValue = lodash.cloneDeep(this.procedureStore.getValue()[key] || []);
    const obj = {};
    obj[key] = [...existingArrayPropertyValue, mi];
    this.procedureStore.update(obj);
  }

  removeFromArrayProperty(key: keyof ProcedureState, mi: MunicipalInstitution) {
    const index = (this.procedureStore.getValue()[key] as Array<MunicipalInstitution>).findIndex((i) => i.RIInstitutionID === mi.RIInstitutionID);
    (this.procedureStore.getValue()[key] as Array<MunicipalInstitution>).splice(index, 1);
  }

  performProcedure(path: string, body: any) {
    return this.httpClient.post<any>(`${this.baseEndpoint}${path}`, body);
  }

  /** Division */
  setDivideMiToDelete(miToDelete: MunicipalInstitution) {
    this.procedureStore.update({ divideMiToDelete: miToDelete });
  }

  addDivisionMiToCreate(institution: RIInstitution) {
    this.procedureStore.update({ divideMisToCreate: [...this.procedureStore.getValue()?.divideMisToCreate, institution] });
  }

  updateDivisionMiToCreate(institution: RIInstitution) {
    const index = this.procedureStore.getValue().divideMisToCreate.findIndex((i) => i.RIInstitutionID === institution.RIInstitutionID);
    this.procedureStore.getValue().divideMisToCreate[index] = institution;
  }

  removeDivisionMiToCreate(institution: RIInstitution) {
    const index = this.procedureStore.getValue().divideMisToCreate.findIndex((i) => i.RIInstitutionID === institution.RIInstitutionID);
    this.procedureStore.getValue().divideMisToCreate.splice(index, 1);
  }

  /** General */

  resetState() {
    this.procedureStore.reset();
  }
}
